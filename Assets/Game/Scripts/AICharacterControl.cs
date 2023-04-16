using System;
using System.Collections;
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
    [SerializeField] private GameObject aiLookTarget;
    [SerializeField] private GameObject aiShootTarget;
    [SerializeField] private GameObject[] bonesHit;
    [SerializeField] private Vector3[] hitLocs;
    [SerializeField] private List<Interactable> gunplayLocations;
    [SerializeField] private AICharacterControl frienlyAiCharacterControl;
    [SerializeField] private List<CharacterControl> friends;
    [SerializeField] private List<CharacterControl> enemies;
    
    [Serializable]
    public class KnownTargetData
    {
        public GameObject obj;
        public CharacterControl character;
        public GameObject[] bones = new GameObject[5];
        public Vector3[] hitLocs;
        public int bonesVisible;
        public bool isKnown;
        public HealthManager healthManager;
        public float health;
        public float time;
        public float distance;
    }

    [SerializeField] public List<KnownTargetData> knownTargetData;
    [SerializeField] private KnownTargetData currentTarget;
    
    // [SerializeField] private Collider[] collidersNearby;
    // [SerializeField] private Collider[] collidersNearby;
    // [SerializeField] private List<Vector3> targetLocations;

    [SerializeField] private bool chooseLocation;
    
    private Vector3 dir;
    // public bool targetKnown;
    public float time;


    public LayerMask hitLayer;
    
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
                
                if (currentTarget.obj)
                {
                    transform.LookAt(new Vector3(currentTarget.obj.transform.position.x, transform.position.y, currentTarget.obj.transform.position.z));
                }
                else
                {
                    transform.LookAt(new Vector3(aiLookTarget.transform.position.x, transform.position.y, aiLookTarget.transform.position.z));
                }

                // TEMPORARY SHOOTING
                StartCoroutine(ShootDelay());
                
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
                        weapon.ShootTarget(aiLookTarget);
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

    // TEST IMPLEMENT
    IEnumerator ShootDelay()
    {
        float shootDelay = Random.Range(0.2f, 3.0f);
        yield return new WaitForSeconds(shootDelay);

        if (weapon.isHandHeld && !weapon.onCooldown && currentTarget.bonesVisible > 1)
        {
            // SELECT WHERE TO SHOOT BEFORE SHOOTING
            int randomBone = Random.Range(0, currentTarget.bones.Length);
            if (currentTarget.bonesVisible > 0)
            {
                crouch = false;

                //CALCULATE NEW ROTATION BASED ON PAYERS DRUNK STATE
                // bulletTargetPointer.transform.position = new Vector3(hit.point.x + Random.Range(-3, 3), hit.point.y + Random.Range(-3, 3), hit.point.z + Random.Range(-3, 3));
                                
                anim.SetTrigger("Shoot");
                pointShootRig.weight = 1;
                
                // WARN PLAYER

                if (currentTarget.bones[randomBone] != null)
                {
                    aiShootTarget.transform.position = currentTarget.bones[randomBone].transform.position;
                    weapon.ShootTarget(aiShootTarget);
                }
                else if (currentTarget.hitLocs[randomBone] != Vector3.zero)
                {
                    aiShootTarget.transform.position = currentTarget.hitLocs[randomBone];
                    weapon.ShootTarget(aiShootTarget);
                }
            }
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
        
        
        // finding friends and enemies closeby

        // Collider[] collidersNearby = Physics.OverlapSphere(transform.position, 15, hitLayer);
        // for (int i = 0; i < collidersNearby.Length; i++)
        // {
        //     if (collidersNearby[i].transform.root.gameObject != gameObject &&
        //         (collidersNearby[i].transform.root.CompareTag("Player") ||
        //          collidersNearby[i].transform.root.CompareTag("AI"))) 
        //     {
        //         if (collidersNearby[i].transform.root.GetComponent<CharacterControl>().isFriendly == isFriendly)
        //         {
        //             bool repeatedCheck = true;
        //             for (int j = 0; j < friends.Count; j++)
        //             {
        //                 if (collidersNearby[i].transform.root.gameObject == friends[j])
        //                 {
        //                     repeatedCheck = false;
        //                 }                    
        //             }
        //
        //             if (repeatedCheck)
        //             {
        //                 friends.Add(collidersNearby[i].transform.root.gameObject);
        //             }
        //         }
        //         else
        //         {
        //             bool repeatedCheck = true;
        //             for (int j = 0; j < enemies.Count; j++)
        //             {
        //                 if (collidersNearby[i].transform.root.gameObject == enemies[j])
        //                 {
        //                     repeatedCheck = false;
        //                 }                    
        //             }
        //
        //             if (repeatedCheck)
        //             {
        //                 enemies.Add(collidersNearby[i].transform.root.gameObject);
        //             }
        //         }
        //     }
        // }

        // if (enemies.Count == 0)
        // {
        //     foreach (var VARIABLE in friends)
        //     {
        //         
        //     }
        // }
        
        // VISION CONE DETECTION
        foreach (CharacterControl enemy in enemies)
        {
            float basePos = enemy.transform.position.y + enemy.m_Capsule.height / 2;
            float j = 1.0f;
            if (enemy.crouch)
            {
                if (enemy.coverAnimRig.weight > 0.5f)
                {
                    basePos = enemy.heightConstraint.transform.position.y / 2;
                }
                else
                {
                    j = 2.0f;
                }
            }

            int hits = 0;

            bonesHit = new GameObject[5];
            hitLocs = new Vector3[5];
            for (int i = 0; i < 5; i++)
            {
                Vector3 dir = new Vector3(enemy.transform.position.x, basePos + 0.5f * (i / j), enemy.transform.position.z) - (aiRayLoc.transform.position + new Vector3(0, 0.2f, 0) + transform.forward * 0.25f);

                // if (time > 1f)
                // {
                //     targetKnown = false;
                //     time = 0;
                // }
                
                if (Vector3.Dot(transform.forward.normalized, new Vector3(dir.x, 0, dir.z).normalized) > 0.8)
                {
                    Ray ray = new Ray(aiRayLoc.transform.position + new Vector3(0, 0.2f, 0) + transform.forward * 0.25f, dir);
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayer))
                    {
                        print(hit.collider.transform.root.name + " " + hit.collider.name);
                        if (hit.collider.transform.root.GetComponent<CharacterControl>() == enemy)
                        {
                            hits++;
                            
                            if (currentTarget.character == enemy)
                            {
                                aiLookTarget.transform.position = hit.point;
                            }

                            bonesHit[i] = hit.transform.gameObject;
                            continue;

                            // targetKnown = true;
                            // time = 0;
                            // aiTarget.transform.position = hit.point;
                        }
                        // aiTarget.transform.position = hit.point;
                        // hitLocations[i] = aiTarget;

                        // hitLocations[i].transform.position = hit.point;
                        bonesHit[i] = null;
                        hitLocs[i] = hit.point;
                        continue;
                    }
                    hitLocs[i] = Vector3.zero;
                }
                
            }

            print(hits);
            if (hits > 0)
            {
                bool repeatedCheck = true;
                for (int k= 0; k < knownTargetData.Count; k++)
                {
                    if (enemy == knownTargetData[k].character)
                    {
                        knownTargetData[k].bones = bonesHit;
                        knownTargetData[k].hitLocs = hitLocs;
                        knownTargetData[k].bonesVisible = hits;
                        knownTargetData[k].isKnown = true;
                        knownTargetData[k].time = 0;
                        repeatedCheck = false;
                    }                    
                }
            
                if (repeatedCheck)
                {
                    KnownTargetData targetData = new KnownTargetData
                    {
                        obj = enemy.gameObject,
                        character = enemy,
                        bones = bonesHit,
                        hitLocs = hitLocs,
                        bonesVisible = hits,
                        isKnown = false,
                        healthManager = enemy.GetComponent<HealthManager>(),
                        distance = 0,
                        time = 0
                    };
                    
                    knownTargetData.Add(targetData);
                    
                    // if (hits > 2 && data.healthManager.health > 30)
                    // {
                    //     currentTarget = data;
                    // }
                }
            }
        }
        
        // SYNC CURRENT TARGET WITH ACTUAL HIT TARGET
        // USE HEALTH TO DETERMINE CURRENT TARGET BY TARGETING THE HEALTHIEST
        // SHOOT FASTER WHEN 4 OR MORE BONES ARE VISIBLE
        // CHOOSE A RANDOM HIT POINT TO SHOOT AT WITH HEAD HAVING THE LEAST PROBABILITY

        knownTargetData.Sort((data1, data2) => data2.health.CompareTo(data1.health));
        if (knownTargetData.Count > 0) //&& knownTargetData[0].bonesVisible >= 2)
        {
            print(knownTargetData[0].obj.name);
            currentTarget = knownTargetData[0];
        }
        
        // UPDATE METHOD FOR TARGET DATA
        foreach (KnownTargetData targetData in knownTargetData)
        {
            targetData.time += Time.deltaTime;
            targetData.health = targetData.healthManager.health;

            if (targetData.time > 1)
            {
                targetData.isKnown = false;
                targetData.time = 0;

                if (targetData == currentTarget)
                {
                    currentTarget = new KnownTargetData();
                }
                
                knownTargetData.Remove(targetData);
                break;
            }
        }

        // float playerBasePos = player.GetComponent<CapsuleCollider>().height / 2; // player.transform.position.y - 1f;
        // for (int i = 0; i < 5; i++)
        // {
        //     Vector3 dir = new Vector3(player.transform.position.x, playerBasePos + 0.5f * i, player.transform.position.z) - (aiRayLoc.transform.position + new Vector3(0, 0.2f, 0) + transform.forward * 0.25f);
        //     
        //     if (time > 1f)
        //     {
        //         targetKnown = false;
        //         time = 0;
        //     }
        //     else if (Vector3.Dot(transform.forward.normalized, new Vector3(dir.x, 0, dir.z).normalized) > 0.8)
        //     {
        //         Ray ray = new Ray(aiRayLoc.transform.position + new Vector3(0, 0.2f, 0) + transform.forward * 0.25f, dir);
        //         if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayer))
        //         {
        //             print(hit.collider.transform.root.name + " " + hit.collider.name);
        //             if (hit.transform.root.CompareTag("Player"))
        //             {
        //                 targetKnown = true;
        //                 time = 0;
        //                 aiTarget.transform.position = hit.point;
        //             }
        //             
        //         }
        //         else
        //         {
        //         }
        //     }
        // }
        //
        // if (targetKnown)
        // {
        //     time += Time.deltaTime;
        // }
        
        
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
                ChooseGunPlayLocation();
            }
        }


        switch (aiBehaviour)
        {
            case InternalBehaviour.Chase:

                if (currentTarget.isKnown)
                {
                    targetTransform = currentTarget.obj.transform;
                    // targetLocations.Add(target);
                    agent.SetDestination(targetTransform.position);

                    if ((currentTarget.obj.transform.position - transform.position).magnitude < 10f)
                    {
                        agent.SetDestination(transform.position);
                        // targetKnown = false;
                        aiBehaviour = InternalBehaviour.Combat;
                        chooseLocation = true;
                    }
                }
                else
                {
                    cachedTransform = null;
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
                
                if (chooseLocation)
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
                        currentInteractable.tag = "Interactable";
                        isInteracting = false;
                        currentInteractable.isOccupied = false;
                    }
                    
                    // CHOOSING COVER LOCATION
                    if (!ChooseGunPlayLocation())
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
                
                if (!currentInteractable && currentTarget.character && (currentTarget.obj.transform.position - transform.position).magnitude > 11f)
                {
                    // SWITCHING COVER POSITION
                    if (isInteracting)
                    {
                        pointShootRig.weight = 0;
                        weapon.gameObject.SetActive(false);
                        anim.SetBool("GunDrawn", false);
                        currentInteractable.tag = "Interactable";
                        isInteracting = false;
                        currentInteractable.isOccupied = false;
                    }
                    
                    pointShootRig.weight = 0;
                    crouch = false;
                    targetIsInteractable = false;
                    cachedTransform = null;
                    characterState = CharacterState.Exploration;
                    aiBehaviour = InternalBehaviour.Chase;
                    
                    transform.LookAt(new Vector3(currentTarget.obj.transform.position.x, transform.position.y, currentTarget.obj.transform.position.z));
                }
                
                break;

        }
    }

    private bool ChooseGunPlayLocation()
    {
        characterState = CharacterState.Exploration;
        crouch = false;
        
        // IF THERE IS AT LEAST 1 LOCATION TO GO TO
        if (gunplayLocations.Count > 0)
        {
            int randomLocation;
            do
            {
                randomLocation = Random.Range(0, gunplayLocations.Count);
                targetTransform = gunplayLocations[randomLocation].targetLocation.transform;
                currentInteractable = gunplayLocations[randomLocation];
                targetIsInteractable = true;
        
            } while (gunplayLocations[randomLocation] != currentInteractable);
            characterMovement.run = true;
            return true;
        }

        // IF THERE ARENT ANY GUNPLAY LOCATIONS FOUND, CHOOSE A FRIEND AND FIND A GUNPLAY LOCATION
        // IF THERE ARE NO VALID LOCATIONS, CHOOSE ANOTHER FRIEND
        // REPEAT THE PROCESS AS MANY TIMES AS THE NUMBER OF FRIENDS WHICH WILL INCREASE THE PROBABILITY OF CHOOSING A TARGET OVER CHASING
        int tryNum = 0;
        while (friends.Count > 0 && tryNum < friends.Count)
        {
            tryNum++;
            int randomFriend = Random.Range(0, friends.Count);
            if (friends[randomFriend].CompareTag("AI"))
            {
                print($"TRY NUM:{tryNum}");
                frienlyAiCharacterControl = friends[randomFriend].GetComponent<AICharacterControl>();
                if (frienlyAiCharacterControl.gunplayLocations.Count > 0)
                {
                    int randomLocation;
                    do
                    {
                        randomLocation = Random.Range(0, frienlyAiCharacterControl.gunplayLocations.Count);
                        targetTransform = frienlyAiCharacterControl.gunplayLocations[randomLocation].targetLocation.transform;
                        currentInteractable = frienlyAiCharacterControl.gunplayLocations[randomLocation];
                        targetIsInteractable = true;
                        print($"{gameObject.name} TRY:{tryNum} FRIEND:{friends[randomFriend].gameObject.name} LOCATION:{frienlyAiCharacterControl.gunplayLocations[randomLocation].name}");
        
                    } while (frienlyAiCharacterControl.gunplayLocations[randomLocation] != currentInteractable);
                    characterMovement.run = true;
                    frienlyAiCharacterControl = null;
                    return true;
                }
            }
        }
        
        Debug.LogWarning($"{gameObject.name}: NO LOCATIONS FOUND");
        characterMovement.run = false;
        aiBehaviour = InternalBehaviour.Chase;
        cachedTransform = null;
        currentInteractable = null;
        target = transform.position;
        agent.SetDestination(target);
        targetIsInteractable = false;
        return false;
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
        
        // switch (other.tag)
        // {
        //     case "Interactable":
        //
        //         Interactable tempInteractable = other.GetComponent<Interactable>();
        //         if (tempInteractable.typeOfInteractable == Interactable.TypeOfInteractable.Cover)
        //         {
        //             bool repeatedCheck = true;
        //             for (int i = 0; i < gunplayLocations.Count; i++)
        //             {
        //                 if (gunplayLocations[i] == tempInteractable)
        //                 {
        //                     repeatedCheck = false;
        //                 }                    
        //             }
        //
        //             if (repeatedCheck && tempInteractable != currentInteractable && !tempInteractable.isOccupied)
        //             {
        //                 print($"{gameObject.name} ADDING {tempInteractable}");
        //                 gunplayLocations.Add(tempInteractable);
        //             }
        //         }
        //
        //         break;
        // }

        if (other.CompareTag("Interactable"))
        {
            Interactable tempInteractable = other.GetComponent<Interactable>();
            if (tempInteractable.typeOfInteractable == Interactable.TypeOfInteractable.Cover)
            {
                bool repeatedCheck = true;
                for (int i = 0; i < gunplayLocations.Count; i++)
                {
                    if (gunplayLocations[i] == tempInteractable)
                    {
                        repeatedCheck = false;
                    }                    
                }

                if (repeatedCheck && tempInteractable != currentInteractable && !tempInteractable.isOccupied)
                {
                    print($"{gameObject.name} ADDING {tempInteractable}");
                    gunplayLocations.Add(tempInteractable);
                }
            }
        }
        
        if (other.transform.root.gameObject != gameObject &&
            (other.transform.root.CompareTag("Player") ||
             other.transform.root.CompareTag("AI")))
        {
            CharacterControl characterControl = other.transform.root.GetComponent<CharacterControl>();
            if (characterControl.isFriendly == isFriendly)
            {
                bool repeatedCheck = true;
                for (int j = 0; j < friends.Count; j++)
                {
                    if (characterControl == friends[j])
                    {
                        repeatedCheck = false;
                    }                    
                }

                if (repeatedCheck)
                {
                    friends.Add(characterControl);
                }
            }
            else
            {
                bool repeatedCheck = true;
                for (int j = 0; j < enemies.Count; j++)
                {
                    if (characterControl == enemies[j])
                    {
                        repeatedCheck = false;
                    }                    
                }

                if (repeatedCheck)
                {
                    enemies.Add(characterControl);
                }
            }
            
            if (characterControl.characterState == CharacterState.None)
            {
                friends.Remove(characterControl);
                enemies.Remove(characterControl);
            }
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
        
        if (other.transform.root.gameObject != gameObject &&
            (other.transform.root.CompareTag("Player") ||
             other.transform.root.CompareTag("AI"))) 
        {
            CharacterControl characterControl = other.transform.root.GetComponent<CharacterControl>();
            if (characterControl.isFriendly == isFriendly)
            {
                friends.Remove(characterControl);
            }
            else
            {
                enemies.Remove(characterControl);
            }
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
        foreach (CharacterControl enemy in enemies)
        {
            float basePos = enemy.transform.position.y + enemy.m_Capsule.height / 2;
            float j = 1.0f;
            if (enemy.crouch)
            {
                if (enemy.coverAnimRig.weight > 0.5f)
                {
                    basePos = enemy.heightConstraint.transform.position.y / 2;
                }
                else
                {
                    j = 2.0f;
                }
            }
            
            for (int i = 0; i < 5; i++)
            {
                Vector3 dir = new Vector3(enemy.transform.position.x, basePos + 0.5f * (i / j), enemy.transform.position.z) - (aiRayLoc.transform.position + new Vector3(0, 0.2f, 0) + transform.forward * 0.25f);
                
                if (Vector3.Dot(transform.forward.normalized, new Vector3(dir.x, 0, dir.z).normalized) > 0.8)
                {
                    Ray ray = new Ray(aiRayLoc.transform.position + new Vector3(0, 0.2f, 0) + transform.forward * 0.25f, dir);
                    // Gizmos.DrawRay(ray);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayer))
                    {
                        // print(hit.collider.transform.root.name + " " + hit.collider.name);
                        
                        if (hit.collider.transform.root.GetComponent<CharacterControl>() == enemy)
                        {
                            Gizmos.color = Color.green;
                            Gizmos.DrawLine(
                                new Vector3(enemy.transform.position.x, basePos + 0.5f * i / j, enemy.transform.position.z), aiRayLoc.transform.position + new Vector3(0, 0.2f, 0) + transform.forward * 0.25f);
                            continue;
                        }
                        else
                        {
                        }
                    }
                    
                }
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(new Vector3(enemy.transform.position.x, basePos + 0.5f * i / j, enemy.transform.position.z), aiRayLoc.transform.position + new Vector3(0, 0.2f, 0) + transform.forward * 0.25f);
            }
        }
        
        // HORIZONTAL DETECTION
        // float totalFOV = 70.0f;
        // float rayRange = 20.0f;
        // float halfFOV = totalFOV / 2.0f;
        //
        // for (int i = 0; i <= halfFOV / 5; i++)
        // {
        //     Gizmos.color = Color.magenta;
        //     
        //     Quaternion rayRot = Quaternion.AngleAxis( -halfFOV + 10 * i, Vector3.up );
        //     Vector3 rayDir = rayRot * transform.forward;
        //
        //     Ray ray = new Ray(aiRayLoc.transform.position + new Vector3(0, 0.2f, 0) + transform.forward * 0.25f, rayDir * rayRange);
        //     RaycastHit hit;
        //     if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayer))
        //     {
        //         
        //         if (hit.collider.transform.root.CompareTag("Player"))
        //         {
        //             // print(hit.collider.transform.root.name + " " + hit.collider.name);
        //             Gizmos.color = Color.green;
        //             Gizmos.DrawRay( aiRayLoc.transform.position + new Vector3(0, 0.2f, 0) + transform.forward * 0.25f, rayDir * rayRange );
        //             continue;
        //         }
        //         else
        //         {
        //         }
        //     }
        //     
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawRay( aiRayLoc.transform.position + new Vector3(0, 0.2f, 0) + transform.forward * 0.25f, rayDir * rayRange );
        // }
        
        // Quaternion leftRayRotation = Quaternion.AngleAxis( -halfFOV, Vector3.up );
        // Quaternion rightRayRotation = Quaternion.AngleAxis( halfFOV, Vector3.up );
        // Vector3 leftRayDirection = leftRayRotation * transform.forward;
        // Vector3 rightRayDirection = rightRayRotation * transform.forward;
        //
        // Gizmos.DrawRay( transform.position, leftRayDirection * rayRange );
        // Gizmos.DrawRay( transform.position, rightRayDirection * rayRange );
    }
}
