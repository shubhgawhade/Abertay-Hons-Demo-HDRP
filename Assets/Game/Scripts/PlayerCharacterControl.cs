using Cinemachine;
using UnityEngine;

public class PlayerCharacterControl : CharacterControl
{
    [SerializeField] private CinemachineTargetGroup cinemachineTarget;
    
    private RaycastHit hit;
    private TextReader textReader;
    
    public float doubleClickDelay = 0.2f;
    public float timer;
    
    public LayerMask ignoreLayer;


    protected override bool ToggleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            crouch = !crouch;
        }
        
        return base.ToggleCrouch();
    }

    // Update is called once per frame
    protected override void Update()
    {
        timer += Time.deltaTime;
        isInteracting = GameManager.IsInteracting;

        base.Update();
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // RAYCAST AT THE MOUSE LOCATION IGNORING THE LAYERS MENTIONED
        Physics.Raycast(ray, out hit, Mathf.Infinity, ~ignoreLayer);
        
        // DETERMINES CHARACTERS BEHAVIOUR ON CLICKING ANYTHING IN THE WORLD BASED ON THE CHARACTER STATE
        if (Input.GetMouseButtonDown(0))
        {
            OnClickedPlayerBehaviour();
        }
        
        PlayerUpdate();
        
        if (gameObject.CompareTag("Player"))
        {
            // DEBUG_CHEATS();
        }
        
    }

    private void PlayerUpdate()
    {
        if ((target - transform.position).magnitude > 0.2f)
        {
            // IF THE INTERACTABLE IS OCCUPIED BEFORE THE CHARACTER REACHES IT
            if (targetIsInteractable && currentInteractable.isOccupied && currentInteractable.typeOfInteractable == Interactable.TypeOfInteractable.Cover)
            {
                cachedTransform = null;
                currentInteractable = null;
                target = transform.position;
                agent.SetDestination(target);
                targetIsInteractable = false;
            }
        }
        
        if (targetIsInteractable && currentInteractable.isOccupied && currentInteractable.typeOfInteractable == Interactable.TypeOfInteractable.Cover)
        {
            cachedTransform = null;
            currentInteractable = null;
            target = transform.position;
            agent.SetDestination(target);
            targetIsInteractable = false;
        }

        // WORLD FUNCTIONS BASED ON THE CHARACTER STATE
        switch (characterState)
        {
            case CharacterState.Gunplay:

                transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
                // transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, hit.point - transform.position, Time.deltaTime * 5);

                bulletTarget.transform.position = hit.point;

                // if (Input.GetMouseButtonDown(1))
                if (Input.GetKeyDown(KeyCode.LeftControl)) 
                {
                    // pointShootRig.weight = 0;
                    GameManager.IsInteracting = false;
                    currentInteractable.isOccupied = false;
                    characterState = CharacterState.Exploration;
                    weapon.gameObject.SetActive(false);
                    anim.SetBool("GunDrawn", false);

                    print(currentInteractable);
                    currentInteractable.tag = "Interactable";
                    cinemachineTarget.RemoveMember(currentInteractable.targetLocation.transform);
                }
                
                break;
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
                                // print("INTERACTED WITH" + hit.collider.name);
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
                            currentInteractable.isOccupied = false;
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

                                weapon.ShootTarget(bulletTarget);
                        }

                        break;
                }

                break;
            
            
            default:

                break;
        }
    }
    
    protected override void InteractableTypeBehaviour()
    {
        GameManager.IsInteracting = true;
        cinemachineTarget.AddMember(currentInteractable.targetLocation.transform, 3, 0);
        
        base.InteractableTypeBehaviour();
        
        switch (currentInteractable.typeOfInteractable)
        {
            case Interactable.TypeOfInteractable.Scripted:
                            
                // Send action to Text reader to enable UI
                textReader = currentInteractable.GetComponent<TextReader>();
                textReader.ToggleUI();
                
                break;
                        
            case Interactable.TypeOfInteractable.Inspectable:

                currentInteractable.GetComponent<InspectableInteractables>().SetupStudio();
                
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
            // characterState = CharacterState.Dead;
        }
        
        // print($"{gameObject.name.ToUpper()} STATE IS {characterState.ToString().ToUpper()}");
    }
}
