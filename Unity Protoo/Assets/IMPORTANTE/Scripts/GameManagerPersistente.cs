using System.Collections.Generic;
using UnityEngine;

public class GameManagerPersistente : MonoBehaviour
{
    public static GameManagerPersistente Instancia;

    public List<FantasmaData> fantasmasGuardados = new List<FantasmaData>();
    public List<PersonajeData> personajesInvocados = new List<PersonajeData>();

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
    }

    public void ResetearFantasmas()
    {
        fantasmasGuardados.Clear();
        personajesInvocados.Clear();
    }
}
