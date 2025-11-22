using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManagerPersistente : MonoBehaviour
{
    public static GameManagerPersistente Instancia;

    [Header("Recursos")]
    private int _ectoplasma = 20;
    public int ectoplasma
    {
        get => _ectoplasma;
        set
        {
            int diferencia = value - _ectoplasma;
            if (diferencia > 0) totalEctoplasmaRecolectado += diferencia;
            _ectoplasma = value;
            OnEctoplasmaCambiado?.Invoke(_ectoplasma);
        }
    }

    public event Action<int> OnEctoplasmaCambiado;

    [Header("Estado de tumbas")]
    public List<string> eliminatedGraves = new List<string>();

    [Header("Estado de fantasmas para combate")]
    public int remainingAttempts = 3;
    public List<PersonajeData> lockedGhosts = new List<PersonajeData>();

    [Header("Fantasmas desbloqueados y derrotados")]
    public List<PersonajeData> ghostsDesbloqueados = new List<PersonajeData>();
    public List<PersonajeData> ghostsDerrotados = new List<PersonajeData>();
    public List<PersonajeData> fantasmasInvocados = new List<PersonajeData>();

    [Header("Fantasma seleccionado actualmente")]
    public PersonajeData ghostSeleccionado;

    [Header("Costo ruleta")]
    public int costoPorTirada = 5;

    [Header("Métricas de partida")]
    public int totalEctoplasmaRecolectado = 0;
    public int totalFantasmasDesbloqueados = 0;

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

    public void MarkGraveAsEliminated(string graveID)
    {
        if (!eliminatedGraves.Contains(graveID))
            eliminatedGraves.Add(graveID);
    }

    public bool UseGhostAttempt(PersonajeData ghost)
    {
        if (!lockedGhosts.Contains(ghost))
            lockedGhosts.Add(ghost);

        remainingAttempts--;
        return remainingAttempts > 0;
    }

    public void ResetAttempts()
    {
        remainingAttempts = 3;
    }

    public bool CanUseGhost(PersonajeData ghost)
    {
        return !ghostsDerrotados.Contains(ghost);
    }

    public void MarkGhostDefeated(PersonajeData ghost)
    {
        if (!ghostsDerrotados.Contains(ghost))
            ghostsDerrotados.Add(ghost);
    }

    public bool NoUnlockedGhosts()
    {
        foreach (var g in ghostsDesbloqueados)
        {
            if (CanUseGhost(g))
                return false;
        }
        return true;
    }

    public void GuardarMetricas()
    {
        totalFantasmasDesbloqueados = ghostsDesbloqueados.Count;
        string ruta = Path.Combine(Application.persistentDataPath, "metricas_partida.txt");
        string contenido = $"Total ectoplasma recolectado: {totalEctoplasmaRecolectado}\n" +
                           $"Total fantasmas desbloqueados: {totalFantasmasDesbloqueados}\n" +
                           $"Fantasmas desbloqueados:\n";

        foreach (var g in ghostsDesbloqueados)
            contenido += $"- {g.nombre} ({g.rareza})\n";

        File.WriteAllText(ruta, contenido);
        Debug.Log($"📊 Métricas guardadas en: {ruta}");
    }
}
