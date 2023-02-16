using System;
using System.Collections;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
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

        public Interactable currentInteractable;
        public bool targetIsInteractable;
        public bool aIStopped;

        // Gunplay local variables
        public GameObject weapon;
        public static Action<Material, bool, float> FadeRoof;
        public static Action<bool> Detect;

        private Rig coverAnimRig;
        private Transform _defaultHeightConstraint;
        private Transform heightConstraint;
        private float coverAnimRigWeight;
        
        private bool crouch;
        private CapsuleCollider m_Capsule;
        
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            // coverAnimRig = GetComponent<RigBuilder>().layers[0].rig;
            // heightConstraint = coverAnimRig.gameObject.GetComponent<BlendConstraint>().data.sourceObjectB;
            // _defaultHeightConstraint = heightConstraint;

            // get the components on the object we need ( should not be null due to require component so no need to check )
            // agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            agent = GetComponent<NavMeshAgent>();
            characterMovement = GetComponent<CharacterMovement>();

            // ShootAt += ShootTarget;
	        agent.updateRotation = false;
	        agent.updatePosition = true;
        }

        private bool ToggleCrouch()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                crouch = !crouch;
            }

            return crouch;
        }

        private void Update()
        {
            timer += Time.deltaTime;

            if (crouch)
            {
                // coverAnimRigWeight = 1;
            }
            else
            {
                // coverAnimRigWeight = 0;
            }
            
            // coverAnimRig.weight = Mathf.Lerp(coverAnimRig.weight, coverAnimRigWeight, 0.05f);
            // coverAnimRig.weight = Mathf.Clamp(coverAnimRig.weight, 0, 1);
            
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
            
            // ----- RE-FORMAT
                    if ((target - transform.position).magnitude > 0.2f)
                    {
                        // print(Mathf.Abs((transform.position - target).magnitude));
                        characterMovement.Move(agent.desiredVelocity, ToggleCrouch(), false);

                        if ((target - transform.position).magnitude < 1f)
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
                                    // print(cachedTransform.eulerAngles);

                                    transform.eulerAngles = cachedTransform.eulerAngles;
                                    cachedTransform = null;
                                }


                                // WHEN THE PLAYER STOPS AT THE INTERACTABES TARGET LOCATION
                                if (targetIsInteractable)
                                {
                                    GameManager.isInteracting = true;
                                    InteractableTypeBehaviour();
                                    currentInteractable = null;
                                }
                            }
                        }
                    }
            
            switch (characterState)
            {
                case CharacterState.Dialogue:
                    // characterMovement.Move(agent.desiredVelocity, ToggleCrouch(), false);
                    break;
                
                
                case CharacterState.Exploration:
                    
                    if (CompareTag("Player") && Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit))
                    {
                        // cachedTransform = null;
                        // currentInteractable = null;
                        OnCLickedObjectBehaviour();
                    }

                    if ((target - transform.position).magnitude < 0.6f && agent.velocity.magnitude == 0 && cachedTransform != null)
                    {
                        // transform.eulerAngles = cachedTransform.eulerAngles;
                    }
                    // ----- RE FORMAT

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
        
        private void InteractableTypeBehaviour()
        {
                GameManager.Intelligence += currentInteractable.rewardIntel;
                print("adding intel: " + currentInteractable.rewardIntel);
                targetIsInteractable = false;
                print("ENABLE UI");
                cinemachineTarget.AddMember(currentInteractable.targetLocation.transform, 3, 0);
                
            switch (currentInteractable.typeOfInteractable)
            {
                case Interactable.TypeOfInteractable.Cover:

                    break;
                            
                case Interactable.TypeOfInteractable.Scripted:
                                
                    // Send action to Text reader to enable UI
                    textReader = currentInteractable.GetComponent<TextReader>();
                    textReader.ToggleUI();
                    
                    break;
                            
                case Interactable.TypeOfInteractable.Unscripted:

                    break;
            }
            
            // if (textReader.interactableStates == InteractableStates.NotInteracted || textReader.interactableStates == InteractableStates.InteractionOver)
            {
            }
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
                    
                    if (!GameManager.isInteracting)
                    {
                        Interactable tempInteractable = hit.collider.GetComponent<Interactable>();

                        if (tempInteractable.isVisible)
                        {
                            currentInteractable = tempInteractable;
                            print("INTERACTED WITH" + hit.collider.name);
                            targetTransform = currentInteractable.targetLocation.transform;
                            target = targetTransform.transform.position;
                            agent.SetDestination(target);
                            // print(interactable.typeOfInteractable);
                            targetIsInteractable = true;
                            //Send action to Text reader script to initialize dialogue but not display yet
                        }
                    }

                    break;
                
                default:
                        
                        
                        
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
            
            // print($"{gameObject.name.ToUpper()} STATE IS {characterState.ToString().ToUpper()}");
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

        private void OnTriggerStay(Collider other)
        {
            switch (other.tag)
            {
                // case "Cover":
                //
                //     if (crouch)
                //     {
                //         coverAnimRigWeight = 1;
                //         // print(other.bounds.extents.y);
                //         float tempY = other.bounds.max.y - 0.5f;
                //         print($"CONSTRAINT HEIGHT: {tempY}");
                //         heightConstraint.position = new Vector3(heightConstraint.position.x, Mathf.Lerp(heightConstraint.position.y, tempY, 0.05f), heightConstraint.position.z);
                //         m_Capsule.height = other.bounds.center.y;
                //         m_Capsule.center = new Vector3(m_Capsule.center.x, (other.bounds.center.y - other.bounds.min.y) / 2 + 0.1f, m_Capsule.center.z);
                //     }
                //     else
                //     {
                //         coverAnimRigWeight = 0;
                //     }
                //     break;
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
                
                // case "Cover":
                //     heightConstraint.position = new Vector3(heightConstraint.position.x, _defaultHeightConstraint.position.y, heightConstraint.position.z);
                //     coverAnimRigWeight = 0;
                //     break;
            }
        }
    }
}