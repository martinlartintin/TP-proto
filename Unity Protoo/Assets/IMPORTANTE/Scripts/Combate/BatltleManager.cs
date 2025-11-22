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

    [Header("Fantasmas")]
    public int maxAttemptsPerBattle = 3;

    private bool playerTurn = true;
    private int currentAttempts = 0;

    void Start()
    {
        if (waveSpawner != null)
            waveSpawner.OnWaveFinished += HandleWaveFinished;

        if (player != null)
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
        currentAttempts = 0;

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

        for (int i = 0; i < attackButtons.Length; i++)
        {
            int index = i;
            attackButtons[i].onClick.RemoveAllListeners();
            attackButtons[i].onClick.AddListener(() => ExecuteAttack(index));
        }
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
                btnText.text = atk.attackName + (atk.currentCooldown > 0 ? $" ({atk.currentCooldown})" : "");
        }
    }

    public void ExecuteAttack(int attackIndex)
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
            Debug.Log("👻 Enemigo derrotado.");

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

        if (player.IsDead())
            HandlePlayerDefeat();
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
        HandlePlayerDefeat();
    }

    private void HandlePlayerDefeat()
    {
        currentAttempts++;

        if (GameManagerPersistente.Instancia != null)
        {
            GameManagerPersistente.Instancia.ectoplasma -= 5;
            if (GameManagerPersistente.Instancia.ectoplasma < 0)
                GameManagerPersistente.Instancia.ectoplasma = 0;
            Debug.Log("💀 Se restaron 5 ectoplasma al jugador por perder. Ectoplasma actual: " + GameManagerPersistente.Instancia.ectoplasma);
        }

        int remainingFantasmas = GameManagerPersistente.Instancia.ghostsDesbloqueados.Count - currentAttempts;
        if (currentAttempts < maxAttemptsPerBattle && remainingFantasmas > 0)
        {
            Debug.Log($"😢 Fantasma derrotado. Te quedan {maxAttemptsPerBattle - currentAttempts} intentos. Volviendo a selección.");
            SceneManager.LoadScene("SelecciónFantasmas");
        }
        else
        {
            Debug.Log("💀 No quedan intentos ni fantasmas. Escena de derrota.");
            SceneManager.LoadScene("Derrota");
        }
    }

}
