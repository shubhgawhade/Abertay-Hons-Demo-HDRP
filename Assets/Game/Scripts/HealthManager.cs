using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private DamageUI damageUI;
    [SerializeField] private Slider healthAndDrunkBar;
    [SerializeField] private Slider drunkBar;
    
    public float health;
    public float drunk;
    private GameObject currentDamageUI;
    private CharacterControl characterControl;
    public List<GameObject> reusabledamageUI;


    private void Awake()
    {
        characterControl = GetComponent<CharacterControl>();
    }

    private void UpdatePlayerHealth()
    {
        if (gameObject.CompareTag("Player"))
        {
            GameManager.PlayerHealth = health;
        }
    }

    private void Update()
    {
        if (characterControl.CompareTag("Player"))
        {
            // HEALTH BAR VALUES
            health = Mathf.Clamp(health, 0, 100);
            Mathf.Clamp(GameManager.Drunkenness, 0, 50);
            // print(GameManager.Drunkenness);
            healthAndDrunkBar.value = Mathf.Lerp(healthAndDrunkBar.value, health, 10 * Time.deltaTime);
            
            // DRUNK BAR VALUES
            drunk = Mathf.Clamp(GameManager.Drunkenness, 0, 50);
            if (characterControl.characterState != CharacterControl.CharacterState.None)
            {
                GameManager.Drunkenness -= 0.2f * Time.deltaTime;
            }
            drunkBar.value = Mathf.Lerp(drunkBar.value, GameManager.Drunkenness, 10 * Time.deltaTime);
        }
    }

    public void AddHealth(float addHealth)
    {
        health += addHealth;
        UpdatePlayerHealth();
    }
    
    public void SubtractHealth(float subtractHealth)
    {
        if (health - subtractHealth > 0)
        {
            health -= subtractHealth;
        }
        else
        {
            health = 0;
            characterControl.characterState = CharacterControl.CharacterState.Dead;
        }
        UpdatePlayerHealth();

        if (!DamageUILLeft())
        {
            currentDamageUI = Instantiate(damageUI.gameObject, transform.position, quaternion.identity);
            reusabledamageUI.Add(currentDamageUI);
        }
        else
        {
            foreach (GameObject temp in reusabledamageUI)
            {
                if (!temp.activeSelf)
                {
                    currentDamageUI = temp;
                    currentDamageUI.transform.position = transform.position;
                    currentDamageUI.SetActive(true);
                    break;
                }
            }
        }
        
        if (characterControl.isFriendly)
        {
            if(characterControl.CompareTag("Player"))
            {
                currentDamageUI.GetComponent<DamageUI>().damageText.color = Color.red;
            }
            else
            {
                currentDamageUI.GetComponent<DamageUI>().damageText.color = Color.white;
            }
        }
        else
        {
            currentDamageUI.GetComponent<DamageUI>().damageText.color = Color.yellow;
        }
        
        currentDamageUI.GetComponent<DamageUI>().damageText.text = subtractHealth.ToString();
    }
    
    private bool DamageUILLeft()
    {
        foreach (GameObject a in reusabledamageUI)
        {
            if (!a.activeSelf)
            {
                return true;
            }
        }

        return false;
    }
}
