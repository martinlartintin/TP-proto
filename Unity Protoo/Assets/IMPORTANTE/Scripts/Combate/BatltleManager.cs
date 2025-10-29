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
    public Button[] attackButtons;

    [Header("Spawner")]
    public WaveSpawnerSequential waveSpawner;

    private bool playerTurn = true;

    void Start()
    {
        // Inicializar sliders
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

        // Inicializar botones de ataque
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

            // Limpiar listeners antiguos
            btn.onClick.RemoveAllListeners();

            // Captura local del índice
            int capturedIndex = i;
            btn.onClick.AddListener(() =>
            {
                Debug.Log($"Presionaste botón {capturedIndex}");
                OnPlayerAttack(capturedIndex);
            });

            Debug.Log($"Botón {i} asignado a ataque: {player.attacks[i].name}");
        }
    }

    public void OnPlayerAttack(int attackIndex)
    {
        if (!playerTurn) return;
        if (attackIndex >= player.attacks.Length)
        {
            Debug.LogWarning($"⚠️ Intento de usar ataque {attackIndex}, pero no existe en player.attacks");
            return;
        }

        Character enemy = waveSpawner.GetCurrentEnemy();
        if (enemy == null) return;

        Attack chosenAttack = player.attacks[attackIndex];

        // Debug para confirmar ataque correcto
        Debug.Log($"Botón presionado: {attackIndex}, Ataque usado: {chosenAttack.name}");

        if (chosenAttack.currentCooldown > 0)
        {
            Debug.Log($"El ataque {chosenAttack.name} está en cooldown ({chosenAttack.currentCooldown} turnos restantes)");
            return;
        }

        // Aplicar daño
        enemy.TakeDamage(chosenAttack.damage);

        if (enemyHealthSlider != null)
            enemyHealthSlider.value = enemy.currentHealth;

        Debug.Log($"{player.characterName} usa {chosenAttack.name} e inflige {chosenAttack.damage} de daño");
        Debug.Log($"{enemy.characterName} tiene {enemy.currentHealth} de vida");

        // Aplicar cooldown
        if (chosenAttack.cooldownTurns > 0)
            chosenAttack.currentCooldown = chosenAttack.cooldownTurns;

        UpdateAttackButtons();

        if (waveSpawner.IsWaveFinished())
        {
            Debug.Log("¡Ganaste la oleada!");
            return;
        }

        // Turno enemigo
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

        Debug.Log($"{enemy.characterName} usa {chosenAttack.name}");
        Debug.Log($"{player.characterName} tiene {player.currentHealth} de vida");

        if (player.IsDead())
        {
            Debug.Log("Perdiste...");
            yield break;
        }

        ReduceCooldowns();
        playerTurn = true;
        UpdateAttackButtons();

        Debug.Log("Es tu turno.");
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

            if (btnText == null)
            {
                Debug.LogWarning($"⚠️ El botón {attackButtons[i].name} no tiene un Text hijo");
                continue;
            }

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
}
