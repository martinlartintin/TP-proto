using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FantasmaData
{
    public string nombre;
    public Rareza rareza;
    public string tumbaName;
}

public class GameManagerPersistente : MonoBehaviour
{
    public static GameManagerPersistente Instancia;

    [Header("Datos de fantasmas")]
    public List<FantasmaData> fantasmasDesbloqueados = new List<FantasmaData>();
    [HideInInspector] public FantasmaData fantasmaSeleccionado;

    [Header("Ectoplasma")]
    [SerializeField] private int _ectoplasma = 10;
    public int costoPorTirada = 2;

    // 🔹 Nuevo evento
    public event Action<int> OnEctoplasmaCambiado;

    public int ectoplasma
    {
        get => _ectoplasma;
        set
        {
            _ectoplasma = value;
            OnEctoplasmaCambiado?.Invoke(_ectoplasma); // 🔹 Dispara evento al cambiar
        }
    }

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
        fantasmasDesbloqueados.Clear();
    }
}
