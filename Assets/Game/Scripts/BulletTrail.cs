using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private GameObject bulletDecalPrefab;

    public float damage;
    public float speed;
    public CharacterControl owner;

    [SerializeField] private List<GameObject> reusableBulletDecals;

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
        else
        {
            if (!DecalsLeft())
            {
                GameObject temp = Instantiate(bulletDecalPrefab, collision.contacts[0].point, Quaternion.identity);
                reusableBulletDecals.Add(temp);
                temp.transform.forward = -collision.contacts[0].normal;
                temp.SetActive(true);
            }
            else
            {
                foreach (GameObject temp in reusableBulletDecals)
                {
                    if (!temp.activeSelf)
                    {
                        temp.transform.position = collision.contacts[0].point;
                        temp.transform.forward = -collision.contacts[0].normal;
                        temp.SetActive(true);
                        break;
                    }
                }
            }
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
    
    private bool DecalsLeft()
    {
        foreach (GameObject a in reusableBulletDecals)
        {
            if (!a.activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    private void OnDisable()
    {
        rb.velocity = Vector3.zero;
    }
}
