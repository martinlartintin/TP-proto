using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using TMPro;

public class RandomShapeSpawner : MonoBehaviour
{
    [Header("Personajes y Spawn")]
    public GameManager.PersonajeData[] personajes; // Datos de cada personaje
    public Transform[] spawnPoints;
    public Vector3 spawnOffset = new Vector3(0, 0.5f, 0);

    [Header("Referencias")]
    public Ruleta ruletaRef;
    public TMP_Text ectoplasmaText;

    private List<Transform> spawnOcupados = new List<Transform>();

    void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError($" spawnPoints no están asignados en {gameObject.name}");
            return;
        }

        RestaurarFantasmas();
        InstanciarPersonajesGuardados();
        ActualizarUI();
    }

    private Transform ObtenerTumbaLibre()
    {
        foreach (Transform punto in spawnPoints)
        {
            bool ocupada = spawnOcupados.Contains(punto) ||
                           (GameManagerPersistente.Instancia != null &&
                            GameManagerPersistente.Instancia.fantasmasDesbloqueados
                                .Any(f => f.tumbaName == punto.name));

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
        if (GameManager.Instance == null || GameManager.Instance.personajesInvocados.Count == 0) return;

        foreach (var data in GameManager.Instance.personajesInvocados)
        {
            var prefabData = personajes.FirstOrDefault(p => p.nombre == data.nombre);
            if (prefabData == null) continue;

            Transform tumbaLibre = ObtenerTumbaLibre();
            if (tumbaLibre == null)
            {
                Debug.LogWarning("⚠️ No hay tumbas libres para instanciar el personaje.");
                break;
            }

            CrearFantasma(prefabData, tumbaLibre);
        }

        GameManager.Instance.personajesInvocados.Clear();
    }

    private void RestaurarFantasmas()
    {
        if (GameManagerPersistente.Instancia == null) return;

        foreach (var data in GameManagerPersistente.Instancia.fantasmasDesbloqueados)
        {
            var prefabData = personajes.FirstOrDefault(p => p.nombre == data.nombre);
            if (prefabData == null) continue;

            Transform tumba = spawnPoints.FirstOrDefault(p => p.name == data.tumbaName);
            if (tumba == null || spawnOcupados.Contains(tumba)) continue;

            spawnOcupados.Add(tumba);
            CrearFantasma(prefabData, tumba);
        }
    }

    private GameObject CrearFantasma(GameManager.PersonajeData prefabData, Transform tumba)
    {
        Vector3 pos = tumba.position + spawnOffset;

        GameObject nuevoFantasma = Instantiate(prefabData.prefab, pos, Quaternion.identity);
        nuevoFantasma.tag = "Shape";

        Character character = nuevoFantasma.GetComponent<Character>();
        if (character != null)
        {
            character.characterName = prefabData.nombre;
            character.currentHealth = character.maxHealth;
        }

        var holder = nuevoFantasma.GetComponent<RarezaHolder>();
        if (holder == null) holder = nuevoFantasma.AddComponent<RarezaHolder>();
        holder.rareza = prefabData.rareza;

        Renderer rend = nuevoFantasma.GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            rend.material = new Material(rend.material);
            switch (prefabData.rareza)
            {
                case Rareza.Legendario: rend.material.color = Color.yellow; break;
                case Rareza.Epico: rend.material.color = Color.magenta; break;
                default: rend.material.color = Color.green; break;
            }
        }

        if (GameManagerPersistente.Instancia != null &&
            !GameManagerPersistente.Instancia.fantasmasDesbloqueados.Any(f => f.tumbaName == tumba.name))
        {
            GameManagerPersistente.Instancia.fantasmasDesbloqueados.Add(new FantasmaData
            {
                nombre = prefabData.nombre,
                rareza = prefabData.rareza,
                tumbaName = tumba.name
            });
        }

        if (ruletaRef != null)
            ruletaRef.HabilitarRuleta();

        ActualizarUI();

        Debug.Log($"💀 Fantasma '{prefabData.nombre}' ({prefabData.rareza}) colocado en tumba '{tumba.name}'");
        return nuevoFantasma;
    }

    public void InvocarFantasmaRandom(List<GameManager.PersonajeData> lista)
    {
        if (lista == null || lista.Count == 0) return;

        Transform tumbaLibre = ObtenerTumbaLibre();
        if (tumbaLibre == null)
        {
            Debug.LogWarning("⚠️ No hay tumbas libres para instanciar el personaje.");
            return;
        }

        GameManager.PersonajeData elegido = lista[Random.Range(0, lista.Count)];
        CrearFantasma(elegido, tumbaLibre);
    }

    private void ActualizarUI()
    {
        if (ectoplasmaText != null && GameManagerPersistente.Instancia != null)
            ectoplasmaText.text = $"Ectoplasma: {GameManagerPersistente.Instancia.ectoplasma}";
    }
}
