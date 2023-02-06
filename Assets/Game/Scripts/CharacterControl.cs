using System;
using System.Collections;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (NavMeshAgent))]
    [RequireComponent(typeof (CharacterMovement))]
    public class CharacterControl : MonoBehaviour
    {
        public enum CharacterState
        {
            Dialogue,
            Exploration,
            Gunplay,
            Dead
        }

        public CharacterState characterState = CharacterState.Dialogue;
        
        [SerializeField] private CinemachineTargetGroup cinemachineTarget;
        [SerializeField] private GameObject bulletTrail;
        [SerializeField] private GameObject muzzleFlash;
        [SerializeField] private GameObject bulletSpawnLoc;
        
        public string name;
        public Transform targetTransform;
        public Transform cachedTransform;
        private Rigidbody rb;
        public NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
        public CharacterMovement characterMovement { get; private set; } // the character we are controlling
        public Vector3 target;                                    // target to aim for
        public float doubleClickDelay;
        public float timer;
        
        private bool cooldown;
        private RaycastHit hit;
        private TextReader textReader;

        private bool targetIsInteractable;
        public bool aIStopped;
        
        
        private int deltaIntelligence;
        
        // Gunplay local variables
        public GameObject weapon;
        public static Action<Material, bool, float> FadeRoof;
        public static Action<bool> Detect;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            // get the components on the object we need ( should not be null due to require component so no need to check )
            // agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            agent = GetComponent<NavMeshAgent>();
            characterMovement = GetComponent<CharacterMovement>();

            deltaIntelligence = 0;
            // ShootAt += ShootTarget;
	        agent.updateRotation = false;
	        agent.updatePosition = true;
        }


        private void Update()
        {
            timer += Time.deltaTime;

            // print(Vector3.Distance(transform.position, Camera.main.transform.position));
            // print(gameObject.name + aIStopped);

            if (targetTransform != null)
            {
                aIStopped = false;
                target = targetTransform.position;
                cachedTransform = targetTransform;
                targetTransform = null;
                agent.SetDestination(target);
            }
            else if (target == Vector3.zero)
            {
                // target = transform.position;
            }
            
            // print(agent.isStopped);
            // print(agent.remainingDistance);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
          
            switch (characterState)
            {
                case CharacterState.Dialogue:
                    
                    break;
                
                
                case CharacterState.Exploration:

                    // ----- RE-FORMAT
                    if ((target - transform.position).magnitude > 0.3f)
                    {
                        // print(Mathf.Abs((transform.position - target).magnitude));
                        characterMovement.Move(agent.desiredVelocity, Input.GetKey(KeyCode.LeftControl), false);

                        if ((target - transform.position).magnitude < 0.8f)
                        {
                            characterMovement.run = false;
                            //TURN AT THE END OF PATH
                            // print((target - transform.position).magnitude);
                            // print(agent.velocity.magnitude);
                            if (agent.velocity.magnitude == 0)  //rb.velocity.magnitude < 0.4f
                            {
                                // print("STOPPED");
                                aIStopped = true;
                                if (cachedTransform != null)
                                {
                                    print(cachedTransform.eulerAngles);

                                    transform.eulerAngles = cachedTransform.eulerAngles;
                                }
                    
                    
                                if (targetIsInteractable && (textReader.interactableStates == InteractableStates.NotInteracted ||
                                                             textReader.interactableStates == InteractableStates.InteractionOver))
                                {
                                    GameManager.isInteracting = true;
                                    print("adding intel: " + deltaIntelligence);
                                    GameManager.Intelligence += deltaIntelligence;
                                    targetIsInteractable = false;
                                    textReader.ToggleUI();
                                    // Send action to Text reader to enable UI
                                    print("ENABLE UI");

                                    cinemachineTarget.AddMember(cachedTransform, 3, 0);
                                }
                            }
                        }
                    }
                    
                    if ((target - transform.position).magnitude < 0.6f && agent.velocity.magnitude == 0 && cachedTransform != null)
                    {
                        // transform.eulerAngles = cachedTransform.eulerAngles;
                    }
                    // ----- RE FORMAT
                    
                    
                    if (CompareTag("Player") && Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit))
                    {
                        cachedTransform = null;
                        OnCLickedObjectBehaviour();
                    }

                    break;
                
                
                case CharacterState.Gunplay:

                    if (Physics.Raycast(ray, out hit))
                    {
                        // Get weapon behavior from an object to allow customization
                        // Cap fire rate
                        // if (timer >= timeBetweenShots)
                        // {
                        //     timer = 0;
                        // }
                        
                        target = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                        ShootTarget(target);
                    }
                    
                    break;
                
                
                // case CharacterState.Dead:
                //     
                //     break;
                
                default:
                    
                    print($"{gameObject.name} NOT SET TO ANY CHARACTER STATE");
                    
                    break;
            }
            
            DEBUG_CHEATS();
        }

        private void OnCLickedObjectBehaviour()
        {
            switch (hit.collider.tag)
            {
                case "Ground":
                    if (timer > doubleClickDelay && !characterMovement.run)
                    {
                        timer = 0;
                        characterMovement.run = false;
                    }
                    else
                    {
                        characterMovement.run = true;
                    }

                    target = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                    agent.SetDestination(target);
                    aIStopped = false;

                    if (targetIsInteractable)
                    {
                        targetIsInteractable = false;
                        GameManager.isInteracting = false;
                    }

                    break;

                case "Interactable":
                    Interactable interactable = hit.collider.GetComponent<Interactable>();
                    textReader = hit.collider.GetComponent<TextReader>();
                    print("INTERACTED WITH" + hit.collider.name);

                    if (interactable.isVisible && !GameManager.isInteracting)
                    {
                        targetTransform = interactable.targetLocation.transform;
                        target = targetTransform.transform.position;
                        agent.SetDestination(target);
                        deltaIntelligence = interactable.playerIntelChange;
                        targetIsInteractable = true;
                        //Send action to Text reader script to initialize dialogue but not display yet
                    }

                    break;
            }
        }

        private void DEBUG_CHEATS()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                characterState = CharacterState.Exploration;
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                characterState = CharacterState.Gunplay;
            }
            
            print($"{gameObject.name.ToUpper()} STATE IS {characterState.ToString().ToUpper()}");
        }
        
        public void ChangeCharacterState(CharacterState characterState)
        {
            this.characterState = characterState;
        }
        
        private void ShootTarget(Vector3 target)
        {
            // print(bulletSpawnLoc.transform.forward);
            Instantiate(muzzleFlash, bulletSpawnLoc.transform.position, Quaternion.identity);
            GameObject temp = Instantiate(bulletTrail, bulletSpawnLoc.transform.position, Quaternion.identity);
            temp.transform.LookAt(target);
            // print(temp.transform.rotation.eulerAngles);
            // bullet.GetComponent<BulletControl>().target = target;

        }
        private void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case "IndoorsVolume":
                    print(other.name);
                    print("ENTERED VOLUME");
                    // FADES THE ROOF
                    Material tempMat = other.transform.root.GetComponent<MeshRenderer>().material;
                    FadeRoof(tempMat, true, 1);
                    Detect(true);
                    break;
                
                case "GunplayCover":
                    print("ENTERED GUNPLAY");
                    // GameManager.IsMoveable = false;
                    characterState = CharacterState.Gunplay;
                    timer = 0;
                    break;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            switch (other.tag)
            {
                case "IndoorsVolume":
                    print("EXITED VOLUME");
                    Material tempMat = other.transform.root.GetComponent<MeshRenderer>().material;
                    FadeRoof(tempMat, false, 8);
                    Detect(false);
                    break;
            }
        }
    }
}
