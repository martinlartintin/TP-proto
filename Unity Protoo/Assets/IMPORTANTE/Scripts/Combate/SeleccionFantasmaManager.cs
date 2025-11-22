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

    private PersonajeData fantasmaSeleccionado;

    void Start()
    {
        CrearBotones();
    }

    void CrearBotones()
    {
        foreach (Transform hijo in panelBotones)
            Destroy(hijo.gameObject);

        var desbloqueados = GameManagerPersistente.Instancia.ghostsDesbloqueados;
        int cantidad = desbloqueados.Count;

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
                PersonajeData f = desbloqueados[i % cantidad];
                texto.text = $"{f.nombre} ({f.rareza})";

                botonComponente.interactable = true;
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
        if (fantasmaSeleccionado == null) return;

        GameManagerPersistente.Instancia.ghostSeleccionado = fantasmaSeleccionado;
        SceneManager.LoadScene("EscenaNoche");
    }
}
