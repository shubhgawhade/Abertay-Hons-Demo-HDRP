using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityStandardAssets.Characters.ThirdPerson;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject bulletTrail;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject bulletSpawnLoc;
    public CharacterControl owner;
    public Timer vulnerabilityTimer;
    public Timer fireRateTimer;
    public float vulnerability;
    public float fireRate;
    public float damage;
    public bool isHandHeld;
    public bool onCooldown;

    private void Awake()
    {
        vulnerabilityTimer = gameObject.AddComponent<Timer>();
        fireRateTimer = gameObject.AddComponent<Timer>();
        owner = transform.root.gameObject.GetComponent<CharacterControl>();
    }

    private void OnEnable()
    {
        
    }

    private void Update()
    {
        if (vulnerabilityTimer.isCompleted)
        {
            vulnerabilityTimer.isCompleted = false;
            owner.crouch = true;
            // vulnerabilityTimer.time = 0;
        }

        if (fireRateTimer.isCompleted)
        {
            fireRateTimer.isCompleted = false;
            // fireRateTimer.time = 0;
            onCooldown = false;
        }
    }
    
    public void ShootTarget(Vector3 bulletTarget)
    {
        vulnerabilityTimer.time = 0;
        vulnerabilityTimer.StartTimer(vulnerability);
        
        fireRateTimer.time = 0;
        fireRateTimer.StartTimer(fireRate + 0.2f);
        onCooldown = true;

        StartCoroutine(WaitForShootingAnimation(bulletTarget));
    }

    IEnumerator WaitForShootingAnimation(Vector3 bulletTarget)
    {
        yield return new WaitForSeconds(0.2f);
        muzzleFlash.SetActive(true);
        GameObject temp = Instantiate(bulletTrail, bulletSpawnLoc.transform.position, Quaternion.identity);
        // temp.GetComponent<BulletTrail>().damage = damage;
        temp.transform.LookAt(bulletTarget);
    }

    private void OnDestroy()
    {
        Destroy(vulnerabilityTimer);
        Destroy(fireRateTimer);
    }
}
