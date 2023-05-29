using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private GameObject bulletTrail;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject bulletSpawnLoc;
    [SerializeField] private CharacterControl owner;
    [SerializeField] private Timer vulnerabilityTimer;
    [SerializeField] private Timer fireRateTimer;
    [SerializeField] private float vulnerabilityTime;
    [SerializeField] private float fireRate;
    [SerializeField] private float shootingAnimationDelay = 0.2f;
    [SerializeField] private float damage;
    // [SerializeField] private float speed = 40;
    
    [SerializeField] private List<GameObject> reusableBullets;

    
    public bool isHandHeld;
    public bool onCooldown;
    public bool autoCrouch = true;

    private void Awake()
    {
        vulnerabilityTimer = gameObject.AddComponent<Timer>();
        fireRateTimer = gameObject.AddComponent<Timer>();
        owner = transform.root.GetComponent<CharacterControl>();
    }

    private void Update()
    {
        if (vulnerabilityTimer.isCompleted)
        {
            vulnerabilityTimer.isCompleted = false;
            if (autoCrouch)
            {
                owner.crouch = true;
            }
            // vulnerabilityTimer.time = 0;
        }

        if (fireRateTimer.isCompleted)
        {
            fireRateTimer.isCompleted = false;
            // fireRateTimer.time = 0;
            onCooldown = false;
        }
    }
    
    public void ShootTarget(GameObject bulletTarget)
    {
        vulnerabilityTimer.time = 0;
        vulnerabilityTimer.StartTimer(vulnerabilityTime);
        
        fireRateTimer.time = 0;
        fireRateTimer.StartTimer(fireRate + shootingAnimationDelay);
        onCooldown = true;
        
        StartCoroutine(WaitForShootingAnimation(bulletTarget));
    }

    IEnumerator WaitForShootingAnimation(GameObject bulletTarget)
    {
        yield return new WaitForSeconds(shootingAnimationDelay);
        muzzleFlash.SetActive(true);
        
        if (!BulletsLeft())
        {
            GameObject temp = Instantiate(bulletTrail, bulletSpawnLoc.transform.position, Quaternion.identity);
            reusableBullets.Add(temp);
            temp.GetComponent<BulletTrail>().owner = owner;
            temp.GetComponent<BulletTrail>().damage = damage;
            // temp.GetComponent<BulletTrail>().speed = speed;
            temp.transform.LookAt(bulletTarget.transform.position);
        }
        else
        {
            foreach (GameObject temp in reusableBullets)
            {
                if (!temp.activeSelf)
                {
                    temp.transform.position = bulletSpawnLoc.transform.position;
                    temp.transform.LookAt(bulletTarget.transform.position);
                    temp.SetActive(true);
                    break;
                }
            }
        }
        
    }
    
    private bool BulletsLeft()
    {
        foreach (GameObject a in reusableBullets)
        {
            if (!a.activeSelf)
            {
                return true;
            }
        }

        return false;
    }

    private void OnDestroy()
    {
        Destroy(vulnerabilityTimer);
        Destroy(fireRateTimer);
    }
}
