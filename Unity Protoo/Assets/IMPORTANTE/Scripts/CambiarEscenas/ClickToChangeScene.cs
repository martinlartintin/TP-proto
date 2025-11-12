using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToChangeScene : MonoBehaviour
{
    [SerializeField] private string nombreDeEscena = "Iglesia_2";

    private void OnMouseDown()
    {
        SceneManager.LoadScene(nombreDeEscena);
    }
}
