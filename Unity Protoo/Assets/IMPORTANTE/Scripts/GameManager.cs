using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int ectoplasmas = 10;

    [System.Serializable]
    public class PersonajeData
    {
        public string nombre;
        public Vector3 posicion;
        public string parentName;
    }

    public List<PersonajeData> personajesInvocados = new List<PersonajeData>();

   private void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject); // <-- importante
    }
    else
    {
        Destroy(gameObject);
    }
}
    
}
