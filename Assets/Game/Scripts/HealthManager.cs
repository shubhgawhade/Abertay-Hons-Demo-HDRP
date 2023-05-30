using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private DamageUI damageUI;
    
    public float health;
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
        damageUI.damageText.text = subtractHealth.ToString();
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
