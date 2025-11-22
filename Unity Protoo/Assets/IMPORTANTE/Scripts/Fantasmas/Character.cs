using UnityEngine;

public class Character : MonoBehaviour
{
    public string characterName;
    public int maxHealth = 100;
    public int currentHealth;
    public Attack[] attacks;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
        }
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }

    public void AttackEnemy(Character enemy, Attack attack)
    {
        if (enemy == null || attack == null)
        {
            Debug.LogWarning("AttackEnemy recibió valores nulos.");
            return;
        }

        Debug.Log($"{characterName} usa {attack.attackName} contra {enemy.characterName}");

        enemy.TakeDamage(attack.damage);
    }
}
