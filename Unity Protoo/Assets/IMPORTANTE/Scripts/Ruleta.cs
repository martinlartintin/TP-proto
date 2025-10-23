using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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
    public SceneChangerNight volverScript;
    public TMP_Text ectoplasmaText;

    private readonly float probComun = 0.6f;
    private readonly float probEpico = 0.3f;
    private readonly float probLegendario = 0.1f;

// Ejemplo basado en tus medidas
    private readonly float anguloAmarillo = 36f;  // Epico
    private readonly float anguloMorado = 108f;  // Epico
    private readonly float anguloVerde = 216f;   // Comun

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
            Debug.Log("⚠️ No tienes ectoplasma suficiente para girar la ruleta.");
            return;
        }

        GameManagerPersistente.Instancia.ectoplasma -= GameManagerPersistente.Instancia.costoPorTirada;
        ActualizarUI();

        velocidadGiro = Random.Range(1000f, 2000f);
        girando = true;

        if (girarButton != null) girarButton.interactable = false;
        if (volverScript != null) volverScript.DesactivarBoton();
    }

    private void DeterminarResultado()
    {
        float rand = Random.value;
        Rareza resultado;

        if (rand < probComun)
            resultado = Rareza.Comun;
        else if (rand < probComun + probEpico)
            resultado = Rareza.Epico;
        else
            resultado = Rareza.Legendario;

        float anguloFinal = 0f;

       switch (resultado)
       {
        case Rareza.Comun:
         // Verde: 0° a 216°
            anguloFinal = Random.Range(0f, anguloVerde);
                 break;
            case Rareza.Epico:
            // Amarillo + Morado: 216° a 360°
            anguloFinal = Random.Range(anguloVerde, 360f);
              break;
          case Rareza.Legendario:
        // Si hay legendario, asigna su rango aquí
             anguloFinal = 0f; // ajusta según tu ruleta
             break;
        }

        ruletaTransform.eulerAngles = new Vector3(0, 0, anguloFinal);

        if (spawnerRef != null && spawnerRef.personajes.Length > 0)
        {
            List<Personaje> lista = new List<Personaje>();
            foreach (var p in spawnerRef.personajes)
                if (p.rareza == resultado)
                    lista.Add(p);

            if (lista.Count > 0)
            {
                Personaje elegido = lista[Random.Range(0, lista.Count)];

                // Guardar personaje invocado
                GameManagerPersistente.Instancia.personajesInvocados.Add(new PersonajeData
                {
                    nombre = elegido.nombre,
                    rareza = elegido.rareza,
                    parentName = "",        // se asigna en RandomShapeSpawner
                    localPosition = Vector3.zero
                });

                Debug.Log($"💀 Guardado {elegido.nombre} ({resultado}) para aparecer en Main.");
            }
        }

        if (volverScript != null) volverScript.ActivarBoton();
        if (girarButton != null) girarButton.interactable = true;

        SceneManager.LoadScene("Main");
    }

    private void ActualizarUI()
    {
        if (ectoplasmaText != null)
            ectoplasmaText.text = $"Ectoplasma: {GameManagerPersistente.Instancia.ectoplasma}";
    }
}
