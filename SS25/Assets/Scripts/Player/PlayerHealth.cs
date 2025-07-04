using UnityEngine;

public class PlayerHealth : MonoBehaviour
{

    public int MaxHealth = 100;
    public int CurrentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentHealth = MaxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        Debug.Log("Player takes " + damage + " damage! Current health: " + CurrentHealth);

        if (CurrentHealth <= 5)
        {
                       Debug.Log("Players heals 50 health");
            CurrentHealth += 50;
        }
        if (CurrentHealth <= 0)
        {
        
        }
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
            Debug.Log("Player has died!");
            // Here you can add logic for player death, like respawning or ending the game
        }
    }
}

