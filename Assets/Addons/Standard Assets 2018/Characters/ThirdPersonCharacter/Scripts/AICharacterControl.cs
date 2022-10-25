using UnityEngine;
using UnityEngine.AI;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof (NavMeshAgent))]
    [RequireComponent(typeof (ThirdPersonCharacter))]
    public class AICharacterControl : MonoBehaviour
    {
        [SerializeField] private Transform targetTransform;
        public NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
        public ThirdPersonCharacter character { get; private set; } // the character we are controlling
        public Vector3 target;                                    // target to aim for

        private bool cooldown;
        private RaycastHit hit;
        private TextReader textReader;

        private bool targetIsInteractable;
        private void Start()
        {
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
                targetTransform = null;
                agent.SetDestination(target);
            }
            else if (target == Vector3.zero)
            {
                target = transform.position;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit) && Input.GetMouseButtonDown(0) && GameManager.IsMoveable)
            {
                switch (hit.collider.tag)
                {
                    case "Ground":
                        target = new Vector3(hit.point.x, 0, hit.point.z);
                        agent.SetDestination(target);
                        break;
                    
                    case "Interactable":
                        Interactable interactable = hit.collider.GetComponent<Interactable>();
                        textReader = hit.collider.GetComponent<TextReader>();
                        print("INTERACTED WITH" + hit.collider.name);

                        if (interactable.isinteracted)
                        {
                            target = interactable.targetLocation.transform.position;
                            agent.SetDestination(target);
                            targetIsInteractable = true;
                            
                            //Send action to Text reader script to initialize dialogue but not display yet
                        }
                        break;
                }
            }
                
            // print(agent.isStopped);

            // print(agent.remainingDistance);
            if (agent.remainingDistance > 0.3f)
            {
                // print(Mathf.Abs((transform.position - target).magnitude));
                character.Move(agent.desiredVelocity, false, false);

                if ((target - transform.position).magnitude < 0.8f && textReader != null && targetIsInteractable)
                {
                    if (textReader.interactableStates == InteractableStates.NotInteracted ||
                        textReader.interactableStates == InteractableStates.InteractionOver)
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
        }
    }
}
