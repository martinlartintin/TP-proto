using UnityEngine;

public enum Rareza
{
    Comun,
    Epico,
    Legendario
}

[System.Serializable]
public class Personaje
{
    public string nombre;
    public GameObject prefab;
    public Rareza rareza;
    [Range(0,100)] public int probabilidad = 100;
}
