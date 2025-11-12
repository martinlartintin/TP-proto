using UnityEngine;
using System.Linq;

public class RandomShapeSpawner : MonoBehaviour
{
    [Header("Personajes y Spawn")]
    public PersonajeData[] personajes;
    public Transform[] spawnPoints;
    public Vector3 spawnOffset = new Vector3(0, 0.5f, 0);

    private bool[] tumbasOcupadas;

    void Start()
    {
        tumbasOcupadas = new bool[spawnPoints.Length];

        // 🔹 Primero, recrear todos los fantasmas invocados anteriormente
        if (GameManagerPersistente.Instancia != null)
        {
            foreach (var f in GameManagerPersistente.Instancia.fantasmasInvocados)
            {
                InstanciarFantasma(f);
            }

            // 🔹 Luego, si hay un fantasma recién invocado, sumarlo
            var nuevo = GameManagerPersistente.Instancia.fantasmaSeleccionado;
            if (nuevo != null)
            {
                InstanciarFantasma(nuevo);
                GameManagerPersistente.Instancia.fantasmasInvocados.Add(nuevo);

                // Evitar repetirlo en el siguiente cambio de escena
                GameManagerPersistente.Instancia.fantasmaSeleccionado = null;
            }
        }
    }

    private void InstanciarFantasma(FantasmaData data)
    {
        var prefabData = personajes.FirstOrDefault(p => p.nombre == data.nombre);
        if (prefabData == null)
        {
            Debug.LogWarning($"❌ No se encontró prefab para {data.nombre}");
            return;
        }

        int indice = BuscarSiguienteTumbaLibre();
        if (indice == -1)
        {
            Debug.LogWarning("⚠️ Todas las tumbas están ocupadas, no se puede invocar más fantasmas.");
            return;
        }

        Transform tumba = spawnPoints[indice];
        Vector3 pos = tumba.position + spawnOffset;

        GameObject nuevoFantasma = Instantiate(prefabData.prefab, pos, Quaternion.identity);
        nuevoFantasma.name = prefabData.nombre;

        tumbasOcupadas[indice] = true;

        // Guardar el nombre de la tumba
        data.tumbaName = tumba.name;

        Debug.Log($"✅ Fantasma {data.nombre} invocado en {tumba.name} (índice {indice})");
    }

    private int BuscarSiguienteTumbaLibre()
    {
        for (int i = 0; i < tumbasOcupadas.Length; i++)
        {
            if (!tumbasOcupadas[i])
            {
                Debug.Log($"□ Tumba libre encontrada: {spawnPoints[i].name} (índice {i})");
                return i;
            }
        }
        return -1;
    }
}
