using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

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
    
    public string name;
    // [SerializeField] private GameObject characterHead;
    public Transform targetTransform;
    public Transform cachedTransform;
    protected Rigidbody rb;
    public NavMeshAgent agent { get; private set; }             // the navmesh agent required for the path finding
    public CharacterMovement characterMovement { get; private set; } // the character we are controlling
    public Vector3 target;                                    // target to aim for
    
    private bool cooldown;

    public bool isInteracting;
    public bool targetIsInteractable;
    public Interactable currentInteractable;
    public bool aIStopped;

    // Gunplay local variables
    public Weapon weapon;
    public Weapon[] weapons;
    public CoverInteractables.WeaponType weaponType = CoverInteractables.WeaponType.Length;

    protected Animator anim; 
    public Rig coverAnimRig;
    protected Rig pointShootRig;
    private Transform _defaultHeightConstraint;
    public Transform heightConstraint;
    private float coverAnimRigWeight;
    
    public bool crouch;
    public CapsuleCollider m_Capsule;

    [SerializeField] protected GameObject bulletTarget;
    [SerializeField] protected List<CharacterControl> friends;
    [SerializeField] protected List<CharacterControl> enemies;
    public bool isFriendly;
    [SerializeField] protected CinemachineTargetGroup cinemachineTarget;

    
    protected void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        m_Capsule = GetComponent<CapsuleCollider>();
        
        if (GetComponent<RigBuilder>())
        {
            coverAnimRig = GetComponent<RigBuilder>().layers[0].rig;
            pointShootRig = GetComponent<RigBuilder>().layers[1].rig;
            heightConstraint = coverAnimRig.gameObject.GetComponent<BlendConstraint>().data.sourceObjectB;
            _defaultHeightConstraint = heightConstraint;
        }
        

        // get the components on the object we need ( should not be null due to require component so no need to check )
        agent = GetComponent<NavMeshAgent>();
        characterMovement = GetComponent<CharacterMovement>();

        // ShootAt += ShootTarget;
        agent.updateRotation = false;
        agent.updatePosition = true;
    }

    protected virtual bool ToggleCrouch()
    {
        return crouch;
    }

    protected virtual void Update()
    {
        if (crouch)
        {
            if (pointShootRig)
            {
                pointShootRig.weight = 0;
            }
        }
        else
        {
            if (coverAnimRig)
            {
                heightConstraint = _defaultHeightConstraint;
                coverAnimRigWeight = 0;
            }
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
        
        
        
        // ----- RE-FORMAT
        // print("DISTANCE LEFT" + (target - transform.position).magnitude);
        if ((target - transform.position).magnitude > 0.2f)
        {
            // IF THE INTERACTABLE IS OCCUPIED BEFORE THE CHARACTER REACHES IT
            // if (targetIsInteractable && currentInteractable.isOccupied && currentInteractable.typeOfInteractable == Interactable.TypeOfInteractable.Cover)
            // {
            //     cachedTransform = null;
            //     currentInteractable = null;
            //     target = transform.position;
            //     agent.SetDestination(target);
            //     targetIsInteractable = false;
            //     // return;
            // }
            
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
                            
                        transform.rotation = Quaternion.Slerp(transform.rotation.normalized, cachedTransform.rotation.normalized, Time.deltaTime * 5);

                        // print(Vector3.Dot(transform.forward.normalized, cachedTransform.transform.forward.normalized));
                        if (Vector3.Dot(transform.forward.normalized, cachedTransform.transform.forward.normalized) > 0.99f)
                        {
                            cachedTransform = null;
                        }
                        
                        // transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, cachedTransform.eulerAngles, Time.deltaTime * 5);
                    }

                    // WHEN THE PLAYER STOPS AT THE INTERACTABES TARGET LOCATION
                    if (targetIsInteractable && !currentInteractable.isOccupied)
                    {
                        InteractableTypeBehaviour();
                        // currentInteractable = null;
                    }
                }
            }
        }
        // ----- RE FORMAT
        
        

        // WORLD FUNCTIONS BASED ON THE CHARACTER STATE
        switch (characterState)
        {
            case CharacterState.Cutscene: 
                
                // characterMovement.Move(agent.desiredVelocity, ToggleCrouch(), false);
                
                break;
            
            
            case CharacterState.Exploration:

                // LERPING BETWEEN DIFFERENT COVER HEIGHTS
                if (coverAnimRig)
                {
                    InterpolateCoverRigWeight();
                }
                
                break;
            
            
            case CharacterState.Gunplay:

                // LERPING BETWEEN DIFFERENT COVER HEIGHTS
                if (coverAnimRig)
                {
                    InterpolateCoverRigWeight();
                }

                break;

            case CharacterState.Dead:

                if (currentInteractable)
                {
                    currentInteractable.tag = "Interactable";
                    currentInteractable.isOccupied = false;
                }

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
                // m_Capsule.enabled = false;
                characterState = CharacterState.None;
                
                break;
            
            default:
                
                cinemachineTarget.RemoveMember(gameObject.transform);
                Debug.LogWarning($"{gameObject.name} NOT SET TO ANY CHARACTER STATE");
                StopAllCoroutines();
                // Time.timeScale = 0.5f;
                // enabled = false;
                
                break;
        }
    }
    
    protected virtual void InteractableTypeBehaviour()
    {
        currentInteractable.isOccupied = true;
        targetIsInteractable = false;
            
        switch (currentInteractable.typeOfInteractable)
        {
            case Interactable.TypeOfInteractable.Cover:
                
                characterState = CharacterState.Gunplay;
                currentInteractable.tag = "Cover";

                //COMAPRE THE TYPE OF WEAPON AND ENABLE THAT WEAPON ON THIS PLAYER
                // weapon = currentInteractable.GetComponent<CoverInteractables>().weapon;
                // currentInteractable.character = characterHead;
                weaponType = currentInteractable.GetComponent<CoverInteractables>().weaponType;
                
                for (int i = 0; i < (int)CoverInteractables.WeaponType.Length; i++)
                {
                    if (i == (int)weaponType)
                    {
                        weapon = weapons[i];
                        if (weapons[i].isHandHeld)
                        {
                            weapons[i].gameObject.SetActive(true);
                        }
                    }
                }
                
                // switch (weaponType)
                // {
                //     case CoverInteractables.WeaponType.Gun:
                //
                //         
                //         break;
                //     
                //     case CoverInteractables.WeaponType.BigGun:
                //
                //         weapon = weapons[1];
                //         if (weapons[1].isHandHeld)
                //         {
                //             weapons[1].gameObject.SetActive(true);
                //         }
                //
                //         break;
                // }
                
                anim.SetBool("GunDrawn", true);
                
                crouch = true;
                
                break;
        }
    }

    private void InterpolateCoverRigWeight()
    {
        coverAnimRig.weight = Mathf.Lerp(coverAnimRig.weight, coverAnimRigWeight, 0.2f);
        coverAnimRig.weight = Mathf.Clamp(coverAnimRig.weight, 0, 1);
    }

    public void ChangeCharacterState(CharacterState characterState)
    {
        this.characterState = characterState;
    }

    protected virtual void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            case "Cover":
                
                if (crouch && isInteracting)
                {
                    // TO HELP THE FRIENDLY AI TO BE A BIT BETTER THAN THE ENEMY AI SO THAT THEY DONT BECOME BAD TEAM MATES
                    if (isFriendly && GetComponent<HealthManager>().health < 30)
                    {
                        GetComponent<HealthManager>().AddHealth(Time.deltaTime);
                    }

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

    protected virtual void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "Cover":
                heightConstraint.position = new Vector3(heightConstraint.position.x, _defaultHeightConstraint.position.y, heightConstraint.position.z);
                coverAnimRigWeight = 0;
                _defaultHeightConstraint = heightConstraint;
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

    protected virtual void OnDrawGizmos()
    {
        if (bulletTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(bulletTarget.transform.position, 0.2f);
        } 
    }
}