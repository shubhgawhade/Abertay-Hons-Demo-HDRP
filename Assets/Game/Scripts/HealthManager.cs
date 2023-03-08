using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class HealthManager : MonoBehaviour
{
    public float health;

    private CharacterControl characterControl;

    private void Awake()
    {
        characterControl = GetComponent<CharacterControl>();
    }

    private void UpdateHealth()
    {
        if (gameObject.CompareTag("Player"))
        {
            GameManager.PlayerHealth = health;
        }
    }
    
    public void AddHealth(float addHealth)
    {
        health += addHealth;
        UpdateHealth();
    }
    
    public void SubtractHealth(float subtractHealth)
    {
        if (health - subtractHealth > 0)
        {
            health -= subtractHealth;
            UpdateHealth();
        }
        else
        {
            health = 0;
            characterControl.characterState = CharacterControl.CharacterState.Dead;
        }
    }
    
}
