using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SeleccionFantasmaManager : MonoBehaviour
{
    [Header("UI")]
    public Transform panelBotones;
    public GameObject botonPrefab;
    public TMP_Text fantasmaSeleccionadoText;

    private FantasmaData fantasmaSeleccionado;

    void Start()
    {
        CrearBotones();
    }

    void CrearBotones()
    {
        foreach (Transform hijo in panelBotones)
            Destroy(hijo.gameObject);

        var desbloqueados = GameManagerPersistente.Instancia.fantasmasDesbloqueados;
        int cantidad = desbloqueados.Count;

        Debug.Log($"[SeleccionFantasmaManager] Fantasmas desbloqueados: {cantidad}");

        for (int i = 0; i < 6; i++)
        {
            GameObject boton = Instantiate(botonPrefab, panelBotones);
            TMP_Text texto = boton.GetComponentInChildren<TMP_Text>();
            Button botonComponente = boton.GetComponent<Button>();

            RectTransform rt = boton.GetComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;

            if (cantidad > 0 && i < cantidad)
            {
                FantasmaData fantasma = desbloqueados[i % cantidad];
                texto.text = $"{fantasma.nombre} ({fantasma.rareza})";

                botonComponente.interactable = true;

                // Usar variable local para evitar problemas de captura
                FantasmaData f = fantasma;
                botonComponente.onClick.AddListener(() =>
                {
                    fantasmaSeleccionado = f;
                    fantasmaSeleccionadoText.text = $"Seleccionado: {f.nombre}";
                });
            }
            else
            {
                texto.text = "??? (Bloqueado)";
                botonComponente.interactable = false;
            }
        }
    }

    public void ConfirmarSeleccion()
    {
        if (fantasmaSeleccionado == null)
        {
            Debug.LogWarning("No seleccionaste ningún fantasma.");
            return;
        }

        GameManagerPersistente.Instancia.fantasmaSeleccionado = fantasmaSeleccionado;
        Debug.Log($"{fantasmaSeleccionado.nombre}");

        SceneManager.LoadScene("EscenaNoche");
    }
}
