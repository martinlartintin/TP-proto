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

    [Header("Todos los fantasmas posibles")]
    public List<FantasmaData> todosLosFantasmas = new List<FantasmaData>();

    private FantasmaData fantasmaSeleccionado;

    void Start()
    {
        CrearBotones();
    }

    void CrearBotones()
    {
        foreach (Transform hijo in panelBotones)
            Destroy(hijo.gameObject);

        if (todosLosFantasmas.Count == 0)
        {
            Debug.LogWarning(" No hay fantasmas definidos en la lista todosLosFantasmas.");
            return;
        }

        for (int i = 0; i < todosLosFantasmas.Count && i < 6; i++)
        {
            FantasmaData fantasma = todosLosFantasmas[i];
            bool desbloqueado = GameManagerPersistente.Instancia.fantasmasDesbloqueados.Contains(fantasma);

            GameObject boton = Instantiate(botonPrefab, panelBotones);
            TMP_Text texto = boton.GetComponentInChildren<TMP_Text>();

            if (desbloqueado)
            {
                texto.text = $"{fantasma.nombre} ({fantasma.rareza})";
                boton.GetComponent<Button>().interactable = true;
                boton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    fantasmaSeleccionado = fantasma;
                    fantasmaSeleccionadoText.text = $"Seleccionado: {fantasma.nombre}";
                });
            }
            else
            {
                texto.text = "??? (Bloqueado)";
                boton.GetComponent<Button>().interactable = false;
            }
        }
    }

    public void ConfirmarSeleccion()
    {
        if (fantasmaSeleccionado == null)
        {
            Debug.LogWarning(" No seleccionaste ningún fantasma.");
            return;
        }

        GameManagerPersistente.Instancia.fantasmaSeleccionado = fantasmaSeleccionado;
        Debug.Log($" Fantasma {fantasmaSeleccionado.nombre} seleccionado para combate!");

        SceneManager.LoadScene("EscenaNoche");
    }
}
