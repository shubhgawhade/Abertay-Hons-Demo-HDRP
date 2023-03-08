using System;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    public float damage;
    public float speed;
    public bool move;

    public Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (move)
        {
            rb.AddForce(speed * Time.deltaTime * transform.forward, ForceMode.Force);
            // transform.position += speed * Time.deltaTime * transform.forward;
            // transform.Translate(speed * Time.deltaTime * transform.forward);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Destroy(gameObject);
        
        // print($"TRIGGER: {other.name}");
        // HitPhysics(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        
        print($"COLLIDER: {collision.collider.name}");
        HitPhysics(collision.collider);
    }

    public void HitPhysics(Collider other)
    {
        if (other.transform.root.CompareTag("Player") || other.transform.root.CompareTag("AI"))
        {
            // Vector3 a = transform.localToWorldMatrix.MultiplyPoint3x4(transform.forward).normalized;
            // print(transform.forward);
            // other.GetComponent<Rigidbody>().AddForce(new Vector3(a.x, 0, a.z), ForceMode.Force);
            print(GameManager.PlayerHealth - damage);

            if (GameManager.PlayerHealth - damage <= 0)
            {
                other.GetComponent<Rigidbody>().useGravity = true;
                other.GetComponent<Rigidbody>().isKinematic = false;
                other.GetComponent<Rigidbody>().AddForce(rb.velocity * 6, ForceMode.Impulse);
            }
            else
            {
                other.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
            
            other.transform.root.GetComponent<HealthManager>().SubtractHealth(damage);
            // print($"TRIGGER: {other.name}");
        }
    }
}
