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
        foreach (var f in GameManagerPersistente.Instancia.fantasmasDesbloqueados)
            InstanciarFantasma(f);
    }

    public void InstanciarFantasma(FantasmaData data)
    {
        PersonajeData personaje = personajes.Find(p => p.nombre == data.nombre);
        if (personaje == null)
        {
            Debug.LogWarning($"⚠️ No se encontró prefab para el fantasma: {data.nombre}");
            return;
        }

        Transform punto = BuscarSiguienteTumbaLibre();
        if (punto == null)
        {
            Debug.LogWarning("⚠️ No hay spawn points disponibles.");
            return;
        }

        GameObject nuevo = Instantiate(personaje.prefab, punto.position, Quaternion.identity, punto);

        // 🔹 Forzar rotación limpia
        nuevo.transform.rotation = Quaternion.Euler(rotacionFija);
        nuevo.transform.localScale = escalaUniforme;

        // 🔹 Ajustar orientación del sprite (mirar hacia la misma dirección)
        var sprite = nuevo.GetComponentInChildren<SpriteRenderer>();
        if (sprite != null)
        {
            bool mirandoIzquierda = sprite.transform.localScale.x < 0;
            if (mirarHaciaDerecha && mirandoIzquierda)
                sprite.transform.localScale = new Vector3(-sprite.transform.localScale.x, sprite.transform.localScale.y, 1);
            else if (!mirarHaciaDerecha && !mirandoIzquierda)
                sprite.transform.localScale = new Vector3(-sprite.transform.localScale.x, sprite.transform.localScale.y, 1);
        }

        Debug.Log($"✅ {data.nombre} instanciado correctamente en {punto.name}");
    }

    private Transform BuscarSiguienteTumbaLibre()
    {
        foreach (var p in spawnPoints)
            if (p.childCount == 0)
                return p;
        return null;
    }
}
