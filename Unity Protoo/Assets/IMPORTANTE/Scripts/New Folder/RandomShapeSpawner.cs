using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomShapeSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public Vector3 escalaUniforme = Vector3.one;
    public Vector3 rotacionFija = Vector3.zero;
    public bool mirarHaciaDerecha = true;

    private void Start()
    {
        foreach (var f in GameManagerPersistente.Instancia.ghostsDesbloqueados)
        {
            InstanciarFantasma(f);
        }
    }

    public GameObject InstanciarFantasma(PersonajeData data)
    {
        if (data == null || data.prefab == null) return null;

        Transform punto = BuscarSiguienteTumbaLibre();
        if (punto == null) return null;

        GameObject nuevo = Instantiate(data.prefab, punto.position, Quaternion.identity, punto);
        nuevo.transform.rotation = Quaternion.Euler(rotacionFija);
        nuevo.transform.localScale = escalaUniforme;

        var sprite = nuevo.GetComponentInChildren<SpriteRenderer>();
        if (sprite != null)
        {
            Vector3 scale = sprite.transform.localScale;
            scale.x = mirarHaciaDerecha ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            sprite.transform.localScale = scale;
        }

        if (GameManagerPersistente.Instancia.ghostSeleccionado != null &&
            GameManagerPersistente.Instancia.ghostSeleccionado.nombre == data.nombre)
        {
            CameraController cam = FindFirstObjectByType<CameraController>();
            if (cam != null) StartCoroutine(EnfocarTrasFrame(nuevo.transform, cam));
        }

        return nuevo;
    }

    private IEnumerator EnfocarTrasFrame(Transform objetivo, CameraController cam)
    {
        yield return null;
        cam.FocusOn(objetivo);
    }

    private Transform BuscarSiguienteTumbaLibre()
    {
        foreach (var p in spawnPoints)
            if (p.childCount == 0)
                return p;
        return null;
    }
}
