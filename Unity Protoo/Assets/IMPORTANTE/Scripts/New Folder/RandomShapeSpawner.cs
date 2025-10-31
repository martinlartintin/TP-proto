using UnityEngine;
using System.Linq;

public class RandomShapeSpawner : MonoBehaviour
{
    [Header("Personajes y Spawn")]
    public PersonajeData[] personajes;
    public Transform[] spawnPoints;
    public Vector3 spawnOffset = new Vector3(0, 0.5f, 0);

    void Start()
    {
        SpawnFantasmaSeleccionado();
    }

    void SpawnFantasmaSeleccionado()
    {
        if (GameManagerPersistente.Instancia == null) return;

        var seleccionado = GameManagerPersistente.Instancia.fantasmaSeleccionado;
        if (seleccionado == null)
        {
            Debug.LogWarning("No se ha seleccionado ningún fantasma.");
            return;
        }

        var prefabData = personajes.FirstOrDefault(p => p.nombre == seleccionado.nombre);
        if (prefabData == null)
        {
            Debug.LogWarning($"No se encontró prefab para {seleccionado.nombre}");
            return;
        }

        Transform tumba = spawnPoints.Length > 0 ? spawnPoints[0] : null;
        if (tumba == null)
        {
            Debug.LogWarning("No hay spawn points asignados.");
            return;
        }

        Vector3 pos = tumba.position + spawnOffset;
        GameObject nuevoFantasma = Instantiate(prefabData.prefab, pos, Quaternion.identity);
        nuevoFantasma.tag = "Player";

        var character = nuevoFantasma.GetComponent<Character>();
        if (character != null)
        {
            character.characterName = prefabData.nombre;
            character.currentHealth = character.maxHealth;
        }

        var holder = nuevoFantasma.GetComponent<RarezaHolder>();
        if (holder == null) holder = nuevoFantasma.AddComponent<RarezaHolder>();
        holder.rareza = prefabData.rareza;

        Debug.Log($"Fantasma instanciado: {prefabData.nombre}");
    }
}
