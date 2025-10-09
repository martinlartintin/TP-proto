using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Nueva clase para definir los personajes disponibles
[System.Serializable]
public class Personaje
{
    public string nombre;
    public GameObject prefab;
    [Range(0, 100)] public int probabilidad;
}

public class RandomShapeSpawner : MonoBehaviour
{
    // Lista de personajes con sus probabilidades
    [Header("Personajes disponibles")]
    public Personaje[] personajes = new Personaje[]
    {
        new Personaje { nombre = "La Locomotora", probabilidad = 10 },
        new Personaje { nombre = "Freddie Mercury", probabilidad = 30 },
        new Personaje { nombre = "Manuel Belgrano", probabilidad = 60 }
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

    private Transform hoveredShape;
    private Transform clickedShape;
    private Transform selectedShape;
    private Renderer moveRenderer;
    private Material moveOriginalMat;
    private bool isMoving = false;

    private Vector3 spawnOffset = new Vector3(0, 0.5f, 0);

    void Start()
    {
        deleteButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);

        if (intercambioText != null)
            intercambioText.SetActive(false);

        deleteButton.onClick.AddListener(DeleteTargetShape);
        cancelButton.onClick.AddListener(CancelSelection);

        if (exorcizarButton != null)
            exorcizarButton.onClick.AddListener(ExorcizarFantasma);

        UpdateEctoplasmaUI();
    }

    void Update()
    {
        HandleMouseHover();
        HandleMouseClick();
    }

    // Método actualizado: invoca personajes según probabilidad
    public void InvocarPersonaje()
    {
        if (ectoplasmas < costoInvocacion)
        {
            Debug.Log(" No tienes suficientes ectoplasmas para invocar.");
            return;
        }

        ectoplasmas -= costoInvocacion;
        UpdateEctoplasmaUI();

        int freeIndex = -1;
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            bool ocupado = false;
            foreach (Transform child in spawnPoints[i])
            {
                if (child.CompareTag("Shape"))
                {
                    ocupado = true;
                    break;
                }
            }
            if (!ocupado)
            {
                freeIndex = i;
                break;
            }
        }

        if (freeIndex == -1)
        {
            Debug.Log("⚠ Todas las cajitas están ocupadas!");
            return;
        }

        // Selección aleatoria basada en probabilidad
        int totalProb = 0;
        foreach (var p in personajes)
            totalProb += p.probabilidad;

        int rand = Random.Range(0, totalProb);
        Personaje elegido = null;
        int acumulado = 0;

        foreach (var p in personajes)
        {
            acumulado += p.probabilidad;
            if (rand < acumulado)
            {
                elegido = p;
                break;
            }
        }

        if (elegido == null)
        {
            Debug.LogError("No se pudo elegir personaje.");
            return;
        }

        GameObject spawned = Instantiate(elegido.prefab);
        Renderer rend = spawned.GetComponent<Renderer>();
        if (rend != null)
            rend.material = new Material(rend.material);

        spawned.transform.position = spawnPoints[freeIndex].position + spawnOffset;
        spawned.tag = "Shape";
        if (spawned.GetComponent<Collider>() == null)
            spawned.AddComponent<BoxCollider>();

        spawned.transform.SetParent(spawnPoints[freeIndex]);
        spawned.transform.localPosition = spawnOffset;

        Debug.Log($"Invocaste a {elegido.nombre}!");
    }

    // --- HOVER ---
    private void HandleMouseHover()
    {
        if (isMoving) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform hitTransform = hit.transform;

            if (hitTransform.CompareTag("Shape") && IsChildOfAnyBox(hitTransform))
            {
                if (hoveredShape != null && hoveredShape != hitTransform)
                    ClearHover();

                hoveredShape = hitTransform;
                Renderer rend = hoveredShape.GetComponent<Renderer>();
                if (rend != null && hoveredShape != clickedShape)
                    rend.material = highlightMaterial;
            }
            else
            {
                ClearHover();
            }
        }
        else
        {
            ClearHover();
        }
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

    // --- CLICK ---
    private void HandleMouseClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform hitTransform = hit.transform;

            if (hitTransform.CompareTag("Shape") && IsChildOfAnyBox(hitTransform))
            {
                if (selectedShape != null)
                {
                    foreach (Transform box in spawnPoints)
                    {
                        if (hitTransform == box || hitTransform.IsChildOf(box))
                        {
                            MoveShapeToBox(box);
                            return;
                        }
                    }
                }

                if (clickedShape != hitTransform)
                {
                    SelectShape(hitTransform);
                }
                else
                {
                    StartExchangeMode(hitTransform);
                }
            }
            else if (isMoving)
            {
                foreach (Transform box in spawnPoints)
                {
                    if (hitTransform == box || hitTransform.IsChildOf(box))
                    {
                        MoveShapeToBox(box);
                        return;
                    }
                }
            }
        }
    }

    private void SelectShape(Transform shape)
    {
        if (!shape.CompareTag("Shape") || !IsChildOfAnyBox(shape)) return;

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

        if (intercambioText != null)
            intercambioText.SetActive(false);
    }

    private void StartExchangeMode(Transform shape)
    {
        if (!shape.CompareTag("Shape") || !IsChildOfAnyBox(shape)) return;

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

        if (intercambioText != null)
            intercambioText.SetActive(true);
    }

    private void MoveShapeToBox(Transform targetBox)
    {
        if (selectedShape == null) return;

        Transform existingShape = null;
        foreach (Transform child in targetBox)
        {
            if (child.CompareTag("Shape"))
            {
                existingShape = child;
                break;
            }
        }

        Transform originalParent = selectedShape.parent;
        Vector3 originalPosition = selectedShape.localPosition;

        if (existingShape != null)
        {
            existingShape.SetParent(originalParent);
            existingShape.localPosition = originalPosition;
        }

        selectedShape.SetParent(targetBox);
        selectedShape.localPosition = spawnOffset;

        ClearMoveSelection();

        if (targetBox.childCount > 0)
            clickedShape = targetBox.GetChild(0);
        else
            clickedShape = selectedShape;

        Renderer rend = clickedShape.GetComponent<Renderer>();
        if (rend != null)
            rend.material = highlightMaterial;

        deleteButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
    }

    private void DeleteTargetShape()
    {
        if (clickedShape != null && clickedShape.CompareTag("Shape") && IsChildOfAnyBox(clickedShape))
        {
            Renderer rend = clickedShape.GetComponent<Renderer>();
            if (rend != null && moveOriginalMat != null)
                rend.material = moveOriginalMat;

            GameObject shapeToDestroy = clickedShape.gameObject;
            ClearDeleteSelection();
            Destroy(shapeToDestroy);
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

        if (!isMoving && intercambioText != null)
            intercambioText.SetActive(false);
    }

    private void ClearMoveSelection()
    {
        if (moveRenderer != null && moveOriginalMat != null)
            moveRenderer.material = moveOriginalMat;

        selectedShape = null;
        moveRenderer = null;
        isMoving = false;

        if (intercambioText != null)
            intercambioText.SetActive(false);
    }

    private bool IsChildOfAnyBox(Transform shape)
    {
        foreach (Transform box in spawnPoints)
        {
            if (shape.IsChildOf(box))
                return true;
        }
        return false;
    }

    // --- Sistema de ectoplasmas ---
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
        Debug.Log("Exorcizaste un fantasma. +2 ectoplasmas!");
    }
}

