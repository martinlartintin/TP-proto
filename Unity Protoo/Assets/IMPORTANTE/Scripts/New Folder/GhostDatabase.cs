using UnityEngine;

[CreateAssetMenu(fileName = "GhostDatabase", menuName = "Juego/GhostDatabase")]
public class GhostDatabase : ScriptableObject
{
    public PersonajeData[] fantasmas;
}

[System.Serializable]
public class Fantasma
{
    public string nombre;
    public Rareza rareza;
    public GameObject prefab;

    [Header("Ataques del Fantasma")]
    public Attack[] attacks;
}
