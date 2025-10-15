using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class Personaje
{
    public string nombre;
    public GameObject prefab;
    [Range(0, 100)] public int probabilidad;
}

public class RandomShapeSpawner : MonoBehaviour
{
    // ---------------- Singleton ----------------
    public static RandomShapeSpawner Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ---------------- Variables ----------------
    [Header("Personajes disponibles")]
    public Personaje[] personajes = new Personaje[]
    {
        new Personaje { nombre = "LaLocomotora", probabilidad = 10 },
        new Personaje { nombre = "FreddieMercury", probabilidad = 30 },
        new Personaje { nombre = "ManuelBelgrano", probabilidad = 60 }
    };

    [Header("Cajitas de spawn")]
    public Transform[] spawnPoints;

    [Header("UI")]
    public Button deleteButton;
    public Button cancelButton;
    public GameObject intercambioText;
    public TMP_Text ectoplasmaText;
    public Button invocarButton;
    public Button exorcizarButton;

    [Header("Highlight")]
    public Material highlightMaterial;

private int ectoplasmas = 10;
private const int costoInvocacion = 2;
private int nextSpawnIndex = 0; 
private int nextPersonajeIndex = 0;

    private Transform hoveredShape;
    private Transform clickedShape;
    private Transform selectedShape;
    private Renderer moveRenderer;
    private Material moveOriginalMat;
    private bool isMoving = false;

    private Vector3 spawnOffset = new Vector3(0, 0.5f, 0);

    // ---------------- Start ----------------
    private void Start()
    {
        deleteButton?.gameObject.SetActive(false);
        cancelButton?.gameObject.SetActive(false);
        intercambioText?.SetActive(false);

        deleteButton?.onClick.AddListener(DeleteTargetShape);
        cancelButton?.onClick.AddListener(CancelSelection);
        exorcizarButton?.onClick.AddListener(ExorcizarFantasma);

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

    // ---------------- Invocar ----------------
public void InvocarPersonaje()
{
    if (ectoplasmas < costoInvocacion)
    {
        Debug.Log("⚠ No tienes suficientes ectoplasmas para invocar.");
        return;
    }

    // Buscar la siguiente caja vacía
    Transform cajaVacía = null;
    int checkedBoxes = 0;
    int totalBoxes = spawnPoints.Length;

    while (checkedBoxes < totalBoxes)
    {
        Transform box = spawnPoints[nextSpawnIndex];

        bool ocupado = false;
        foreach (Transform child in box)
            if (child.CompareTag("Shape")) { ocupado = true; break; }

        if (!ocupado)
        {
            cajaVacía = box;
            break;
        }

        nextSpawnIndex = (nextSpawnIndex + 1) % totalBoxes;
        checkedBoxes++;
    }

    if (cajaVacía == null)
    {
        Debug.Log("⚠ Todas las cajitas están ocupadas!");
        return; // No gastar ectoplasmas si no hay lugar
    }

    // Elegir personaje en orden
    Personaje elegido = personajes[nextPersonajeIndex];
    nextPersonajeIndex = (nextPersonajeIndex + 1) % personajes.Length;

    // Instanciar prefab
    GameObject spawned = Instantiate(elegido.prefab);
    spawned.transform.SetParent(GameManager.Instance.transform);
    spawned.transform.position = cajaVacía.position + spawnOffset;
    spawned.tag = "Shape";

    if (spawned.GetComponent<Collider>() == null)
        spawned.AddComponent<BoxCollider>();

    Renderer rend = spawned.GetComponent<Renderer>();
    if (rend != null)
        rend.material = new Material(rend.material);

    // Restar ectoplasmas y actualizar UI
    ectoplasmas -= costoInvocacion;
    UpdateEctoplasmaUI();
    GuardarEstado();

    // Avanzar índice para la próxima caja
    nextSpawnIndex = (nextSpawnIndex + 1) % totalBoxes;
}


    // ---------------- Guardar / Restaurar ----------------
    public void GuardarEstado()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.ectoplasmas = ectoplasmas;
        GameManager.Instance.personajesInvocados.Clear();

        foreach (Transform child in GameManager.Instance.transform)
        {
            if (child.CompareTag("Shape"))
            {
                // Guardamos el nombre de prefab y la caja más cercana
                Transform closestBox = null;
                float minDist = Mathf.Infinity;
                foreach (Transform box in spawnPoints)
                {
                    float dist = Vector3.Distance(child.position, box.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closestBox = box;
                    }
                }

                GameManager.Instance.personajesInvocados.Add(new GameManager.PersonajeData
                {
                    nombre = child.name.Replace("(Clone)", ""),
                    posicion = child.position,
                    parentName = closestBox != null ? closestBox.name : ""
                });
            }
        }
    }

private void SwapShapes(Transform shapeA, Transform shapeB)
{
    if (shapeA == null || shapeB == null) return;

    // Guardar posiciones
    Vector3 posA = shapeA.position;
    Vector3 posB = shapeB.position;

    // Intercambiar
    shapeA.position = posB;
    shapeB.position = posA;

    // Opcional: mantener ambos bajo GameManager
    shapeA.SetParent(GameManager.Instance.transform);
    shapeB.SetParent(GameManager.Instance.transform);

    // Colores de cajas (si las cajas tienen renderer)
    foreach (Transform box in spawnPoints)
    {
        Renderer rend = box.GetComponent<Renderer>();
        if (rend == null) continue;

        if (Vector3.Distance(box.position, shapeA.position) < 0.1f || Vector3.Distance(box.position, shapeB.position) < 0.1f)
            rend.material.color = Color.gray; // ocupada
        else
            rend.material.color = Color.white; // libre
    }

    ClearMoveSelection();
    clickedShape = shapeA;

    Renderer rendShape = clickedShape.GetComponent<Renderer>();
    if (rendShape != null) rendShape.material = highlightMaterial;

    GuardarEstado();
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

            // Evitar duplicados
            bool yaExiste = false;
            foreach (Transform child in GameManager.Instance.transform)
                if (child.name.Contains(data.nombre)) yaExiste = true;
            if (yaExiste) continue;

            // Instanciar prefab
            GameObject spawned = Instantiate(prefab.prefab);
            spawned.transform.SetParent(GameManager.Instance.transform);
            spawned.transform.position = data.posicion;
            spawned.tag = "Shape";

            if (spawned.GetComponent<Collider>() == null)
                spawned.AddComponent<BoxCollider>();

            Renderer rend = spawned.GetComponent<Renderer>();
            if (rend != null) rend.material = new Material(rend.material);
        }
    }

    // ---------------- UI y ectoplasmas ----------------
    private void UpdateEctoplasmaUI()
    {
        if (ectoplasmaText != null)
            ectoplasmaText.text = "Ectoplasmas: " + ectoplasmas;

        if (invocarButton != null)
            invocarButton.interactable = ectoplasmas >= costoInvocacion;
    }

    public void ExorcizarFantasma()
    {
        ectoplasmas += 2;
        UpdateEctoplasmaUI();
        GuardarEstado();
        Debug.Log("Exorcizaste un fantasma. +2 ectoplasmas!");
    }

    // ---------------- Hover, Click y Mover ----------------
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
            if (selectedShape != null && hitTransform != selectedShape)
            {
                // Hacer swap entre selectedShape y el que clickeaste
                SwapShapes(selectedShape, hitTransform);
                return;
            }

            if (clickedShape != hitTransform) SelectShape(hitTransform);
            else StartExchangeMode(hitTransform);
        }
        else if (isMoving)
        {
            foreach (Transform box in spawnPoints)
                if (hitTransform == box || hitTransform.IsChildOf(box))
                {
                    MoveShapeToBox(box);
                    return;
                }
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
        intercambioText?.SetActive(false);
    }

    private void StartExchangeMode(Transform shape)
    {
        if (!shape.CompareTag("Shape")) return;

        selectedShape = shape;
        isMoving = true;

        moveRenderer = shape.GetComponent<Renderer>();
        if (moveRenderer != null)
        {
            moveOriginalMat = moveRenderer.material;
            moveRenderer.material = highlightMaterial;
        }

        clickedShape = null;
        deleteButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        intercambioText?.SetActive(true);
    }


private void MoveShapeToBox(Transform targetBox)
{
    if (selectedShape == null || targetBox == null) return;

    // Caja original del personaje seleccionado
    Transform originalBox = null;
    foreach (Transform box in spawnPoints)
    {
        if (Vector3.Distance(selectedShape.position, box.position) < 0.1f)
        {
            originalBox = box;
            break;
        }
    }

    // Ver si hay un personaje en la caja destino
    Transform targetShape = null;
    foreach (Transform child in targetBox)
        if (child.CompareTag("Shape"))
        {
            targetShape = child;
            break;
        }

    if (targetShape != null)
    {
        // Swap real
        Vector3 selPos = selectedShape.position;
        Vector3 targetPos = targetShape.position;

        selectedShape.position = targetPos;
        targetShape.position = selPos;

        selectedShape.SetParent(GameManager.Instance.transform);
        targetShape.SetParent(GameManager.Instance.transform);

        // Colores de cajas
        if (originalBox != null)
        {
            Renderer origRend = originalBox.GetComponent<Renderer>();
            if (origRend != null) origRend.material.color = Color.gray;
        }

        Renderer targetRend = targetBox.GetComponent<Renderer>();
        if (targetRend != null) targetRend.material.color = Color.gray;
    }
    else
    {
        // Caja destino vacía
        selectedShape.position = targetBox.position + spawnOffset;
        selectedShape.SetParent(GameManager.Instance.transform);

        // Colores
        if (originalBox != null)
        {
            Renderer origRend = originalBox.GetComponent<Renderer>();
            if (origRend != null) origRend.material.color = Color.white;
        }

        Renderer targetRend = targetBox.GetComponent<Renderer>();
        if (targetRend != null) targetRend.material.color = Color.gray;
    }

    ClearMoveSelection();

    clickedShape = selectedShape;
    Renderer rend = clickedShape.GetComponent<Renderer>();
    if (rend != null) rend.material = highlightMaterial;

    deleteButton.gameObject.SetActive(true);
    cancelButton.gameObject.SetActive(true);

    GuardarEstado();

    // Recalcular nextSpawnIndex
    nextSpawnIndex = 0;
    while (nextSpawnIndex < spawnPoints.Length)
    {
        bool ocupado = false;
        foreach (Transform child in spawnPoints[nextSpawnIndex])
            if (child.CompareTag("Shape")) { ocupado = true; break; }

        if (!ocupado) break;
        nextSpawnIndex++;
    }
}



    private void DeleteTargetShape()
{
    if (clickedShape != null && clickedShape.CompareTag("Shape"))
    {
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
                boxRend.material.color = Color.white; // Caja libre
        }

        Renderer rend = clickedShape.GetComponent<Renderer>();
        if (rend != null && moveOriginalMat != null)
            rend.material = moveOriginalMat;

        Destroy(clickedShape.gameObject);
        ClearDeleteSelection();
        GuardarEstado();
    }
}

    private void CancelSelection()
    {
        ClearDeleteSelection();
        ClearMoveSelection();
    }

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
        intercambioText?.SetActive(false);
    }

    private void ClearMoveSelection()
    {
        if (moveRenderer != null && moveOriginalMat != null)
            moveRenderer.material = moveOriginalMat;

        selectedShape = null;
        moveRenderer = null;
        isMoving = false;

        intercambioText?.SetActive(false);
    }
}
