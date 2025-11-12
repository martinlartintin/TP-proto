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

    private int enemiesSpawned = 0;
    private int enemiesDefeated = 0;
    private Character currentEnemy;

    private bool waveRunning = false;

    private void Start()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("❌ No se asignó un prefab de enemigo en WaveSpawnerSequential.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("❌ No se asignaron puntos de aparición (spawnPoints).");
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
            if (currentEnemy == null && enemiesSpawned < enemiesPerWave)
            {
                SpawnNextEnemy();
            }

            yield return new WaitUntil(() => currentEnemy != null && currentEnemy.IsDead());

            if (currentEnemy != null && currentEnemy.IsDead())
            {
                enemiesDefeated++;

                // 💰 Otorgar ectoplasma al jugador
                if (GameManagerPersistente.Instancia != null)
                {
                    GameManagerPersistente.Instancia.ectoplasma += ectoplasmaPorEnemigo;
                    Debug.Log($"💀 Enemigo derrotado (+{ectoplasmaPorEnemigo} ectoplasma). Total: {GameManagerPersistente.Instancia.ectoplasma}");
                }
                else
                {
                    Debug.LogWarning("⚠️ No se encontró GameManagerPersistente, no se pudo otorgar ectoplasma.");
                }

                Destroy(currentEnemy.gameObject, 0.1f);
                currentEnemy = null;
            }

            yield return new WaitForSeconds(delayBetweenSpawns);
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
        }
        else
        {
            currentEnemy.gameObject.name = $"Enemy_{enemiesSpawned + 1}";
            Debug.Log($"👻 Enemigo {enemiesSpawned + 1}/{enemiesPerWave} apareció en {spawn.name}");
        }

        enemiesSpawned++;
    }

    private void WaveFinished()
    {
        Debug.Log("✅ Oleada terminada: todos los enemigos fueron derrotados.");
    }

    public bool IsWaveFinished()
    {
        return !waveRunning || enemiesDefeated >= enemiesPerWave;
    }

    public Character GetCurrentEnemy()
    {
        return currentEnemy;
    }
}
