using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (NavMeshAgent))]
    [RequireComponent(typeof (CharacterMovement))]
    public class CharacterControl : MonoBehaviour
    {
        public enum CharacterState
        {
            Cutscene,
            Exploration,
            Gunplay,
            Dead,
            None
        }

        public CharacterState characterState = CharacterState.Cutscene;
        
        [SerializeField] private CinemachineTargetGroup cinemachineTarget;
        
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
        public Weapon weapon;

        private Animator anim; 
        private Rig coverAnimRig;
        private Rig pointShootRig;
        private Transform _defaultHeightConstraint;
        private Transform heightConstraint;
        private float coverAnimRigWeight;
        
        public bool crouch;
        private CapsuleCollider m_Capsule;

        public LayerMask ignoreLayer;

        [SerializeField] private GameObject bulletTarget;
        
        private void Start()
        {
            anim = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            coverAnimRig = GetComponent<RigBuilder>().layers[0].rig;
            pointShootRig = GetComponent<RigBuilder>().layers[1].rig;
            heightConstraint = coverAnimRig.gameObject.GetComponent<BlendConstraint>().data.sourceObjectB;
            _defaultHeightConstraint = heightConstraint;

            // get the components on the object we need ( should not be null due to require component so no need to check )
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
                pointShootRig.weight = 0;
            }
            else
            {
                heightConstraint = _defaultHeightConstraint;
                coverAnimRigWeight = 0;
            }
            
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
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            // ----- RE-FORMAT
            if ((target - transform.position).magnitude > 0.2f)
            {
                characterMovement.Move(agent.desiredVelocity, ToggleCrouch(), false);
                agent.height = m_Capsule.height;

                if ((target - transform.position).magnitude < 1f)
                {
                    characterMovement.run = false;
                    
                    //CHARACTER STOPPED
                    // print(agent.velocity.magnitude);
                    if (agent.velocity.magnitude == 0)  //rb.velocity.magnitude < 0.4f
                    {
                        // print("STOPPED");
                        aIStopped = true;
                        
                        //TURN TO THE CACHED TRANSFORMS ROTATION THE END OF PATH
                        if (cachedTransform != null)
                        {
                            // print(cachedTransform.eulerAngles);
                            // transform.eulerAngles = cachedTransform.eulerAngles;
                            
                            transform.rotation = Quaternion.Slerp(transform.rotation, cachedTransform.rotation, Time.deltaTime * 5);
                            // transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, cachedTransform.eulerAngles, Time.deltaTime * 5);
                        }

                        // WHEN THE PLAYER STOPS AT THE INTERACTABES TARGET LOCATION
                        if (targetIsInteractable)
                        {
                            GameManager.IsInteracting = true;
                            InteractableTypeBehaviour();
                            // currentInteractable = null;
                        }
                    }
                }
            }
            // ----- RE FORMAT

            // RAYCAST AT THE MOUSE LOCATION IGNORING THE LAYERS MENTIONED
            Physics.Raycast(ray, out hit, Mathf.Infinity, ~ignoreLayer);

            // DETERMINES CHARACTERS BEHAVIOUR ON CLICKING ANYTHING IN THE WORLD BASED ON THE CHARACTER STATE
            if (CompareTag("Player") && Input.GetMouseButtonDown(0))
            {
                OnClickedPlayerBehaviour();
            }
            
            // WORLD FUNCTIONS BASED ON THE CHARACTER STATE
            switch (characterState)
            {
                case CharacterState.Cutscene: 
                    
                    // characterMovement.Move(agent.desiredVelocity, ToggleCrouch(), false);
                    
                    break;
                
                
                case CharacterState.Exploration:

                    // LERPING BETWEEN DIFFERENT COVER HEIGHTS
                    InterpolateCoverRIgWeight();
                    
                    break;
                
                
                case CharacterState.Gunplay:

                    transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
                    // transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, hit.point - transform.position, Time.deltaTime * 5);

                    bulletTarget.transform.position = hit.point;

                    
                    // LERPING BETWEEN DIFFERENT COVER HEIGHTS
                    InterpolateCoverRIgWeight();

                    // if (Input.GetMouseButtonDown(1))
                    if (Input.GetKeyDown(KeyCode.LeftControl)) 
                    {
                        // pointShootRig.weight = 0;
                        GameManager.IsInteracting = false;
                        characterState = CharacterState.Exploration;
                        weapon.gameObject.SetActive(false);
                        anim.SetBool("GunDrawn", false);

                        // print(currentInteractable);
                        currentInteractable.transform.root.tag = "Interactable";
                        cinemachineTarget.RemoveMember(currentInteractable.targetLocation.transform);
                    }
                    
                    break;

                case CharacterState.Dead:

                    m_Capsule.enabled = false;
                    agent.updatePosition = false;
                    
                    Rigidbody[] bones = GetComponentsInChildren<Rigidbody>();
                    
                    foreach (Rigidbody bone in bones)
                    {
                        bone.useGravity = true;
                        bone.isKinematic = false;
                        // bone.AddForce(new Vector3(rb.velocity.x, 0, rb.velocity.z) * 2, ForceMode.Impulse);
                        // bone.AddForce(rb.velocity * 2, ForceMode.Impulse);
                    }
                    anim.enabled = false;
                    // agent.SetDestination(transform.position);
                    agent.enabled = false;
                    rb.isKinematic = true;
                    characterState = CharacterState.None;
                    
                    break;
                
                default:
                    
                    print($"{gameObject.name} NOT SET TO ANY CHARACTER STATE");
                    
                    break;
            }

            if (gameObject.CompareTag("Player"))
            {
                DEBUG_CHEATS();
            }
        }

        private void InterpolateCoverRIgWeight()
        {
            coverAnimRig.weight = Mathf.Lerp(coverAnimRig.weight, coverAnimRigWeight, 0.2f);
            coverAnimRig.weight = Mathf.Clamp(coverAnimRig.weight, 0, 1);
        }

        private void InteractableTypeBehaviour()
        {
            GameManager.Intelligence += currentInteractable.rewardIntel;
            print("adding intel: " + currentInteractable.rewardIntel);
            targetIsInteractable = false;
            cinemachineTarget.AddMember(currentInteractable.targetLocation.transform, 3, 0);
                
            switch (currentInteractable.typeOfInteractable)
            {
                case Interactable.TypeOfInteractable.Cover:
                    
                    characterState = CharacterState.Gunplay;
                    currentInteractable.tag = "Cover";
                    weapon = currentInteractable.GetComponent<CoverInteractables>().weapon;
                   
                    if (weapon.isHandHeld)
                    {
                        weapon.gameObject.SetActive(true);
                        anim.SetBool("GunDrawn", true);
                    }
                    
                    crouch = true;
                    
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
        
        // DETERMINES CHARACTERS BEHAVIOUR ON CLICKING ANYTHING IN THE WORLD BASED ON THE CHARACTER STATE
        private void OnClickedPlayerBehaviour()
        {
            switch (characterState)
            {
                case CharacterState.Cutscene:
                    
                    // characterMovement.Move(agent.desiredVelocity, ToggleCrouch(), false);
                    
                    break;
                
                
                case CharacterState.Exploration:
                    
                    // cachedTransform = null;
                    // currentInteractable = null;
                    
                    //DOUBLE CLICK TO RUN
                    if (timer > doubleClickDelay && !characterMovement.run)
                    {
                        timer = 0;
                        characterMovement.run = false;
                    }
                    else
                    {
                        characterMovement.run = true;
                    }
                    
                    switch (hit.collider.tag)
                    {
                        case "Ground":

                            cachedTransform = null;
                            target = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                            agent.SetDestination(target);
                            aIStopped = false;

                            if (targetIsInteractable)
                            {
                                targetIsInteractable = false;
                                GameManager.IsInteracting = false;
                            }

                            break;

                        case "Interactable":
                    
                            if (!GameManager.IsInteracting)
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

                    break;
                
                
                case CharacterState.Gunplay:
                    
                    //BAD
                    switch (hit.collider.tag)
                    {
                        case "Interactable":

                            Interactable tempInteractable = hit.collider.GetComponent<Interactable>();
                            if (tempInteractable.typeOfInteractable == Interactable.TypeOfInteractable.Cover && tempInteractable.isVisible)
                            {
                                pointShootRig.weight = 0;
                                weapon.gameObject.SetActive(false);
                                anim.SetBool("GunDrawn", false);
                                crouch = false;
                                GameManager.IsInteracting = false;
                                characterState = CharacterState.Exploration;
                                currentInteractable.transform.root.tag = "Interactable";
                                cinemachineTarget.RemoveMember(currentInteractable.targetLocation.transform);
                                characterMovement.run = true;
                                currentInteractable = tempInteractable;
                                print("INTERACTED WITH" + hit.collider.name);
                                targetTransform = currentInteractable.targetLocation.transform;
                                target = targetTransform.transform.position;
                                agent.SetDestination(target);
                                // print(interactable.typeOfInteractable);
                                targetIsInteractable = true;
                            }

                            break;
                        
                
                        default:
                        
                            //SHOOT IF PLAYER CLICKED ANYTHING ELSE
                            // heightConstraint.transform.position += new Vector3(0, 1, 0);
                            
                            if (weapon.isHandHeld && !weapon.onCooldown)
                            {
                                    crouch = false;
                                    target = new Vector3(hit.point.x, hit.point.y, hit.point.z);

                                    //CALCULATE NEW ROTATION BASED ON PAYERS DRUNK STATE
                                    // bulletTargetPointer.transform.position = new Vector3(hit.point.x + Random.Range(-3, 3), hit.point.y + Random.Range(-3, 3), hit.point.z + Random.Range(-3, 3));
                                    
                                    anim.SetTrigger("Shoot");
                                    pointShootRig.weight = 1;

                                    weapon.ShootTarget(bulletTarget.transform.position);
                            }

                            break;
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
            else if (Input.GetMouseButtonDown(1))
            {
                characterState = CharacterState.Dead;
            }
            
            // print($"{gameObject.name.ToUpper()} STATE IS {characterState.ToString().ToUpper()}");
        }
        
        public void ChangeCharacterState(CharacterState characterState)
        {
            this.characterState = characterState;
        }

        private void OnTriggerStay(Collider other)
        {
            switch (other.tag)
            {
                case "Cover":
                    
                    if (crouch)
                    {
                        coverAnimRigWeight = 1;
                        // print(other.bounds.extents.y);
                        float tempY = other.bounds.max.y - 0.5f;
                        // print($"CONSTRAINT HEIGHT: {tempY}");
                        heightConstraint.position = new Vector3(heightConstraint.position.x, Mathf.Lerp(heightConstraint.position.y, tempY, 0.1f), heightConstraint.position.z);
                        m_Capsule.height = other.bounds.center.y;
                        m_Capsule.center = new Vector3(m_Capsule.center.x, (other.bounds.center.y - other.bounds.min.y) / 2 + 0.1f, m_Capsule.center.z);
                        agent.height = m_Capsule.height;
                    }
                    
                    break;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            switch (other.tag)
            {
                case "Cover":
                    heightConstraint.position = new Vector3(heightConstraint.position.x, _defaultHeightConstraint.position.y, heightConstraint.position.z);
                    coverAnimRigWeight = 0;
                    break;
            }
        }

        private void OnDrawGizmos()
        {
            if (bulletTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(bulletTarget.transform.position, 0.2f);
            } 
        }
    }
}