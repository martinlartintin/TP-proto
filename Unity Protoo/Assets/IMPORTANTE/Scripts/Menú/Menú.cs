using UnityEngine;
using UnityEngine.SceneManagement;

public class Menú : MonoBehaviour
{
    public void Salir()
    {
        Application.Quit();
    }

    public void Jugar()
    {
        SceneManager.LoadScene("Main");
    }
}
