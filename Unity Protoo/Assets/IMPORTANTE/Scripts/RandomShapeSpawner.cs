using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class RandomShapeSpawner : MonoBehaviour
{
    [Header("Personajes y Spawn")]
    public Personaje[] personajes;
    public Transform[] spawnPoints;
    public Vector3 spawnOffset = new Vector3(0, 0.5f, 0);

    [Header("Referencias")]
    public TMP_Text ectoplasmaText;

    private List<Transform> spawnOcupados = new List<Transform>();

    private void Start()
    {
        RestaurarFantasmas();
        InstanciarPersonajesGuardados();
        ActualizarUI();
    }

    private Transform ObtenerTumbaLibre()
    {
        foreach (Transform punto in spawnPoints)
        {
            bool ocupada = spawnOcupados.Contains(punto) ||
                GameManagerPersistente.Instancia.fantasmasGuardados.Any(f => f.tumbaName == punto.name);

            if (!ocupada)
            {
                spawnOcupados.Add(punto);
                return punto;
            }
        }
        return null;
    }

    private void InstanciarPersonajesGuardados()
    {
        foreach (var data in GameManagerPersistente.Instancia.personajesInvocados)
        {
            Personaje prefab = personajes.FirstOrDefault(p => p.nombre == data.nombre);
            if (prefab == null) continue;

            Transform tumbaLibre = ObtenerTumbaLibre();
            if (tumbaLibre == null) break;

            CrearFantasma(prefab, tumbaLibre, data.localPosition);
        }

        GameManagerPersistente.Instancia.personajesInvocados.Clear();
    }

    private void RestaurarFantasmas()
    {
        foreach (var data in GameManagerPersistente.Instancia.fantasmasGuardados)
        {
            Personaje prefab = personajes.FirstOrDefault(p => p.nombre == data.nombre);
            if (prefab == null) continue;

            Transform tumba = spawnPoints.FirstOrDefault(p => p.name == data.tumbaName);
            if (tumba == null || spawnOcupados.Contains(tumba)) continue;

            spawnOcupados.Add(tumba);
            CrearFantasma(prefab, tumba, Vector3.zero);
        }
    }

    private GameObject CrearFantasma(Personaje prefab, Transform tumba, Vector3 offset)
    {
        Vector3 pos = tumba.position + offset;
        GameObject nuevoFantasma = Instantiate(prefab.prefab, pos, Quaternion.identity);
        nuevoFantasma.transform.SetParent(tumba);
        nuevoFantasma.transform.localPosition = offset;
        nuevoFantasma.tag = "Shape";

        var holder = nuevoFantasma.GetComponent<RarezaHolder>();
        if (holder == null) holder = nuevoFantasma.AddComponent<RarezaHolder>();
        holder.rareza = prefab.rareza;

        Renderer rend = nuevoFantasma.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            rend.material = new Material(rend.material);
            switch (prefab.rareza)
            {
                case Rareza.Legendario: rend.material.color = Color.yellow; break;
                case Rareza.Epico: rend.material.color = Color.magenta; break;
                default: rend.material.color = Color.green; break;
            }
        }

        if (!GameManagerPersistente.Instancia.fantasmasGuardados.Any(f => f.tumbaName == tumba.name))
        {
            GameManagerPersistente.Instancia.fantasmasGuardados.Add(new FantasmaData
            {
                nombre = prefab.nombre,
                rareza = prefab.rareza,
                tumbaName = tumba.name
            });
        }

        ActualizarUI();
        return nuevoFantasma;
    }

    private void ActualizarUI()
    {
        if (ectoplasmaText != null)
            ectoplasmaText.text = $"Ectoplasma: {GameManagerPersistente.Instancia.ectoplasma}";
    }
}
