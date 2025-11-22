using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class Ruleta : MonoBehaviour
{
    [Header("Ruleta")]
    public RectTransform ruletaTransform;
    public float velocidadGiro = 400f;
    public float desaceleracion = 800f;
    private bool girando = false;

    [Header("UI")]
    public Button girarButton;
    public TMP_Text ectoplasmaText;

    [Header("Personajes disponibles")]
    public List<PersonajeData> personajesDisponibles = new List<PersonajeData>();

    private void Start()
    {
        if (girarButton != null)
            girarButton.onClick.AddListener(GirarRuleta);

        ActualizarUI();
    }

    private void Update()
    {
        if (!girando) return;

        ruletaTransform.Rotate(0, 0, velocidadGiro * Time.deltaTime);
        velocidadGiro -= desaceleracion * Time.deltaTime;

        if (velocidadGiro <= 0)
        {
            girando = false;
            velocidadGiro = 0;
            DeterminarResultado();
        }
    }

    public void GirarRuleta()
    {
        if (girando) return;

        if (GameManagerPersistente.Instancia == null)
        {
            Debug.LogError("GameManagerPersistente no encontrado. Asegurate de tenerlo en la escena inicial.");
            return;
        }

        if (GameManagerPersistente.Instancia.ectoplasma < GameManagerPersistente.Instancia.costoPorTirada)
        {
            Debug.Log("No tienes ectoplasma suficiente para girar la ruleta.");
            return;
        }

        GameManagerPersistente.Instancia.ectoplasma -= GameManagerPersistente.Instancia.costoPorTirada;
        ActualizarUI();

        velocidadGiro = Random.Range(1000f, 2000f);
        girando = true;

        if (girarButton != null)
            girarButton.interactable = false;
    }

    private void DeterminarResultado()
    {
        if (personajesDisponibles == null || personajesDisponibles.Count == 0)
        {
            Debug.LogError("Lista 'personajesDisponibles' vacía. Configurala en el Inspector.");
            if (girarButton != null) girarButton.interactable = true;
            return;
        }

        float angulo = ruletaTransform.eulerAngles.z % 360f;
        Rareza resultado = Rareza.Comun;

        if (angulo >= 0 && angulo < 120f) resultado = Rareza.Comun;
        else if (angulo >= 120f && angulo < 240f) resultado = Rareza.Epico;
        else resultado = Rareza.Legendario;

        Debug.Log($"🎯 Resultado de la ruleta: {resultado}");

        List<PersonajeData> lista = personajesDisponibles
            .Where(p => p.rareza == resultado)
            .ToList();

        if (lista.Count == 0)
        {
            Debug.LogWarning($"No hay personajes de rareza {resultado}. Se seleccionará uno de todos los disponibles.");
            lista = new List<PersonajeData>(personajesDisponibles);
        }

        if (lista.Count == 0)
        {
            Debug.LogError("No hay personajes configurados en 'personajesDisponibles'. No se puede continuar.");
            if (girarButton != null) girarButton.interactable = true;
            return;
        }

        PersonajeData elegido = lista[Random.Range(0, lista.Count)];
        FantasmaData fantasma = new FantasmaData
        {
            nombre = elegido.nombre,
            rareza = elegido.rareza,
            prefab = elegido.prefab
        };

        GameManagerPersistente.Instancia.fantasmasDesbloqueados.Add(fantasma);
        GameManagerPersistente.Instancia.fantasmaSeleccionado = fantasma;

        Debug.Log($"✨ Fantasma obtenido: {fantasma.nombre} ({fantasma.rareza})");

        SceneManager.LoadScene("Main");
    }

    private void ActualizarUI()
    {
        if (GameManagerPersistente.Instancia == null) return;

        if (ectoplasmaText != null)
            ectoplasmaText.text = $"Ectoplasma: {GameManagerPersistente.Instancia.ectoplasma}";

        if (girarButton != null)
            girarButton.interactable = !girando &&
                GameManagerPersistente.Instancia.ectoplasma >= GameManagerPersistente.Instancia.costoPorTirada;
    }
}