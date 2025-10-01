using UnityEngine;

public class Character : MonoBehaviour
{
    public string characterName;
    public int maxHealth = 100;
    public int currentHealth;

    public Attack[] attacks; // array de ataques

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }
}
