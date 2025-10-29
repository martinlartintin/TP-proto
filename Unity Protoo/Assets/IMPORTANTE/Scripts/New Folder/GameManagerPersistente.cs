using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FantasmaData
{
    public string nombre;
    public Rareza rareza;
    public string tumbaName; // opcional, solo para combate
}

public class GameManagerPersistente : MonoBehaviour
{
    public static GameManagerPersistente Instancia;

    [Header("Datos de fantasmas")]
    public List<FantasmaData> fantasmasDesbloqueados = new List<FantasmaData>();
    [HideInInspector] public FantasmaData fantasmaSeleccionado;

    [Header("Ectoplasma")]
    public int ectoplasma = 10;
    public int costoPorTirada = 2;

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

        // TEMPORAL: agregar fantasmas de prueba
        if(fantasmasDesbloqueados.Count == 0)
        {
            fantasmasDesbloqueados.Add(new FantasmaData { nombre = "FantasmaRojo", rareza = Rareza.Epico });
            fantasmasDesbloqueados.Add(new FantasmaData { nombre = "FantasmaAzul", rareza = Rareza.Comun });
        }
    }

    public void ResetearFantasmas()
    {
        fantasmasDesbloqueados.Clear();
    }
}
