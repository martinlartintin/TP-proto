using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    public Button changeSceneButton;

    private string dayScene = "Main";
    private string nightScene = "EscenaNoche";

    void Start()
    {
        if (changeSceneButton != null)
            changeSceneButton.onClick.AddListener(ChangeScene);
    }

    void ChangeScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == dayScene)
            SceneManager.LoadScene(nightScene);
        else if (currentScene == nightScene)
            SceneManager.LoadScene(dayScene);
    }
}
