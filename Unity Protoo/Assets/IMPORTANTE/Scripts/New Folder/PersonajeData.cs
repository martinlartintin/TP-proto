using UnityEngine;

public enum Rareza
{
    Comun,
    Epico,
    Legendario
}

[CreateAssetMenu(fileName = "NuevoPersonaje", menuName = "Datos/Personaje")]
[System.Serializable]
public class PersonajeData : ScriptableObject
{
    public string nombre;
    public GameObject prefab;
    public Rareza rareza; // 🔹 Este campo SÍ existe
    [Range(0, 100)] public int probabilidad = 100;
}
