using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string persistentSceneName = "PersistentScene";

    private string currentScene; // para saber qué escena está activa

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        currentScene = SceneManager.GetActiveScene().name;
    }

    public void CargarEscena(string nombre)
    {
        StartCoroutine(CargarAsync(nombre));
    }

    private IEnumerator CargarAsync(string nombre)
    {
        // Si ya estamos en la escena, no hacemos nada
        if (currentScene == nombre) yield break;

        // Cargar nueva escena aditivamente
        yield return SceneManager.LoadSceneAsync(nombre, LoadSceneMode.Additive);

        // Activar la nueva escena
        Scene nueva = SceneManager.GetSceneByName(nombre);
        SceneManager.SetActiveScene(nueva);

        // Descargar la escena anterior solo si no es la persistente
        if (currentScene != persistentSceneName)
            yield return SceneManager.UnloadSceneAsync(currentScene);

        currentScene = nombre;
    }

    // Función útil para volver a Main
    public void VolverAMain()
    {
        CargarEscena("Main");
    }
}
