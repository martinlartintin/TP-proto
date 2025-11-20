using UnityEngine;
using System.Collections.Generic;

public class RandomShapeSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Prefabs por nombre")]
    public List<PersonajeData> personajes;

    [Header("Ajustes globales")]
    public Vector3 escalaUniforme = Vector3.one;
    public Vector3 rotacionFija = Vector3.zero;
    public bool mirarHaciaDerecha = true;

    private void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("⚠️ No hay spawn points asignados.");
            return;
        }

        foreach (var f in GameManagerPersistente.Instancia.fantasmasDesbloqueados)
        {
            InstanciarFantasma(f);
        }
    }

    public GameObject InstanciarFantasma(FantasmaData data)
    {
        if (data == null)
        {
            Debug.LogError("⚠️ FantasmaData es null. No se puede instanciar.");
            return null;
        }

        PersonajeData personaje = personajes.Find(p => p.nombre == data.nombre);
        if (personaje == null)
        {
            Debug.LogWarning($"⚠️ No se encontró PersonajeData para el fantasma: {data.nombre}");
            return null;
        }

        if (personaje.prefab == null)
        {
            Debug.LogError($"⚠️ El prefab de {data.nombre} está vacío. Asigna un prefab en el Inspector.");
            return null;
        }

        Transform punto = BuscarSiguienteTumbaLibre();
        if (punto == null)
        {
            Debug.LogWarning("⚠️ No hay spawn points disponibles.");
            return null;
        }

        GameObject nuevo = Instantiate(personaje.prefab, punto.position, Quaternion.identity, punto);

        // 🔹 Forzar rotación y escala
        nuevo.transform.rotation = Quaternion.Euler(rotacionFija);
        nuevo.transform.localScale = escalaUniforme;

        // 🔹 Ajustar orientación del sprite
        var sprite = nuevo.GetComponentInChildren<SpriteRenderer>();
        if (sprite != null)
        {
            Vector3 scale = sprite.transform.localScale;
            scale.x = mirarHaciaDerecha ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            sprite.transform.localScale = scale;
        }

        Debug.Log($"✅ {data.nombre} instanciado correctamente en {punto.name}");
        return nuevo;
    }

    private Transform BuscarSiguienteTumbaLibre()
    {
        foreach (var p in spawnPoints)
        {
            if (p.childCount == 0)
                return p;
        }
        return null;
    }
}
