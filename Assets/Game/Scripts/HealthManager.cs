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
    }
    
}
