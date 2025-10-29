using System.Collections.Generic;
using UnityEngine;

public class GameManagerPersistente : MonoBehaviour
{
    public static GameManagerPersistente Instancia;

    [Header("Datos de fantasmas")]
    public List<PersonajeData> fantasmasDesbloqueados = new List<PersonajeData>(); // 🔹 ESTA es la lista que busca el otro script
    [HideInInspector] public PersonajeData fantasmaSeleccionado;                   // 🔹 El fantasma elegido

    [Header("Ectoplasma")]
    public int ectoplasma = 10;          // Cantidad inicial
    public int costoPorTirada = 2;       // Cuánto gasta cada tirada

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetearFantasmas()
    {
        fantasmasDesbloqueados.Clear();
    }
}
