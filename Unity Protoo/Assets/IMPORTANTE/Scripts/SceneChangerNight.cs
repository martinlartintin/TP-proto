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
            Debug.LogError("No se asignó el botón en el Inspector");
    }

    void ChangeScene()
    {
        SceneManager.LoadScene("Main");
    }
}
