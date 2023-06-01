using Cinemachine;
using UnityEngine;

public class PlayerCharacterControl : CharacterControl
{    
    [SerializeField] Texture2D mouseShootingTexture;

    [SerializeField] private GameObject shootLoc;
    [SerializeField] private GameObject clickLocUI;
    
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

                Cursor.SetCursor(mouseShootingTexture, new Vector2 (mouseShootingTexture.width / 2, mouseShootingTexture.height / 2), CursorMode.Auto);
                transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
                // transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, hit.point - transform.position, Time.deltaTime * 5);

                bulletTarget.transform.position = hit.point;

                // if (Input.GetMouseButtonDown(1))
                if (Input.GetMouseButtonDown(1)) 
                {
                    pointShootRig.weight = 0;
                    cachedTransform = null;
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
            
            default:
                
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

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
                        
                        // CLIKING UI
                        clickLocUI.transform.position = new Vector3(target.x, target.y + 0.2f, target.z);
                        clickLocUI.SetActive(true);

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
                        if (tempInteractable && tempInteractable.typeOfInteractable == Interactable.TypeOfInteractable.Cover && tempInteractable.isVisible)
                        {
                            pointShootRig.weight = 0;
                            weapon.gameObject.SetActive(false);
                            anim.SetBool("GunDrawn", false);
                            crouch = false;
                            GameManager.IsInteracting = false;
                            characterState = CharacterState.Exploration;
                            currentInteractable.tag = "Interactable";
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
                        else
                        {
                            Shoot();
                        }

                        break;
                    
            
                    default:
                    
                        //SHOOT IF PLAYER CLICKED ANYTHING ELSE
                        // heightConstraint.transform.position += new Vector3(0, 1, 0);
                        
                        Shoot();
                        
                        break;
                }

                break;
            
            
            default:
                
                
                break;
        }
    }

    public void Shoot()
    {
        if (weapon.isHandHeld && !weapon.onCooldown)
        {
            crouch = false;
            target = new Vector3(hit.point.x, hit.point.y, hit.point.z);

            float accuracy = GameManager.Drunkenness / (150 - GameManager.PlayerHealth);
            //CALCULATE NEW ROTATION BASED ON PAYERS DRUNK STATE
            if (GameManager.Drunkenness > 0)
            {
                // float accuracy = 0.8f;
                shootLoc.transform.position = new Vector3(hit.point.x + Random.Range(-accuracy, accuracy), hit.point.y + Random.Range(-accuracy, accuracy), hit.point.z + Random.Range(-accuracy, accuracy));

                if (accuracy > 0.7f && accuracy < 1f)
                {
                    weapon.damage = 20f;
                }
                else if (accuracy > 0.5f && accuracy < 0.7f)
                {
                    weapon.damage = 15f;
                }
                else
                {
                    weapon.damage = 12f;
                }
            }
            else
            {
                // IF NOT DRUNK
                shootLoc.transform.position = bulletTarget.transform.position;
            }
            
            print($"{GameManager.Drunkenness} : {accuracy} : {weapon.damage}");
            anim.SetTrigger("Shoot");
            pointShootRig.weight = 1;

            weapon.ShootTarget(shootLoc);
        }
    }
    
    protected override void InteractableTypeBehaviour()
    {
        GameManager.IsInteracting = true;
        
        base.InteractableTypeBehaviour();
        
        switch (currentInteractable.typeOfInteractable)
        {
            case Interactable.TypeOfInteractable.Scripted:
                            
                cinemachineTarget.AddMember(currentInteractable.targetLocation.transform, 3, 0);
                // Send action to Text reader to enable UI
                textReader = currentInteractable.GetComponent<TextReader>();
                textReader.ToggleUI();
                
                break;
                        
            case Interactable.TypeOfInteractable.Inspectable:

                currentInteractable.GetComponent<InspectableInteractables>().SetupStudio();

                break;
            
            case Interactable.TypeOfInteractable.Cover:

                // cinemachineTarget.m_Targets[0].weight = 2;
                foreach (CharacterControl characterControl in enemies)
                {
                    bool hasRepeated = false;
                    foreach (CinemachineTargetGroup.Target target in cinemachineTarget.m_Targets)
                    {
                        if (target.target == characterControl.transform)
                        {
                            hasRepeated = true;
                        }

                    }

                    if (!hasRepeated && characterControl.characterState != CharacterState.Cutscene)
                    {
                        cinemachineTarget.AddMember(characterControl.transform, 2, 2);
                    }
                }
                
                break;
        }
        
    }
    
    // protected override void OnTriggerStay(Collider other)
    // {
    //     base.OnTriggerStay(other);
    //
    //     if (other.transform.root.gameObject != gameObject &&
    //         (other.transform.root.CompareTag("Player") ||
    //          other.transform.root.CompareTag("AI")))
    //     {
    //         CharacterControl characterControl = other.transform.root.GetComponent<CharacterControl>();
    //         if (characterControl.isFriendly == isFriendly)
    //         {
    //             bool repeatedCheck = true;
    //             for (int j = 0; j < friends.Count; j++)
    //             {
    //                 if (characterControl == friends[j])
    //                 {
    //                     repeatedCheck = false;
    //                 }                    
    //             }
    //
    //             if (repeatedCheck)
    //             {
    //                 friends.Add(characterControl);
    //             }
    //         }
    //         else
    //         {
    //             bool repeatedCheck = true;
    //             for (int j = 0; j < enemies.Count; j++)
    //             {
    //                 if (characterControl == enemies[j])
    //                 {
    //                     repeatedCheck = false;
    //                 }                    
    //             }
    //
    //             if (repeatedCheck)
    //             {
    //                 enemies.Add(characterControl);
    //             }
    //         }
    //         
    //         if (characterControl.characterState == CharacterState.None)
    //         {
    //             friends.Remove(characterControl);
    //             enemies.Remove(characterControl);
    //         }
    //     }
    //     
    // }

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
