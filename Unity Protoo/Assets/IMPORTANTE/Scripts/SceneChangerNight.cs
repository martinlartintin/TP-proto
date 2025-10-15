using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangerNight : MonoBehaviour
{
    public Button changeSceneButton; 

    void Start()
    {
        if (changeSceneButton != null)
            changeSceneButton.onClick.AddListener(ChangeScene);
        else
            Debug.LogError("No se ha asignado el botón en el Inspector");
    }

    void ChangeScene()
    {
        // Guardar estado usando singleton
        if (RandomShapeSpawner.Instance != null)
            RandomShapeSpawner.Instance.GuardarEstado();

        SceneManager.LoadScene("Main"); // Cambia "Main" por el nombre de tu escena de día si es distinto
    }
    
}
