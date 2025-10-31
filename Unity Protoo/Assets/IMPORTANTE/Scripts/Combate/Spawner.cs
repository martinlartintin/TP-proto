using System.Collections;
using UnityEngine;

public class WaveSpawnerSequential : MonoBehaviour
{
    [Header("Enemigos")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int enemiesPerWave = 5;
    public float delayBetweenSpawns = 1f;

    private int enemiesSpawned = 0;
    private int enemiesDefeated = 0;
    private Character currentEnemy;      

    void Start()
    {
        StartCoroutine(HandleWave());
    }

    IEnumerator HandleWave()
    {
        while (enemiesDefeated < enemiesPerWave)
        {
            if (currentEnemy == null)
            {
                SpawnNextEnemy();
            }

            yield return new WaitUntil(() => currentEnemy != null && currentEnemy.IsDead());

            if (currentEnemy != null && currentEnemy.IsDead())
            {
                enemiesDefeated++;

                GameObject obj = currentEnemy.gameObject;
                currentEnemy = null;
                Destroy(obj, 0.1f);
            }

            yield return new WaitForSeconds(delayBetweenSpawns);
        }

        WaveFinished();
    }

    void SpawnNextEnemy()
    {
        if (enemiesSpawned >= enemiesPerWave) return;

        Transform spawn = spawnPoints.Length > 0
            ? spawnPoints[Random.Range(0, spawnPoints.Length)]
            : transform;

        GameObject enemyObj = Instantiate(enemyPrefab, spawn.position, spawn.rotation);
        currentEnemy = enemyObj.GetComponent<Character>();
        enemiesSpawned++;

        Debug.Log($"Enemigo {enemiesSpawned}/{enemiesPerWave} apareció");
    }

    void WaveFinished()
    {
        Debug.Log("Oleada terminada: todos los enemigos fueron derrotados.");
    }

    public bool IsWaveFinished()
    {
        return enemiesDefeated >= enemiesPerWave;
    }

    public Character GetCurrentEnemy()
    {
        return currentEnemy;
    }
}
