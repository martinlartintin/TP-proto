using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform spawnPoint;

    void Start()
    {
        var fantasma = GameManagerPersistente.Instancia.fantasmaSeleccionado;

        if (fantasma == null)
        {
            Debug.LogError("❌ No se encontró el fantasma seleccionado.");
            return;
        }

        if (fantasma.prefab == null)
        {
            Debug.LogError("❌ El fantasma seleccionado NO tiene prefab asignado.");
            return;
        }

        GameObject playerObj = Instantiate(fantasma.prefab, spawnPoint.position, spawnPoint.rotation);
        Character player = playerObj.GetComponent<Character>();

        if (player == null)
        {
            Debug.LogError("❌ El prefab del jugador no tiene componente 'Character'.");
            return;
        }

        BattleManager bm = FindObjectOfType<BattleManager>();
        if (bm != null)
        {
            bm.SetPlayer(player);
            Debug.Log("✔ Player asignado al BattleManager.");
        }
        else
        {
            Debug.LogError("❌ No se encontró BattleManager en la escena.");
        }

        Debug.Log("✔ Fantasma instanciado correctamente.");
    }
}
