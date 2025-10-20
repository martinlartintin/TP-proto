
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;


public enum Rareza
{
    Comun,
    Epico,
    Legendario
}

[System.Serializable]
public class Personaje
{
    public string nombre;
    public GameObject prefab;
    [Range(0, 100)] public int probabilidad;
    public Rareza rareza;
}

public class RarezaHolder : MonoBehaviour
{
    public Rareza rareza;
}

public class RandomShapeSpawner : MonoBehaviour
{
    public static RandomShapeSpawner Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [Header("Personajes disponibles")]
    public Personaje[] personajes = new Personaje[]
    {
        new Personaje { nombre = "LaLocomotora", probabilidad = 10, rareza = Rareza.Legendario },
        new Personaje { nombre = "FreddieMercury", probabilidad = 30, rareza = Rareza.Epico },
        new Personaje { nombre = "ManuelBelgrano", probabilidad = 60, rareza = Rareza.Comun }
    };

    [Header("Cajitas de spawn")]
    public Transform[] spawnPoints;

    [Header("UI")]
    public Button deleteButton;
    public Button cancelButton;
    public GameObject intercambioText;
    public TMP_Text ectoplasmaText;
    public Button invocarButton;

    [Header("Highlight")]
    public Material highlightMaterial;

    private int ectoplasmas = 10;
    private int nextSpawnIndex = 0;
    private Transform hoveredShape;
    private Transform clickedShape;
    private Renderer moveRenderer;
    private Material moveOriginalMat;
    private bool isMoving = false;
    private Vector3 spawnOffset = new Vector3(0, 0.5f, 0);

    private void Start()
    {
        deleteButton?.gameObject.SetActive(false);
        cancelButton?.gameObject.SetActive(false);
        intercambioText?.SetActive(false);

        deleteButton?.onClick.AddListener(DeleteTargetShape);
        cancelButton?.onClick.AddListener(CancelSelection);

        ectoplasmas = GameManager.Instance != null ? GameManager.Instance.ectoplasmas : 10;

        StartCoroutine(RestoreNextFrame());
        UpdateEctoplasmaUI();
    }

    private IEnumerator RestoreNextFrame()
    {
        yield return null;
        RestaurarPersonajes();
    }

    private void Update()
    {
        HandleMouseHover();
        HandleMouseClick();
    }

    // ---------------- COSTOS Y RECOMPENSAS ----------------
    private int CostoPorRareza(Rareza rareza)
    {
        switch (rareza)
        {
            case Rareza.Legendario: return 5;
            case Rareza.Epico: return 3;
            case Rareza.Comun: return 1;
            default: return 1;
        }
    }

    private int RecompensaPorRareza(Rareza rareza)
    {
        switch (rareza)
        {
            case Rareza.Legendario: return 3;
            case Rareza.Epico: return 2;
            case Rareza.Comun: return 1;
            default: return 1;
        }
    }

    // ---------------- INVOCAR ----------------
    public void InvocarPersonaje()
    {
        if (!HayCajaLibre())
        {
            Debug.Log("⚠ Todas las cajitas están ocupadas!");
            return;
        }

        Personaje elegido = ElegirPersonajeAleatorio();
        if (elegido == null) return;

        int costo = CostoPorRareza(elegido.rareza);
        if (ectoplasmas < costo)
        {
            Debug.Log(" No tienes suficientes ectoplasmas para invocar este personaje.");
            return;
        }

        // Buscar caja vacía
        Transform cajaVacia = null;
        int totalBoxes = spawnPoints.Length;
        int checkedBoxes = 0;

        while (checkedBoxes < totalBoxes)
        {
            Transform box = spawnPoints[nextSpawnIndex];
            bool ocupado = false;

            foreach (Transform child in box)
                if (child.CompareTag("Shape")) { ocupado = true; break; }

            if (!ocupado)
            {
                cajaVacia = box;
                break;
            }

            nextSpawnIndex = (nextSpawnIndex + 1) % totalBoxes;
            checkedBoxes++;
        }

        if (cajaVacia == null)
        {
            Debug.Log("⚠ Todas las cajitas están ocupadas!");
            return;
        }

        // Instanciar prefab
        GameObject spawned = Instantiate(elegido.prefab);
        spawned.transform.SetParent(GameManager.Instance.transform);
        spawned.transform.position = cajaVacia.position + spawnOffset;
        spawned.tag = "Shape";

        if (spawned.GetComponent<Collider>() == null)
            spawned.AddComponent<BoxCollider>();

        // Agregar RarezaHolder
        var holder = spawned.GetComponent<RarezaHolder>();
        if (holder == null) holder = spawned.AddComponent<RarezaHolder>();
        holder.rareza = elegido.rareza;

        // Colores según rareza
        Renderer rend = spawned.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material = new Material(rend.material);
            if (elegido.rareza == Rareza.Legendario) rend.material.color = Color.yellow;
            else if (elegido.rareza == Rareza.Epico) rend.material.color = Color.magenta;
            else rend.material.color = Color.green;
        }

        // Restar ectoplasmas
        ectoplasmas -= costo;
        UpdateEctoplasmaUI();
        GuardarEstado();

        // Avanzar índice
        nextSpawnIndex = (nextSpawnIndex + 1) % totalBoxes;

        Debug.Log($"🌀 Invocaste a {elegido.nombre} ({elegido.rareza}) - Costó {costo} ectoplasmas.");
    }

    // ---------------- ELECCIÓN ALEATORIA ----------------
    private Personaje ElegirPersonajeAleatorio()
    {
        List<Personaje> disponibles = new List<Personaje>();
        int totalProbabilidad = 0;

        foreach (var p in personajes)
        {
            int costo = CostoPorRareza(p.rareza);
            if (ectoplasmas >= costo)
            {
                disponibles.Add(p);
                totalProbabilidad += p.probabilidad;
            }
        }

        if (disponibles.Count == 0)
        {
            Debug.Log("⚠ No tienes ectoplasmas suficientes para invocar ningún personaje.");
            return null;
        }

        int randomPoint = Random.Range(0, totalProbabilidad);
        int acumulador = 0;

        foreach (var p in disponibles)
        {
            acumulador += p.probabilidad;
            if (randomPoint < acumulador)
                return p;
        }

        return disponibles[0];
    }

    // ---------------- CHEQUEO DE CAJAS ----------------
    private bool HayCajaLibre()
    {
        foreach (Transform box in spawnPoints)
        {
            bool ocupado = false;
            foreach (Transform child in box)
                if (child.CompareTag("Shape"))
                {
                    ocupado = true;
                    break;
                }

            if (!ocupado) return true;
        }
        return false;
    }

    // ---------------- ELIMINAR (EXORCIZAR) ----------------
    private void DeleteTargetShape()
    {
        if (clickedShape == null || !clickedShape.CompareTag("Shape")) return;

        // Leer rareza desde RarezaHolder
        Rareza rareza = Rareza.Comun;
        RarezaHolder holder = clickedShape.GetComponent<RarezaHolder>();
        if (holder != null) rareza = holder.rareza;

        int recompensa = RecompensaPorRareza(rareza);
        ectoplasmas += recompensa;

        Debug.Log($"💀 Exorcizaste un {rareza}! +{recompensa} ectoplasmas.");

        UpdateEctoplasmaUI();

        // Restaurar color de la caja
        Transform parentBox = null;
        foreach (Transform box in spawnPoints)
        {
            if (Vector3.Distance(clickedShape.position, box.position) < 0.1f)
            {
                parentBox = box;
                break;
            }
        }

        if (parentBox != null)
        {
            Renderer boxRend = parentBox.GetComponent<Renderer>();
            if (boxRend != null)
                boxRend.material.color = Color.white;
        }

        Destroy(clickedShape.gameObject);
        ClearDeleteSelection();
        GuardarEstado();
    }

    // ---------------- GUARDAR / RESTAURAR ----------------
    public void GuardarEstado()
{
    if (GameManager.Instance == null) return;

    // Guardamos los ectoplasmas actuales
    GameManager.Instance.ectoplasmas = ectoplasmas;

    // Limpiamos la lista de personajes invocados
    GameManager.Instance.personajesInvocados.Clear();

    // Filtramos los hijos de GameManager que no hayan sido destruidos
    Transform[] children = GameManager.Instance.transform.Cast<Transform>()
        .Where(c => c != null)
        .ToArray();

    foreach (Transform child in children)
    {
        if (child.CompareTag("Shape"))
        {
            Transform closestBox = null;
            float minDist = Mathf.Infinity;

            foreach (Transform box in spawnPoints)
            {
                if (box == null) continue; // chequeo extra por si algún spawnPoint desaparece
                float dist = Vector3.Distance(child.position, box.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestBox = box;
                }
            }

            // Guardamos los datos del personaje
            GameManager.Instance.personajesInvocados.Add(new GameManager.PersonajeData
            {
                nombre = child.name.Replace("(Clone)", ""),
                posicion = child.position,
                parentName = closestBox != null ? closestBox.name : ""
            });
        }
    }

    Debug.Log("✅ Estado guardado: " + GameManager.Instance.personajesInvocados.Count + " personajes.");
}



    private void RestaurarPersonajes()
    {
        if (GameManager.Instance == null) return;

        foreach (var data in GameManager.Instance.personajesInvocados)
        {
            Personaje prefab = null;
            foreach (var p in personajes)
                if (p.nombre == data.nombre) prefab = p;

            if (prefab == null) continue;

            bool yaExiste = false;
            foreach (Transform child in GameManager.Instance.transform)
                if (child.name.Contains(data.nombre)) yaExiste = true;
            if (yaExiste) continue;

            GameObject spawned = Instantiate(prefab.prefab);
            spawned.transform.SetParent(GameManager.Instance.transform);
            spawned.transform.position = data.posicion;
            spawned.tag = "Shape";

            if (spawned.GetComponent<Collider>() == null)
                spawned.AddComponent<BoxCollider>();

            // RarezaHolder
            var holder = spawned.GetComponent<RarezaHolder>();
            if (holder == null) holder = spawned.AddComponent<RarezaHolder>();
            holder.rareza = prefab.rareza;

            Renderer rend = spawned.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material = new Material(rend.material);
                if (prefab.rareza == Rareza.Legendario) rend.material.color = Color.yellow;
                else if (prefab.rareza == Rareza.Epico) rend.material.color = Color.magenta;
                else rend.material.color = Color.green;
            }
        }
    }

    // ---------------- UI ----------------
    private void UpdateEctoplasmaUI()
    {
        if (ectoplasmaText != null)
            ectoplasmaText.text = "Ectoplasmas: " + ectoplasmas;

        if (invocarButton != null)
            invocarButton.interactable = (ectoplasmas > 0) && HayCajaLibre();
    }

    // ---------------- INTERACCIÓN ----------------
    private void HandleMouseHover()
    {
        if (isMoving) return;
        if (Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform hitTransform = hit.transform;

            if (hitTransform.CompareTag("Shape"))
            {
                if (hoveredShape != null && hoveredShape != hitTransform)
                    ClearHover();

                hoveredShape = hitTransform;
                Renderer rend = hoveredShape.GetComponent<Renderer>();
                if (rend != null && hoveredShape != clickedShape)
                    rend.material = highlightMaterial;
            }
            else ClearHover();
        }
        else ClearHover();
    }

    private void ClearHover()
    {
        if (hoveredShape != null && hoveredShape != clickedShape)
        {
            Renderer rend = hoveredShape.GetComponent<Renderer>();
            if (rend != null && moveOriginalMat != null)
                rend.material = moveOriginalMat;
        }
        hoveredShape = null;
    }

    private void HandleMouseClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (Camera.main == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform hitTransform = hit.transform;

            if (hitTransform.CompareTag("Shape"))
            {
                if (clickedShape != hitTransform)
                    SelectShape(hitTransform);
            }
        }
    }

    private void SelectShape(Transform shape)
    {
        if (!shape.CompareTag("Shape")) return;

        ClearDeleteSelection();
        clickedShape = shape;

        Renderer rend = shape.GetComponent<Renderer>();
        if (rend != null)
        {
            moveOriginalMat = rend.material;
            rend.material = highlightMaterial;
        }

        deleteButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
    }

    private void CancelSelection() => ClearDeleteSelection();

    private void ClearDeleteSelection()
    {
        if (clickedShape != null)
        {
            Renderer rend = clickedShape.GetComponent<Renderer>();
            if (rend != null && moveOriginalMat != null)
                rend.material = moveOriginalMat;
        }

        clickedShape = null;
        deleteButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
    }
}   