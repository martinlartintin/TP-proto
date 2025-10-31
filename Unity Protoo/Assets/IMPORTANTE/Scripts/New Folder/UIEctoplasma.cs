using UnityEngine;
using TMPro;

public class UIEctoplasma : MonoBehaviour
{
    public TMP_Text ectoplasmaText;

    void OnEnable()
    {
        if (GameManagerPersistente.Instancia != null)
        {
            GameManagerPersistente.Instancia.OnEctoplasmaCambiado += ActualizarUI;
            ActualizarUI(GameManagerPersistente.Instancia.ectoplasma);
        }
    }

    void OnDisable()
    {
        if (GameManagerPersistente.Instancia != null)
            GameManagerPersistente.Instancia.OnEctoplasmaCambiado -= ActualizarUI;
    }

    void ActualizarUI(int valor)
    {
        if (ectoplasmaText != null)
            ectoplasmaText.text = $"Ectoplasma: {valor}";
    }
}