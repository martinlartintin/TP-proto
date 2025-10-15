using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    [Header("Personajes")]
    public Character player;

    [Header("UI")]
    public Slider playerHealthSlider;
    public Slider enemyHealthSlider;

    [Header("Spawner")]
    public WaveSpawnerSequential waveSpawner; // Asignar en Inspector

    private bool playerTurn = true;

    void Start()
    {
        playerHealthSlider.maxValue = player.maxHealth;
        playerHealthSlider.value = player.currentHealth;
    }

    public void OnPlayerAttack(int attackIndex)
    {
        if (!playerTurn) return;

        Character enemy = waveSpawner.GetCurrentEnemy();
        if (enemy == null) return; // No hay enemigo vivo

        Attack chosenAttack = player.attacks[attackIndex];
        enemy.TakeDamage(chosenAttack.damage);

        if (enemyHealthSlider != null)
            enemyHealthSlider.value = enemy.currentHealth;

        Debug.Log(player.characterName + " usa " + chosenAttack.name);
        Debug.Log(enemy.characterName + " tiene " + enemy.currentHealth + " de vida");

        // Revisar si toda la oleada terminó
        if (waveSpawner.IsWaveFinished())
        {
            Debug.Log("¡Ganaste la oleada!");
            return;
        }

        playerTurn = false;
        StartCoroutine(EnemyTurnCoroutine());
    }

    IEnumerator EnemyTurnCoroutine()
    {
        yield return new WaitForSeconds(1f);

        Character enemy = waveSpawner.GetCurrentEnemy();
        if (enemy == null) // Si el enemigo murió antes de su turno
        {
            playerTurn = true;
            yield break;
        }

        int attackIndex = Random.Range(0, enemy.attacks.Length);
        Attack chosenAttack = enemy.attacks[attackIndex];

        player.TakeDamage(chosenAttack.damage);

        if (playerHealthSlider != null)
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
