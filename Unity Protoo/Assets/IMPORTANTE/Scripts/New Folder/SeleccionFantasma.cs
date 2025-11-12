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

        for (int i = 0; i < 6; i++)
        {
            GameObject boton = Instantiate(botonPrefab, panelBotones);
            TMP_Text texto = boton.GetComponentInChildren<TMP_Text>();
            Button botonComponente = boton.GetComponent<Button>();

            // Posición para 2 filas x 3 columnas (ejemplo simple)
            RectTransform rt = boton.GetComponent<RectTransform>();
            int fila = i / 3;
            int columna = i % 3;
            rt.anchoredPosition = new Vector2(columna * 150 - 150, -fila * 60); // ajustar según tamaño del botón

            if (cantidad > 0 && i < cantidad)
            {
                FantasmaData f = desbloqueados[i % cantidad];
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
        if (fantasmaSeleccionado == null)
        {
            Debug.LogWarning("No seleccionaste ningún fantasma.");
            return;
        }

        GameManagerPersistente.Instancia.fantasmaSeleccionado = fantasmaSeleccionado;
        Debug.Log($"Fantasma {fantasmaSeleccionado.nombre} seleccionado para combate!");

        if (fantasmaSeleccionado.personaje != null && fantasmaSeleccionado.personaje.prefab != null)
        {
            Transform spawn = GameObject.Find("PlayerSpawnPoint")?.transform;
            Vector3 pos = spawn != null ? spawn.position : Vector3.zero;

            GameObject jugador = Instantiate(fantasmaSeleccionado.personaje.prefab, pos, Quaternion.identity);
            jugador.name = fantasmaSeleccionado.nombre;
            jugador.tag = "Player";
        }

        SceneManager.LoadScene("EscenaNoche");
    }
}
