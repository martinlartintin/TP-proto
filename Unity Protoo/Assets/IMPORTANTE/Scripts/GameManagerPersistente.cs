using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FantasmaData
{
    public string nombre;
    public Rareza rareza;
    public string tumbaName; // Guarda la referencia de la tumba
}

public class GameManagerPersistente : MonoBehaviour
{
    public static GameManagerPersistente Instancia;

    public List<FantasmaData> fantasmasGuardados = new List<FantasmaData>();

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
        fantasmasGuardados.Clear();
    }
}
