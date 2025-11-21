using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    [Header("Personajes")]
    public Character player;

    [Header("UI")]
    public Slider playerHealthSlider;
    public Slider enemyHealthSlider;
    public Button[] attackButtons;

    [Header("Spawner")]
    public WaveSpawnerSequential waveSpawner;

    private bool playerTurn = true;

    void Start()
    {
        if (waveSpawner != null)
        {
            waveSpawner.OnWaveFinished += HandleWaveFinished;
        }

        if (playerHealthSlider != null)
        {
            playerHealthSlider.maxValue = player.maxHealth;
            playerHealthSlider.value = player.currentHealth;
        }

        if (enemyHealthSlider != null && waveSpawner != null)
        {
            Character enemy = waveSpawner.GetCurrentEnemy();
            if (enemy != null)
            {
                enemyHealthSlider.maxValue = enemy.maxHealth;
                enemyHealthSlider.value = enemy.currentHealth;
            }
        }

        InitializeAttackButtons();
        UpdateAttackButtons();
    }

    private void InitializeAttackButtons()
    {
        int minLength = Mathf.Min(attackButtons.Length, player.attacks.Length);

        for (int i = 0; i < minLength; i++)
        {
            Button btn = attackButtons[i];

            if (btn == null)
            {
                Debug.LogWarning($"⚠️ Botón {i} no asignado");
                continue;
            }

            btn.onClick.RemoveAllListeners();

            int capturedIndex = i;
            btn.onClick.AddListener(() => OnPlayerAttack(capturedIndex));
        }
    }

    public void OnPlayerAttack(int attackIndex)
    {
        if (!playerTurn) return;

        Character enemy = waveSpawner.GetCurrentEnemy();
        if (enemy == null) return;

        Attack chosenAttack = player.attacks[attackIndex];

        if (chosenAttack.currentCooldown > 0) return;

        enemy.TakeDamage(chosenAttack.damage);

        if (enemyHealthSlider != null)
            enemyHealthSlider.value = enemy.currentHealth;

        if (chosenAttack.cooldownTurns > 0)
            chosenAttack.currentCooldown = chosenAttack.cooldownTurns;

        UpdateAttackButtons();

        playerTurn = false;
        StartCoroutine(EnemyTurnCoroutine());
    }

    IEnumerator EnemyTurnCoroutine()
    {
        yield return new WaitForSeconds(1f);

        Character enemy = waveSpawner.GetCurrentEnemy();
        if (enemy == null)
        {
            playerTurn = true;
            yield break;
        }

        int attackIndex = Random.Range(0, enemy.attacks.Length);
        Attack chosenAttack = enemy.attacks[attackIndex];

        player.TakeDamage(chosenAttack.damage);

        if (playerHealthSlider != null)
            playerHealthSlider.value = player.currentHealth;

        if (player.IsDead())
        {
            Debug.Log("Perdiste...");
            yield break;
        }

        ReduceCooldowns();
        playerTurn = true;
        UpdateAttackButtons();
    }

    private void ReduceCooldowns()
    {
        foreach (Attack atk in player.attacks)
        {
            if (atk.currentCooldown > 0)
                atk.currentCooldown--;
        }
    }

    private void UpdateAttackButtons()
    {
        int minLength = Mathf.Min(attackButtons.Length, player.attacks.Length);

        for (int i = 0; i < minLength; i++)
        {
            Attack atk = player.attacks[i];
            Text btnText = attackButtons[i].GetComponentInChildren<Text>();

            if (atk.currentCooldown > 0)
            {
                attackButtons[i].interactable = false;
                btnText.text = $"{atk.name} ({atk.currentCooldown})";
            }
            else
            {
                attackButtons[i].interactable = true;
                btnText.text = atk.name;
            }
        }
    }

    private void HandleWaveFinished()
    {
        Debug.Log("EVENTO RECIBIDO → cambiando de escena...");

        SceneManager.LoadScene("Victoria");
    }
}
