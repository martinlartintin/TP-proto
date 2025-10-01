using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangerDay : MonoBehaviour
{
    public Button changeSceneButton; 

    void Start()
    {
        if (changeSceneButton != null)
        {
            
            changeSceneButton.onClick.AddListener(() => SceneManager.LoadScene("EscenaNoche"));
        }
        else
        {
            Debug.LogError("No se ha asignado el botón en el Inspector");
        }
    }
}
