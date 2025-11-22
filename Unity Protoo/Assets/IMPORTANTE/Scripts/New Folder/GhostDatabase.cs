using UnityEngine;

[CreateAssetMenu(fileName = "GhostDatabase", menuName = "Juego/GhostDatabase")]
public class GhostDatabase : ScriptableObject
{
    public GhostEntry[] ghosts;
}

[System.Serializable]
public class GhostEntry
{
    public string nombre;
    public Rareza rareza;
    public GameObject prefab;

    [Header("Ataques del Fantasma")]
    public Attack[] attacks;
}
