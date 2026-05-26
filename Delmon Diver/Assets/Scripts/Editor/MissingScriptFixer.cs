#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor utility to safely remove missing script components from prefabs and GameObjects.
/// Resolves the critical error:
///   "Error while saving Prefab: You are trying to save a Prefab with a missing script."
///
/// Usage:
///   Tools > Delmon Diver > Clean Selected Prefab Missing Scripts   (for a selected prefab)
///   Tools > Delmon Diver > Clean All Prefabs in Project           (scans entire project)
///   Tools > Delmon Diver > Clean Scene Missing Scripts            (cleans open scene)
/// </summary>
public class MissingScriptFixer : EditorWindow
{
    // =====================================================================
    // Menu: Fix selected prefab (most common use-case)
    // =====================================================================
    [MenuItem("Tools/Delmon Diver/Clean Selected Prefab Missing Scripts")]
    private static void CleanSelectedPrefab()
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            EditorUtility.DisplayDialog(
                "No Selection",
                "Please select a Prefab asset in the Project window first.",
                "OK"
            );
            return;
        }

        string assetPath = AssetDatabase.GetAssetPath(selected);
        if (string.IsNullOrEmpty(assetPath))
        {
            // Object is in the scene, not a project prefab
            int removed = CleanGameObjectRecursive(selected);
            EditorUtility.DisplayDialog(
                "Done",
                $"Cleaned scene object '{selected.name}'.\nRemoved {removed} missing script(s).",
                "OK"
            );
            return;
        }

        // Load the prefab contents
        using (var editingScope = new PrefabUtility.EditPrefabContentsScope(assetPath))
        {
            GameObject prefabRoot = editingScope.prefabContentsRoot;
            int removed = CleanGameObjectRecursive(prefabRoot);

            Debug.Log($"[MissingScriptFixer] Cleaned prefab '{selected.name}': Removed {removed} missing script(s).");
            EditorUtility.DisplayDialog(
                "Prefab Cleaned",
                $"Successfully removed {removed} missing script(s) from '{selected.name}'.\n\nPrefab saved automatically.",
                "Great!"
            );
        }

        // Force refresh asset database
        AssetDatabase.Refresh();
    }

    // =====================================================================
    // Menu: Fix the specific First Person Controller prefab (targeted fix)
    // =====================================================================
    [MenuItem("Tools/Delmon Diver/Fix First Person Controller Prefab")]
    private static void FixFirstPersonControllerPrefab()
    {
        string targetPath = "Assets/Pirates Island/Standard Assets/Character Controllers/First Person Controller.prefab";
        GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(targetPath);

        if (prefabAsset == null)
        {
            EditorUtility.DisplayDialog(
                "Prefab Not Found",
                $"Could not locate prefab at:\n{targetPath}\n\nPlease use 'Clean Selected Prefab Missing Scripts' instead after selecting it manually.",
                "OK"
            );
            return;
        }

        using (var editingScope = new PrefabUtility.EditPrefabContentsScope(targetPath))
        {
            GameObject prefabRoot = editingScope.prefabContentsRoot;
            int removed = CleanGameObjectRecursive(prefabRoot);

            Debug.Log($"[MissingScriptFixer] First Person Controller prefab fixed. Removed {removed} missing script(s).");
            EditorUtility.DisplayDialog(
                "First Person Controller Fixed!",
                $"Successfully removed {removed} missing script(s) from the First Person Controller prefab.\n\nYou can now save scenes normally.",
                "Excellent!"
            );
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    // =====================================================================
    // Menu: Scan and clean ALL prefabs in the entire project
    // =====================================================================
    [MenuItem("Tools/Delmon Diver/Clean All Prefabs in Project")]
    private static void CleanAllPrefabsInProject()
    {
        bool confirm = EditorUtility.DisplayDialog(
            "Scan Entire Project?",
            "This will scan ALL prefabs in your project for missing scripts and remove them. This cannot be undone.\n\nProceed?",
            "Yes, Clean All",
            "Cancel"
        );

        if (!confirm) return;

        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        int totalPrefabsScanned = 0;
        int totalScriptsRemoved = 0;
        int affectedPrefabs = 0;

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            using (var editingScope = new PrefabUtility.EditPrefabContentsScope(path))
            {
                GameObject prefabRoot = editingScope.prefabContentsRoot;
                int removed = CleanGameObjectRecursive(prefabRoot);

                if (removed > 0)
                {
                    affectedPrefabs++;
                    totalScriptsRemoved += removed;
                    Debug.Log($"[MissingScriptFixer] Cleaned '{path}': Removed {removed} missing script(s).");
                }

                totalPrefabsScanned++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Project Scan Complete",
            $"Scan Results:\n" +
            $"- Prefabs Scanned: {totalPrefabsScanned}\n" +
            $"- Prefabs Affected: {affectedPrefabs}\n" +
            $"- Total Missing Scripts Removed: {totalScriptsRemoved}",
            "Done"
        );
    }

    // =====================================================================
    // Menu: Clean missing scripts in the currently open scene
    // =====================================================================
    [MenuItem("Tools/Delmon Diver/Clean Scene Missing Scripts")]
    private static void CleanScene()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>(includeInactive: true);
        int totalRemoved = 0;

        foreach (GameObject go in allObjects)
        {
            // Only process root scene objects (not prefab assets)
            if (go.scene.IsValid())
            {
                totalRemoved += CleanGameObjectRecursive(go);
            }
        }

        EditorUtility.DisplayDialog(
            "Scene Cleaned",
            $"Removed {totalRemoved} missing script(s) from the open scene.",
            "OK"
        );

        Debug.Log($"[MissingScriptFixer] Scene clean complete. Removed {totalRemoved} missing script(s).");
    }

    // =====================================================================
    // Core Helper: Recursively clean a GameObject and all its children
    // =====================================================================
    private static int CleanGameObjectRecursive(GameObject go)
    {
        if (go == null) return 0;

        int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);

        // Recurse into all children
        foreach (Transform child in go.transform)
        {
            removed += CleanGameObjectRecursive(child.gameObject);
        }

        return removed;
    }
}
#endif
