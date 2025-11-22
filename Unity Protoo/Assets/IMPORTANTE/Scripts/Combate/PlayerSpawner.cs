using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform spawnPoint;

    void Start()
    {
        var fantasma = GameManagerPersistente.Instancia.ghostSeleccionado;

        if (fantasma == null || fantasma.prefab == null)
        {
            Debug.LogError("No hay fantasma seleccionado o prefab asignado.");
            return;
        }

        GameObject playerObj = Instantiate(fantasma.prefab, spawnPoint.position, spawnPoint.rotation);
        Character player = playerObj.GetComponent<Character>();

        BattleManager bm = FindFirstObjectByType<BattleManager>();
        if (bm != null)
            bm.SetPlayer(player);
    }
}
