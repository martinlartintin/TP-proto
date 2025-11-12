using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    [Header("UI")]
    public Slider playerHealthSlider;
    public Slider enemyHealthSlider;
    public Button[] attackButtons;

    [Header("Spawner")]
    public WaveSpawnerSequential waveSpawner;

    private Character player;
    private bool playerTurn = true;

    void Start()
    {
        // Instanciar el prefab del fantasma seleccionado
        var seleccionado = GameManagerPersistente.Instancia.fantasmaSeleccionado;
        if (seleccionado != null && seleccionado.personaje != null && seleccionado.personaje.prefab != null)
        {
            Transform spawn = GameObject.Find("PlayerSpawnPoint")?.transform;
            Vector3 pos = spawn != null ? spawn.position : Vector3.zero;

            GameObject playerObj = Instantiate(seleccionado.personaje.prefab, pos, Quaternion.identity);
            playerObj.name = seleccionado.nombre;
            playerObj.tag = "Player";

            player = playerObj.GetComponent<Character>();
        }

        if (player == null)
        {
            Debug.LogError("⚠️ No se pudo instanciar el jugador. Revisa que el prefab tenga el script Character.");
            return;
        }

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
            if (btn == null) continue;

            btn.onClick.RemoveAllListeners();
            int capturedIndex = i;
            btn.onClick.AddListener(() => { OnPlayerAttack(capturedIndex); });
        }
    }

    public void OnPlayerAttack(int attackIndex)
    {
        if (!playerTurn) return;
        if (attackIndex >= player.attacks.Length) return;

        Character enemy = waveSpawner.GetCurrentEnemy();
        if (enemy == null) return;

        Attack chosenAttack = player.attacks[attackIndex];
        if (chosenAttack.currentCooldown > 0) return;

        // Aplicar daño
        enemy.TakeDamage(chosenAttack.damage);
        if (enemyHealthSlider != null) enemyHealthSlider.value = enemy.currentHealth;

        // Aplicar cooldown
        if (chosenAttack.cooldownTurns > 0) chosenAttack.currentCooldown = chosenAttack.cooldownTurns;

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
        if (playerHealthSlider != null) playerHealthSlider.value = player.currentHealth;

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
            if (atk.currentCooldown > 0) atk.currentCooldown--;
        }
    }

    private void UpdateAttackButtons()
    {
        int minLength = Mathf.Min(attackButtons.Length, player.attacks.Length);

        for (int i = 0; i < minLength; i++)
        {
            Attack atk = player.attacks[i];
            Text btnText = attackButtons[i].GetComponentInChildren<Text>();
            if (btnText == null) continue;

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
