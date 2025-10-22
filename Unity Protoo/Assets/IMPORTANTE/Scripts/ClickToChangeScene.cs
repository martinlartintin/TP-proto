using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToChangeScene : MonoBehaviour
{
    [SerializeField] private string nombreDeEscena = "Iglesia"; // Cambia por el nombre de tu escena destino

    private void OnMouseDown()
    {
        // Cargar la escena al hacer clic
        SceneManager.LoadScene(nombreDeEscena);
    }
}
