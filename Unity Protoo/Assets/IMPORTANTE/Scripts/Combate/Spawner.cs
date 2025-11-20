using System.Collections;
using UnityEngine;

public class WaveSpawnerSequential : MonoBehaviour
{
    [Header("Configuración de oleada")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int enemiesPerWave = 5;
    public float delayBetweenSpawns = 1f;

    [Header("Recompensa")]
    public int ectoplasmaPorEnemigo = 3; // 💰 Ectoplasma ganado por cada enemigo derrotado

    private Character currentEnemy; // Variable global para compatibilidad
    private int enemiesSpawned = 0;
    private int enemiesDefeated = 0;
    private bool waveRunning = false;

    private void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("❌ No se asignó un prefab de enemigo.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("❌ No se asignaron puntos de spawn.");
            return;
        }

        StartCoroutine(HandleWave());
    }

    private IEnumerator HandleWave()
    {
        waveRunning = true;
        enemiesSpawned = 0;
        enemiesDefeated = 0;

        Debug.Log("🚀 Iniciando nueva oleada...");

        while (enemiesDefeated < enemiesPerWave)
        {
            // Spawn solo si no hay enemigo activo
            if (currentEnemy == null && enemiesSpawned < enemiesPerWave)
            {
                SpawnNextEnemy();

                // Espera hasta que el enemigo actual muera
                yield return new WaitUntil(() => currentEnemy.IsDead());

                // Otorgar recompensa
                enemiesDefeated++;
                GiveEctoplasma(ectoplasmaPorEnemigo);

                Destroy(currentEnemy.gameObject, 0.1f);
                currentEnemy = null;

                yield return new WaitForSeconds(delayBetweenSpawns);
            }
            else
            {
                // Espera un frame si aún hay enemigo activo
                yield return null;
            }
        }

        WaveFinished();
        waveRunning = false;
    }

    private void SpawnNextEnemy()
    {
        if (enemiesSpawned >= enemiesPerWave) return;

        Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyObj = Instantiate(enemyPrefab, spawn.position, spawn.rotation);

        currentEnemy = enemyObj.GetComponent<Character>();

        if (currentEnemy == null)
        {
            Debug.LogError($"⚠️ El prefab '{enemyPrefab.name}' no tiene componente 'Character'.");
            Destroy(enemyObj);
        }
        else
        {
            currentEnemy.gameObject.name = $"Enemy_{enemiesSpawned + 1}";
            Debug.Log($"👻 Enemigo {enemiesSpawned + 1}/{enemiesPerWave} apareció en {spawn.name}");
        }

        enemiesSpawned++;
    }

    private void GiveEctoplasma(int cantidad)
    {
        if (GameManagerPersistente.Instancia != null)
        {
            GameManagerPersistente.Instancia.ectoplasma += cantidad;
            Debug.Log($"💀 Enemigo derrotado (+{cantidad} ectoplasma). Total: {GameManagerPersistente.Instancia.ectoplasma}");
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró GameManagerPersistente.");
        }
    }

    private void WaveFinished()
    {
        Debug.Log("✅ Oleada terminada: todos los enemigos derrotados.");
    }

    // 🔹 Método agregado para compatibilidad con BattleManager
    public Character GetCurrentEnemy()
    {
        return currentEnemy;
    }

    public bool IsWaveFinished()
    {
        return !waveRunning || enemiesDefeated >= enemiesPerWave;
    }
}
