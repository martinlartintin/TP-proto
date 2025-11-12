using UnityEngine;
using System;
using System.Collections.Generic;

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
    public List<FantasmaData> fantasmasInvocados = new List<FantasmaData>();

    [Header("Ectoplasma")]
    [SerializeField] private int _ectoplasma = 10;
    public int costoPorTirada = 2;

    public event Action<int> OnEctoplasmaCambiado;

    public int ectoplasma
    {
        get => _ectoplasma;
        set
        {
            _ectoplasma = value;
            OnEctoplasmaCambiado?.Invoke(_ectoplasma);
        }
    }

    private void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("✅ GameManagerPersistente inicializado correctamente.");
        }
        else if (Instancia != this)
        {
            Debug.LogWarning("⚠️ Duplicado destruido de GameManagerPersistente en escena: " + gameObject.scene.name);
            Destroy(gameObject);
        }
    }


    public void ResetearFantasmas()
    {
        fantasmasDesbloqueados.Clear();
        fantasmasInvocados.Clear();
        fantasmaSeleccionado = null;
    }
}
