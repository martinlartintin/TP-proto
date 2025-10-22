using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangerDay1 : MonoBehaviour
{
    public Button changeSceneButton;

    void Start()
    {
        if (changeSceneButton != null)
            changeSceneButton.onClick.AddListener(ChangeScene);
        else
            Debug.LogError("No se asignó el botón en el Inspector");
    }

    void ChangeScene()
    {
        // No necesitamos spawner aquí, solo cambiamos de escena
        SceneManager.LoadScene("EscenaNoche");
    }
}
