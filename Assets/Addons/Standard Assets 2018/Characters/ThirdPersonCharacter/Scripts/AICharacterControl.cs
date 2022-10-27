using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (NavMeshAgent))]
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class AICharacterControl : MonoBehaviour
    {
        public string name;
        [SerializeField] private Transform targetTransform;
        public Transform cachedTransform;
        private Rigidbody rb;
        public NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
        public ThirdPersonCharacter character { get; private set; } // the character we are controlling
        public Vector3 target;                                    // target to aim for

        private bool cooldown;
        private RaycastHit hit;
        private TextReader textReader;

        private bool targetIsInteractable;

        public bool aIStopped;
        
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            // get the components on the object we need ( should not be null due to require component so no need to check )
            // agent = GetComponentInChildren<UnityEngine.AI.NavMeshAgent>();
            agent = GetComponent<NavMeshAgent>();
            character = GetComponent<ThirdPersonCharacter>();

	        agent.updateRotation = false;
	        agent.updatePosition = true;
        }


        private void Update()
        {
            if (targetTransform != null)
            {
                target = targetTransform.position;
                cachedTransform = targetTransform;
                targetTransform = null;
                agent.SetDestination(target);
            }
            else if (target == Vector3.zero)
            {
                // target = transform.position;
            }
            
            // disable if player isMoveable
            // print(agent.remainingDistance);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0) && GameManager.IsMoveable && CompareTag("Player"))
            {
                cachedTransform = null;
                switch (hit.collider.tag)
                {
                    case "Ground":
                        target = new Vector3(hit.point.x, 0, hit.point.z);
                        agent.SetDestination(target);

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
                            target = interactable.targetLocation.transform.position;
                            agent.SetDestination(target);
                            targetIsInteractable = true;
                            GameManager.isInteracting = true;
                            //Send action to Text reader script to initialize dialogue but not display yet
                        }
                        break;
                }
            }
                
            // print(agent.isStopped);
            // print(agent.remainingDistance);
            if ((target - transform.position).magnitude > 0.3f)
            {
                // print(Mathf.Abs((transform.position - target).magnitude));
                character.Move(agent.desiredVelocity, false, false);

                if ((target - transform.position).magnitude < 0.8f)
                {
                    //TURN AT THE END OF PATH
                    print((target - transform.position).magnitude);
                    if (agent.velocity.magnitude == 0 && cachedTransform != null)
                    {
                        // print("STOPPED");
                        aIStopped = true;
                        transform.eulerAngles = cachedTransform.eulerAngles;
                    }



                    if (targetIsInteractable && (textReader.interactableStates == InteractableStates.NotInteracted ||
                                                 textReader.interactableStates == InteractableStates.InteractionOver))
                    {
                        targetIsInteractable = false;
                        textReader.ToggleUI();
                        // Send action to Text reader to enable UI
                        print("ENABLE UI");
                    }
                    
                }
            }
            else
            {
                
            }
            
            if ((target - transform.position).magnitude < 0.6f && agent.velocity.magnitude == 0 && cachedTransform != null)
            {
                    // transform.eulerAngles = cachedTransform.eulerAngles;
            }
        }
    }
}
