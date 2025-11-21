using System.Collections;
using UnityEngine;

public class WaveSpawnerSequential : MonoBehaviour
{
    [Header("Configuración de oleada")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int enemiesPerWave = 5;
    public float delayBetweenSpawns = 1f;

    [Header("Recompensa por OLEADA")]
    public int ectoplasmaPorOleada = 5;   

    public System.Action OnWaveFinished;

    private Character currentEnemy;
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
            if (currentEnemy == null && enemiesSpawned < enemiesPerWave)
            {
                SpawnNextEnemy();

                // Espera a que muera
                yield return new WaitUntil(() => currentEnemy.IsDead());

                enemiesDefeated++;

                Destroy(currentEnemy.gameObject, 0.1f);
                currentEnemy = null;

                yield return new WaitForSeconds(delayBetweenSpawns);
            }
            else
            {
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

    private void WaveFinished()
    {
        Debug.Log("🏆 Oleada COMPLETADA.");

        if (GameManagerPersistente.Instancia != null)
        {
            GameManagerPersistente.Instancia.ectoplasma += ectoplasmaPorOleada;
            Debug.Log($"💰 Recompensa de oleada: +{ectoplasmaPorOleada} ectoplasma");
        }

        OnWaveFinished?.Invoke();
    }

    public Character GetCurrentEnemy()
    {
        return currentEnemy;
    }

    public bool IsWaveFinished()
    {
        return !waveRunning || enemiesDefeated >= enemiesPerWave;
    }
}
