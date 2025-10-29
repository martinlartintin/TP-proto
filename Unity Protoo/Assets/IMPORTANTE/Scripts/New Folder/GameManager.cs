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
        public Rareza rareza;
        public GameObject prefab; // Necesario para instanciar en la escena
    }

    public List<PersonajeData> personajesInvocados = new List<PersonajeData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
}
