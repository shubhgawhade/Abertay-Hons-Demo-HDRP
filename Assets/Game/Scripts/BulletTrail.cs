using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;

    public float damage;
    public float speed;

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
        
        print($"TRIGGER: {other.name}");
        // HitPhysics(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        
        print($"COLLIDER: {collision.collider.name}");
        HitPhysics(collision.collider);
    }

    private void HitPhysics(Collider hitCollider)
    {
        if (hitCollider.transform.root.CompareTag("Player") || hitCollider.transform.root.CompareTag("AI"))
        {
            HealthManager hitCharacterHealthManger = hitCollider.transform.root.GetComponent<HealthManager>();
            Rigidbody hitCharacterRigidbody = hitCollider.GetComponent<Rigidbody>();
            
            // print(hitCharacterHealthManger.health - damage);

            if (hitCharacterHealthManger.health - damage <= 0)
            {
                hitCharacterRigidbody.useGravity = true;
                hitCharacterRigidbody.isKinematic = false;
                hitCharacterRigidbody.AddForce(rb.velocity * 6, ForceMode.Impulse);
            }
            else
            {
                hitCharacterRigidbody.velocity = Vector3.zero;
            }
            
            hitCharacterHealthManger.SubtractHealth(damage);
        }
    }
}
