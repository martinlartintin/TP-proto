using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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

            if (cantidad > 0 && i < cantidad)
            {
                FantasmaData fantasma = desbloqueados[i % cantidad];
                texto.text = $"{fantasma.nombre} ({fantasma.rareza})";

                botonComponente.interactable = true;
                botonComponente.onClick.AddListener(() =>
                {
                    fantasmaSeleccionado = fantasma;
                    fantasmaSeleccionadoText.text = $"Seleccionado: {fantasma.nombre}";
                });
            }
            else if (cantidad > 0)
            {
                FantasmaData fantasma = desbloqueados[i % cantidad];
                texto.text = $"{fantasma.nombre} ({fantasma.rareza})";

                botonComponente.interactable = true;
                botonComponente.onClick.AddListener(() =>
                {
                    fantasmaSeleccionado = fantasma;
                    fantasmaSeleccionadoText.text = $"Seleccionado: {fantasma.nombre}";
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
        Debug.Log($"Fantasma {fantasmaSeleccionado.nombre} seleccionado para combate!");

        SceneManager.LoadScene("EscenaNoche");
    }
}
