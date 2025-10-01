using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangerNight : MonoBehaviour
{
    public Button changeSceneButton; 

    void Start()
    {
        if (changeSceneButton != null)
        {
            
            changeSceneButton.onClick.AddListener(() => SceneManager.LoadScene("Main"));
        }
        else
        {
            Debug.LogError("No se ha asignado el botón en el Inspector");
        }
    }
}
