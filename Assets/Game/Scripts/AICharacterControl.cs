using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[Serializable]
public class AICharacterControl : CharacterControl
{
    private enum InternalBehaviour
    {
        None,
        Chase,
        Flee,
        Combat
    }

    [Header("AI SETTINGS")]
    [Space(10)]
    [SerializeField] private InternalBehaviour aiBehaviour = InternalBehaviour.None;
    
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject aiRayLoc;
    [SerializeField] private GameObject aiTarget;
    [SerializeField] private List<Interactable> gunplayLocations;
    // [SerializeField] private Collider[] collidersNearby;
    // [SerializeField] private List<Vector3> targetLocations;

    [SerializeField] private bool chooseLocation;
    
    private Vector3 dir;
    public bool targetKnown;
    public float time;


    public LayerMask hitLayer;
    public bool isFriendly;
    
    RaycastHit hit;

    protected override bool ToggleCrouch()
    {
        // KEY TO TOGGLE CROUCH
        if (Input.GetMouseButtonDown(0))
        {
            // crouch = !crouch;
        }
        
        return base.ToggleCrouch();
    }

    protected override void Update()
    {
        base.Update();
        
        AIUpdate();
        
        // CODE FOR AI TO CHOOSE LOCATIONS TO MOVE TO AND SELECT COVER POSITIONS
        // BASED ON THE SITUATION SHOULD THE AI BE LOOKING FOR THE PLAYER, IN COMBAT OR PATROLLING
        AIBehaviour();
        
        // CODE TO SELECT A RANDOM COVER LOCATION TO GO TO
        if (Input.GetMouseButtonDown(0))
        {
            // crouch = false;
            // characterState = CharacterState.Exploration;
            // int location = Random.Range(0, gunplayLocations.Count);
            // targetTransform = gunplayLocations[location].targetLocation.transform;
            // currentInteractable = gunplayLocations[location];
            // targetIsInteractable = true;
        }
        
        
    }
    
    

    private void AIUpdate()
    {
        switch (characterState)
        {
            // CODE FOR AI TO CHOOSE SHOOTING LOCATION 
            case CharacterState.Gunplay:

                // Vector3 targetDirection = new Vector3(aiTarget.transform.position.x, transform.position.y, aiTarget.transform.position.z) - new Vector3(transform.position.x, transform.position.y, transform.position.z);
                // Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5);
                
                // if (targetKnown)
                {
                    transform.LookAt(new Vector3(aiTarget.transform.position.x, transform.position.y, aiTarget.transform.position.z));
                }
                
                if (Input.GetKeyDown(KeyCode.S))
                {
                    if (weapon.isHandHeld && !weapon.onCooldown)
                    {
                        crouch = false;

                        //CALCULATE NEW ROTATION BASED ON PAYERS DRUNK STATE
                        // bulletTargetPointer.transform.position = new Vector3(hit.point.x + Random.Range(-3, 3), hit.point.y + Random.Range(-3, 3), hit.point.z + Random.Range(-3, 3));
                                
                        anim.SetTrigger("Shoot");
                        pointShootRig.weight = 1;

                        // SELECT WHERE TO SHOOT BEFORE SHOOTING
                        weapon.ShootTarget(aiTarget.transform.position);
                    }
                    
                    // weapon.ShootTarget(transform.forward);
                }

                // transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
                // bulletTarget.transform.position = hit.point;
                //
                // if (Input.GetKeyDown(KeyCode.LeftControl)) 
                // {
                //     GameManager.IsInteracting = false;
                //     characterState = CharacterState.Exploration;
                //     weapon.gameObject.SetActive(false);
                //     anim.SetBool("GunDrawn", false);
                //     currentInteractable.transform.root.tag = "Interactable";
                //     cinemachineTarget.RemoveMember(currentInteractable.targetLocation.transform);
                // }
                
                break;
            
            case CharacterState.None:

                aiBehaviour = InternalBehaviour.None;
                
                break;
        }
    }

    private void AIBehaviour()
    {
        // VISION CONE DETECTION AND CHASE
        
        // dir = player.transform.position - aiRayLoc.transform.position;
        //
        // if (time > 1f)
        // {
        //     targetKnown = false;
        //     time = 0;
        // }
        // else if (Vector3.Dot(transform.forward.normalized, dir.normalized) > 0.8)
        // {
        //     Ray ray = new Ray(aiRayLoc.transform.position, dir);
        //     if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayer))
        //     {
        //         print(hit.collider.transform.root.name + " " + hit.collider.name);
        //         if (hit.transform.root.CompareTag("Player"))
        //         {
        //             targetKnown = true;
        //             time = 0;
        //         }
        //     }
        //     else
        //     {
        //     }
        //     
        //     aiTarget.transform.position = hit.point;
        // }
        
        float playerBasePos = player.transform.position.y - 1f;
        for (int i = 0; i < 5; i++)
        {
            Vector3 dir = new Vector3(player.transform.position.x, playerBasePos + 0.5f * i, player.transform.position.z) - aiRayLoc.transform.position + new Vector3(0, 0.2f, 0) + transform.forward * 0.25f;
            
            if (time > 1f)
            {
                targetKnown = false;
                time = 0;
            }
            else if (Vector3.Dot(transform.forward.normalized, new Vector3(dir.x, 0, dir.z).normalized) > 0.8)
            {
                Ray ray = new Ray(aiRayLoc.transform.position + new Vector3(0, 0.2f, 0) + transform.forward * 0.25f, dir);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayer))
                {
                    // print(hit.collider.transform.root.name + " " + hit.collider.name);
                    if (hit.transform.root.CompareTag("Player"))
                    {
                        targetKnown = true;
                        time = 0;
                        aiTarget.transform.position = hit.point;
                    }
                    
                }
                else
                {
                }
            }
        }

        if (targetKnown)
        {
            time += Time.deltaTime;
        }


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
                ChooseLocation();
            }
        }


        switch (aiBehaviour)
        {
            case InternalBehaviour.Chase:

                if (targetKnown)
                {
                    targetTransform = player.transform;
                    // targetLocations.Add(target);
                    agent.SetDestination(targetTransform.position);

                    if ((player.transform.position - transform.position).magnitude < 10f)
                    {
                        agent.SetDestination(transform.position);
                        // targetKnown = false;
                        aiBehaviour = InternalBehaviour.Combat;
                        chooseLocation = true;
                    }
                }
                
                break; 
            
            case InternalBehaviour.Combat:

                // UPDATES AVAILABLE LOCATIONS
                for (int i = 0; i < gunplayLocations.Count; i++)
                {
                    if (gunplayLocations[i].isOccupied)
                    {
                        print($"{gameObject.name} REMOVING {gunplayLocations[i]}");
                        gunplayLocations.Remove(gunplayLocations[i]);
                        i = 0;
                    }                        
                }
                
                if (targetKnown && chooseLocation)
                // if (Input.GetKeyDown(KeyCode.A))
                {
                    chooseLocation = false;
                    // SCAN FOR NEARBY COVER INTERACTABLES
                    // collidersNearby = Physics.OverlapSphere(transform.position, 15);
                    //
                    // foreach (Collider col in collidersNearby)
                    // {
                    //     if (col.CompareTag("Interactable"))
                    //     {
                    //         Interactable tempInteractable = col.GetComponent<Interactable>();
                    //         if (tempInteractable.typeOfInteractable == Interactable.TypeOfInteractable.Cover)
                    //         {
                    //             gunplayLocations.Add(tempInteractable);
                    //         }
                    //     }
                    // }
                    
                    // SWITCHING COVER POSITION
                    if (isInteracting)
                    {
                        pointShootRig.weight = 0;
                        weapon.gameObject.SetActive(false);
                        anim.SetBool("GunDrawn", false);
                        // NO NEED FOR ROOT AS CURRENT INTERACTABLE IS OF TYPE INTERACTABLE
                        currentInteractable.transform.root.tag = "Interactable";
                        isInteracting = false;
                        currentInteractable.isOccupied = false;
                    }
                    
                    // CHOOSING COVER LOCATION
                    if (!ChooseLocation())
                    {
                        // SWITCH BACK TO COMBAT FROM CHASE WHEN IT FINDS COVER
                        
                    }

                    // int location = Random.Range(0, gunplayLocations.Count);
                    // do
                    // {
                    //     location = Random.Range(0, gunplayLocations.Count);
                    //     print(gameObject.name);
                    //     targetTransform = gunplayLocations[location].targetLocation.transform;
                    //     currentInteractable = gunplayLocations[location];
                    //     targetIsInteractable = true;
                    //     
                    // } while (gunplayLocations[location] != currentInteractable);

                    // int location = Random.Range(0, gunplayLocations.Count);
                    // targetTransform = gunplayLocations[location].targetLocation.transform;
                    // currentInteractable = gunplayLocations[location];
                    // targetIsInteractable = true;
                }
                
                        if ((player.transform.position - transform.position).magnitude > 11f)
                        {
                            // SWITCHING COVER POSITION
                            if (isInteracting)
                            {
                                pointShootRig.weight = 0;
                                weapon.gameObject.SetActive(false);
                                anim.SetBool("GunDrawn", false);
                                // NO NEED FOR ROOT AS CURRENT INTERACTABLE IS OF TYPE INTERACTABLE
                                currentInteractable.transform.root.tag = "Interactable";
                                isInteracting = false;
                                currentInteractable.isOccupied = false;
                            }
                            
                            pointShootRig.weight = 0;
                            crouch = false;
                            targetIsInteractable = false;
                            cachedTransform = null;
                            characterState = CharacterState.Exploration;
                            aiBehaviour = InternalBehaviour.Chase;
                            
                            transform.LookAt(new Vector3(aiTarget.transform.position.x, transform.position.y, aiTarget.transform.position.z));
                        }
                
                break;

        }
    }

    private bool ChooseLocation()
    {
        characterState = CharacterState.Exploration;
        crouch = false;


        // IF THERE IS AT LEAST 1 LOCATION TO GO TO
        if (gunplayLocations.Count > 0)
        {
            int location = Random.Range(0, gunplayLocations.Count);
            do
            {
                location = Random.Range(0, gunplayLocations.Count);
                targetTransform = gunplayLocations[location].targetLocation.transform;
                currentInteractable = gunplayLocations[location];
                targetIsInteractable = true;

            } while (gunplayLocations[location] != currentInteractable);
            characterMovement.run = true;
            return true;
        }
        else
        {
            characterMovement.run = false;
            aiBehaviour = InternalBehaviour.Chase;
            cachedTransform = null;
            currentInteractable = null;
            target = transform.position;
            agent.SetDestination(target);
            targetIsInteractable = false;
            return false;
        }
    }

    protected override void InteractableTypeBehaviour()
    {
        isInteracting = true;
        base.InteractableTypeBehaviour();
    }

    // protected void OnTriggerEnter(Collider other)
    // {
    //     switch (other.tag)
    //     {
    //         case "Interactable":
    //             
    //             Interactable tempInteractable = other.GetComponent<Interactable>();
    //             if (tempInteractable.typeOfInteractable == Interactable.TypeOfInteractable.Cover)
    //             {
    //                 gunplayLocations.Add(tempInteractable);
    //             }
    //
    //             break;
    //     }
    // }

    protected override void OnTriggerStay(Collider other)
    {
        base.OnTriggerStay(other);
        
        switch (other.tag)
        {
            case "Interactable":

                Interactable tempInteractable = other.GetComponent<Interactable>();
                if (tempInteractable.typeOfInteractable == Interactable.TypeOfInteractable.Cover)
                {
                    bool a = true;
                    for (int i = 0; i < gunplayLocations.Count; i++)
                    {
                        if (gunplayLocations[i] == tempInteractable)
                        {
                            a = false;
                        }                    
                    }

                    if (a && tempInteractable != currentInteractable && !tempInteractable.isOccupied)
                    {
                        print($"{gameObject.name} ADDING {tempInteractable}");
                        gunplayLocations.Add(tempInteractable);
                    }
                }
    
                break;
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        
        switch (other.tag)
        {
            case "Interactable":
                
                Interactable tempInteractable = other.GetComponent<Interactable>();
                if (tempInteractable.typeOfInteractable == Interactable.TypeOfInteractable.Cover)
                {
                    print($"{gameObject.name} REMOVING {tempInteractable}");
                    gunplayLocations.Remove(tempInteractable);
                }
    
                break;
        }
    }

    // VISUALIZE THE AI DETECTION SYSTEM
    protected override void OnDrawGizmos()
    {
        // Gizmos.color = Color.green;
        // Gizmos.DrawWireSphere(transform.position, detectionRadius);
        // Gizmos.color = Color.cyan;
        // Gizmos.DrawLine(player.transform.position, aiRayLoc.transform.position);

        // VERTICAL DETECTION
        float playerBasePos = player.transform.position.y - 1f;
        for (int i = 0; i < 5; i++)
        {
            Vector3 dir = new Vector3(player.transform.position.x, playerBasePos + 0.5f * i, player.transform.position.z) - (aiRayLoc.transform.position + new Vector3(0, 0.2f, 0) + transform.forward * 0.25f);
            
            if (Vector3.Dot(transform.forward.normalized, new Vector3(dir.x, 0, dir.z).normalized) > 0.8)
            {
                Ray ray = new Ray(aiRayLoc.transform.position + new Vector3(0, 0.2f, 0) + transform.forward * 0.25f, dir);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayer))
                {
                        // print(hit.collider.transform.root.name + " " + hit.collider.name);

                    if (hit.collider.transform.root.CompareTag("Player"))
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawLine(
                            new Vector3(player.transform.position.x, playerBasePos + 0.5f * i, player.transform.position.z), aiRayLoc.transform.position + new Vector3(0, 0.2f, 0) + transform.forward * 0.25f);
                        continue;
                    }
                    else
                    {
                    }
                }
                
            }
                Gizmos.color = Color.red;
                Gizmos.DrawLine(new Vector3(player.transform.position.x, playerBasePos + 0.5f * i, player.transform.position.z), aiRayLoc.transform.position + new Vector3(0, 0.2f, 0) + transform.forward * 0.25f);
        }
        
        // HORIZONTAL DETECTION
        float totalFOV = 70.0f;
        float rayRange = 20.0f;
        float halfFOV = totalFOV / 2.0f;
        
        for (int i = 0; i <= halfFOV / 5; i++)
        {
            Gizmos.color = Color.magenta;
            
            Quaternion rayRot = Quaternion.AngleAxis( -halfFOV + 10 * i, Vector3.up );
            Vector3 rayDir = rayRot * transform.forward;
        
            Ray ray = new Ray(aiRayLoc.transform.position + new Vector3(0, 0.2f, 0) + transform.forward * 0.25f, rayDir * rayRange);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayer))
            {
                
                if (hit.collider.transform.root.CompareTag("Player"))
                {
                    // print(hit.collider.transform.root.name + " " + hit.collider.name);
                    Gizmos.color = Color.green;
                    Gizmos.DrawRay( aiRayLoc.transform.position + new Vector3(0, 0.2f, 0) + transform.forward * 0.25f, rayDir * rayRange );
                    continue;
                }
                else
                {
                }
            }
            
            Gizmos.color = Color.red;
            Gizmos.DrawRay( aiRayLoc.transform.position + new Vector3(0, 0.2f, 0) + transform.forward * 0.25f, rayDir * rayRange );
        }
        
        // Quaternion leftRayRotation = Quaternion.AngleAxis( -halfFOV, Vector3.up );
        // Quaternion rightRayRotation = Quaternion.AngleAxis( halfFOV, Vector3.up );
        // Vector3 leftRayDirection = leftRayRotation * transform.forward;
        // Vector3 rightRayDirection = rightRayRotation * transform.forward;
        //
        // Gizmos.DrawRay( transform.position, leftRayDirection * rayRange );
        // Gizmos.DrawRay( transform.position, rightRayDirection * rayRange );
    }
}
