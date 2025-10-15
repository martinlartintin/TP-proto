using System.Collections;
using UnityEngine;

public class WaveSpawnerSequential : MonoBehaviour
{
    [Header("Enemigos")]
    public GameObject enemyPrefab;       // Prefab del enemigo (con Character)
    public Transform[] spawnPoints;      // Puntos de spawn
    public int enemiesPerWave = 5;       // Total de enemigos en la oleada
    public float delayBetweenSpawns = 1f; // Tiempo entre enemigos

    private int enemiesSpawned = 0;      // Cuántos enemigos se han generado
    private int enemiesDefeated = 0;     // Cuántos murieron
    private Character currentEnemy;      // Enemigo activo

    void Start()
    {
        StartCoroutine(HandleWave());
    }

    IEnumerator HandleWave()
    {
        while (enemiesDefeated < enemiesPerWave)
        {
            // Si no hay enemigo activo, spawneá uno
            if (currentEnemy == null)
            {
                SpawnNextEnemy();
            }

            // Esperar hasta que el enemigo muera
            yield return new WaitUntil(() => currentEnemy != null && currentEnemy.IsDead());

            // Contar la derrota y destruir el objeto con pequeño delay
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

    // Funciones públicas para BattleManager
    public bool IsWaveFinished()
    {
        return enemiesDefeated >= enemiesPerWave;
    }

    public Character GetCurrentEnemy()
    {
        return currentEnemy;
    }
}
