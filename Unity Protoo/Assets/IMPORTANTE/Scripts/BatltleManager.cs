using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    [Header("Personajes")]
    public Character player;
    public Character enemy;

    [Header("UI")]
    public Slider playerHealthSlider;
    public Slider enemyHealthSlider;

    private bool playerTurn = true;

    void Start()
    {
        playerHealthSlider.maxValue = player.maxHealth;
        playerHealthSlider.value = player.currentHealth;

        enemyHealthSlider.maxValue = enemy.maxHealth;
        enemyHealthSlider.value = enemy.currentHealth;
    }

    public void OnPlayerAttack(int attackIndex)
    {
        if (!playerTurn) return;

        Attack chosenAttack = player.attacks[attackIndex];

        enemy.TakeDamage(chosenAttack.damage);
        enemyHealthSlider.value = enemy.currentHealth;

        Debug.Log(player.characterName + " usa " + chosenAttack.name);
        Debug.Log(enemy.characterName + " tiene " + enemy.currentHealth + " de vida");

        if (enemy.IsDead())
        {
            Debug.Log("¡Ganaste!");
            return;
        }

        playerTurn = false;

        StartCoroutine(EnemyTurnCoroutine());
    }

    IEnumerator EnemyTurnCoroutine()
    {
        yield return new WaitForSeconds(1f);

        int attackIndex = Random.Range(0, enemy.attacks.Length);
        Attack chosenAttack = enemy.attacks[attackIndex];

        player.TakeDamage(chosenAttack.damage);
        playerHealthSlider.value = player.currentHealth;

        Debug.Log(enemy.characterName + " usa " + chosenAttack.name);
        Debug.Log(player.characterName + " tiene " + player.currentHealth + " de vida");

        if (player.IsDead())
        {
            Debug.Log("Perdiste...");
            yield break;
        }

        playerTurn = true;
        Debug.Log("Es tu turno.");
    }
}
