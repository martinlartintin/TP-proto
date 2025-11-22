using UnityEngine;

public class Grave : MonoBehaviour
{
    public Transform focusPoint;


    private void OnMouseDown()
    {
        CameraController cam = FindFirstObjectByType<CameraController>();


        if (cam != null)
        {
            cam.FocusOn(focusPoint != null ? focusPoint : transform);
        }
        else
        {
            Debug.LogWarning("⚠ No se encontró CameraController en la escena.");
        }
    }
}
