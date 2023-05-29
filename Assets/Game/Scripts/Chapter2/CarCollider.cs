using System;
using UnityEngine;

public class CarCollider : MonoBehaviour
{
    private Chapter2 chapter2Manager;
    [SerializeField] private int hits;

    private void Start()
    {
        chapter2Manager = GameManager.Chapter2Manager;
    }

    private void OnTriggerEnter(Collider other)
    {
        // ENTER GATE TO FAIL SCREEN
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Bullet"))
        {
            hits++;

            if (hits == 3)
            {
                chapter2Manager.unlockables[1].unlocked = true;
            }
        }
    }
}
