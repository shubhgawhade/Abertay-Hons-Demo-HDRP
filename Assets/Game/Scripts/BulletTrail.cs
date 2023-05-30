using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    public float damage;
    public float speed;
    public CharacterControl owner;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.AddForce(speed * Time.deltaTime * transform.forward, ForceMode.Force);
        // transform.position += speed * Time.deltaTime * transform.forward;
        // transform.Translate(speed * Time.deltaTime * transform.forward);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Destroy(gameObject);
        
        // print($"TRIGGER: {other.name}");
        // HitPhysics(other);
    }

    private void Update()
    {
        // if (owner.characterState == CharacterControl.CharacterState.None)
        // {
        //     Destroy(gameObject);
        // }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.transform.root.CompareTag("Player") || collision.collider.transform.root.CompareTag("AI"))
        {
            print(owner.gameObject.name + " HIT " + collision.collider.transform.root.gameObject.name);
        }
        
        // Destroy(gameObject);
        
        print($"COLLIDER: {collision.collider.name}");
        HitPhysics(collision.collider);
        gameObject.SetActive(false);
    }

    private void HitPhysics(Collider hitCollider)
    {
        if (hitCollider.transform.root.CompareTag("Player") || hitCollider.transform.root.CompareTag("AI"))
        {
            HealthManager hitCharacterHealthManger = hitCollider.transform.root.GetComponent<HealthManager>();
            Rigidbody hitCharacterRigidbody = hitCollider.GetComponent<Rigidbody>();
            
            // print(hitCharacterHealthManger.health - damage);
            if (owner.isFriendly != hitCollider.transform.root.GetComponent<CharacterControl>().isFriendly)
            {
                if (hitCharacterHealthManger.health - damage <= 0)
                {
                    hitCharacterRigidbody.useGravity = true;
                    hitCharacterRigidbody.isKinematic = false;
                    hitCharacterRigidbody.AddForce(rb.velocity * 3, ForceMode.Impulse);
                }
                else
                {
                    hitCharacterRigidbody.velocity = Vector3.zero;
                }
                
                hitCharacterHealthManger.SubtractHealth(damage);
            }
        }
    }

    private void OnDisable()
    {
        rb.velocity = Vector3.zero;
    }
}
