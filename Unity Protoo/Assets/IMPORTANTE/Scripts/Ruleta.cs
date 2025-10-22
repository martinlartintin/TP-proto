using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // Para TMP_Text

public class Ruleta : MonoBehaviour
{
    [Header("Ruleta")]
    public RectTransform ruletaTransform;
    public float velocidadGiro = 1000f;
    public float desaceleracion = 100f;
    private bool girando = false;

    [Header("Referencias")]
    public RandomShapeSpawner spawnerRef;
    public Button girarButton;
    public TMP_Text ectoplasmaText;

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

        if (GameManagerPersistente.Instancia.ectoplasma < GameManagerPersistente.Instancia.costoPorTirada)
        {
            Debug.Log("⚠ No tienes ectoplasma suficiente para girar la ruleta.");
            return;
        }

        // Gastar ectoplasma
        GameManagerPersistente.Instancia.ectoplasma -= GameManagerPersistente.Instancia.costoPorTirada;
        ActualizarUI();

        velocidadGiro = Random.Range(1000f, 2000f);
        girando = true;

        // Bloquear botón mientras gira
        girarButton.interactable = false;
    }

    private void DeterminarResultado()
    {
        float angulo = ruletaTransform.eulerAngles.z % 360f;
        Rareza resultado = Rareza.Comun;
        if (angulo >= 0 && angulo < 120f) resultado = Rareza.Comun;
        else if (angulo >= 120f && angulo < 240f) resultado = Rareza.Epico;
        else resultado = Rareza.Legendario;

        if (spawnerRef == null || spawnerRef.personajes == null || spawnerRef.personajes.Length == 0) return;

        List<Personaje> lista = new List<Personaje>();
        foreach (var p in spawnerRef.personajes)
            if (p.rareza == resultado) lista.Add(p);

        if (lista.Count == 0) return;

        Personaje elegido = lista[Random.Range(0, lista.Count)];
        GameManager.Instance.personajesInvocados.Add(new GameManager.PersonajeData
        {
            nombre = elegido.nombre,
            rareza = elegido.rareza
        });

        Debug.Log($"💀 Guardado {elegido.nombre} ({resultado}) para aparecer en Main.");

        // Cargar Main
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
    }

    // Se llama desde RandomShapeSpawner después de instanciar un fantasma
    public void HabilitarRuleta()
    {
        girando = false;
        ActualizarUI();
    }

    private void ActualizarUI()
    {
        if (ectoplasmaText != null)
            ectoplasmaText.text = $"Ectoplasma: {GameManagerPersistente.Instancia.ectoplasma}";

        if (girarButton != null)
            girarButton.interactable = !girando &&
                                      GameManagerPersistente.Instancia.ectoplasma >= GameManagerPersistente.Instancia.costoPorTirada;
    }
}
