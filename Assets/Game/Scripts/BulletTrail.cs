using System;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    public float speed;
    public bool move;

    // Update is called once per frame
    void Update()
    {
        if (move)
        {
            transform.position += speed * Time.deltaTime * transform.forward;
            // transform.Translate(speed * Time.deltaTime * transform.forward);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.collider.name);
    }
}
