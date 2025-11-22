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
    private string[] originalButtonTexts;
    void Start()
    {
        if (waveSpawner != null)
            waveSpawner.OnWaveFinished += HandleWaveFinished;

        originalButtonTexts = new string[attackButtons.Length];
        for (int i = 0; i < attackButtons.Length; i++)
        {
            Text btnText = attackButtons[i].GetComponentInChildren<Text>();
            if (btnText != null)
                originalButtonTexts[i] = btnText.text;

            int index = i;
            attackButtons[i].onClick.RemoveAllListeners();
            attackButtons[i].onClick.AddListener(() => ExecuteAttack(index));
        }

        InitializeBattle();
    }

    public void SetPlayer(Character newPlayer)
    {
        player = newPlayer;

        if (player == null)
        {
            Debug.LogError("❌ BattleManager recibió un player nulo");
            return;
        }

        InitializeBattle();
        Debug.Log("✅ Player asignado: " + player.characterName);
    }

    private void InitializeBattle()
    {
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

        UpdateAttackButtons();
    }

    private void UpdateAttackButtons()
    {
        if (attackButtons == null || player == null) return;

        for (int i = 0; i < attackButtons.Length; i++)
        {
            if (i >= player.attacks.Length)
            {
                attackButtons[i].interactable = false;
                Text btnTextEmpty = attackButtons[i].GetComponentInChildren<Text>();
                if (btnTextEmpty != null) btnTextEmpty.text = "";
                continue;
            }

            Attack atk = player.attacks[i];
            attackButtons[i].interactable = atk.currentCooldown <= 0 && !player.IsDead();

            Text btnText = attackButtons[i].GetComponentInChildren<Text>();
            if (btnText != null)
            {
                string baseName = originalButtonTexts[i];
                btnText.text = atk.currentCooldown > 0 ? $"{baseName} ({atk.currentCooldown})" : baseName;
            }
        }
    }

    private void ExecuteAttack(int attackIndex)
    {
        if (!playerTurn || attackIndex >= player.attacks.Length || player.IsDead()) return;

        Character enemy = waveSpawner.GetCurrentEnemy();
        if (enemy == null) return;

        Attack atk = player.attacks[attackIndex];
        if (atk.currentCooldown > 0) return;

        enemy.TakeDamage(atk.damage);
        atk.currentCooldown = atk.cooldownTurns;

        UpdateUI();
        UpdateAttackButtons();

        if (enemy.IsDead())
        {
            Debug.Log("👻 Enemigo derrotado.");
        }

        StartCoroutine(EnemyTurn());
    }

    private IEnumerator EnemyTurn()
    {
        playerTurn = false;
        yield return new WaitForSeconds(1f);

        Character enemy = waveSpawner.GetCurrentEnemy();
        if (enemy != null && enemy.attacks.Length > 0)
        {
            int randAtk = Random.Range(0, enemy.attacks.Length);
            Attack atk = enemy.attacks[randAtk];
            player.TakeDamage(atk.damage);
            Debug.Log($"{enemy.characterName} usa {atk.attackName}");
        }

        ReduceCooldowns();
        UpdateUI();
        UpdateAttackButtons();

        playerTurn = true;
    }

    private void ReduceCooldowns()
    {
        if (player == null) return;

        foreach (Attack atk in player.attacks)
        {
            if (atk.currentCooldown > 0)
                atk.currentCooldown--;
        }
    }

    private void UpdateUI()
    {
        if (playerHealthSlider != null)
            playerHealthSlider.value = player.currentHealth;

        Character enemy = waveSpawner.GetCurrentEnemy();
        if (enemyHealthSlider != null && enemy != null)
            enemyHealthSlider.value = enemy.currentHealth;
    }

    private void HandleWaveFinished()
    {
        Debug.Log("🏁 Oleada terminada");
        SceneManager.LoadScene("Victoria");
    }
}
