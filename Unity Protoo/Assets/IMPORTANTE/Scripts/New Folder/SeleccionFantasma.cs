using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

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
        if (GameManagerPersistente.Instancia == null || 
            GameManagerPersistente.Instancia.fantasmasDesbloqueados.Count == 0)
        {
            Debug.Log("No hay fantasmas desbloqueados");
            return;
        }

        foreach (var f in GameManagerPersistente.Instancia.fantasmasDesbloqueados)
        {
            GameObject boton = Instantiate(botonPrefab, panelBotones);
            TMP_Text texto = boton.GetComponentInChildren<TMP_Text>();
            texto.text = $"{f.nombre} ({f.rareza})";

            FantasmaData fantasma = f;
            boton.GetComponent<Button>().onClick.AddListener(() =>
            {
                fantasmaSeleccionado = fantasma;
                fantasmaSeleccionadoText.text = $"Seleccionado: {fantasma.nombre}";
            });
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
