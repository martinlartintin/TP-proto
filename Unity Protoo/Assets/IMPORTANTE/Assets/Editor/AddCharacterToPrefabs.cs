using UnityEngine;
using UnityEditor;

public class AddCharacterToPrefabs : MonoBehaviour
{
    [MenuItem("Tools/Agregar Character a Prefabs de Fantasmas")]
    public static void AgregarCharacter()
    {
        // Carpeta donde están tus prefabs
        string rutaPrefabs = "Assets/Prefabs/Fantasmas"; // cambia según tu proyecto

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { rutaPrefabs });
        int contador = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null)
            {
                // Instancia temporal para editar
                GameObject temp = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

                if (temp.GetComponent<Character>() == null)
                {
                    Character character = temp.AddComponent<Character>();

                    // Valores por defecto
                    character.maxHealth = 100;
                    character.currentHealth = 100;

                    // Inicializa un array vacío de ataques si es que existe la propiedad
                    if (character.attacks == null)
                        character.attacks = new Attack[0];

                    // Guardar cambios en el prefab
                    PrefabUtility.ApplyPrefabInstance(temp, InteractionMode.AutomatedAction);
                    contador++;
                    Debug.Log($"✅ Character agregado a prefab: {prefab.name}");
                }

                Object.DestroyImmediate(temp);
            }
        }

        Debug.Log($"🎯 Proceso terminado. Se actualizaron {contador} prefabs.");
    }
}
