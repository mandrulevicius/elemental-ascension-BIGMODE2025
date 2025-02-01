using UnityEngine;
using UnityEditor;

public class SimpleColliderGenerator : EditorWindow {
    [MenuItem("Tools/Generate Simple Colliders")]
    public static void ShowWindow() {
        GetWindow(typeof(SimpleColliderGenerator));
    }

    void OnGUI() {
        if (GUILayout.Button("Generate Collider for Selected Prefab")) {
            GameObject selected = Selection.activeGameObject;
            if (selected != null) {
                GenerateCollider(selected);
            } else {
                Debug.LogWarning("Select a prefab with a MeshFilter first.");
            }
        }
    }

    static void GenerateCollider(GameObject obj) {
        // Find a MeshFilter in the prefab (searching children as needed)
        MeshFilter mf = obj.GetComponentInChildren<MeshFilter>();
        if (mf == null) {
            Debug.LogWarning("No MeshFilter found on " + obj.name);
            return;
        }

        Mesh mesh = mf.sharedMesh;
        if (mesh == null) {
            Debug.LogWarning("MeshFilter has no mesh.");
            return;
        }

        // Use the mesh's bounds to create a simple BoxCollider
        Bounds bounds = mesh.bounds;

        // Add a BoxCollider on the root object
        BoxCollider bc = obj.AddComponent<BoxCollider>();
        bc.center = bounds.center;
        bc.size = bounds.size;

        Debug.Log("BoxCollider added based on mesh bounds.");
    }
}