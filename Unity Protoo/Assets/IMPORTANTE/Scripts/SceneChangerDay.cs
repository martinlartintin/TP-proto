using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangerDay : MonoBehaviour
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

        SceneManager.LoadScene("EscenaNoche");
    }
}
