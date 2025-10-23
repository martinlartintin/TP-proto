using System;
using UnityEngine;

[Serializable]
public class PersonajeData
{
    public string nombre;           // Nombre del personaje
    public Rareza rareza;           // Rareza (Comun, Epico, Legendario)
    public string parentName;       // Nombre del transform padre (tumba)
    public Vector3 localPosition;   // Posición local dentro del padre
}
