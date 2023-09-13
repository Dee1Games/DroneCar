#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Toolset : EditorWindow
{
    [MenuItem("Tools/Tool Set")]
    public static void ShowWindow()
    {
        Toolset window = (Toolset) GetWindow(typeof(Toolset));
        window.titleContent = new GUIContent("Tool Set");
    }

    public Transform transform;
    private void OnGUI()
    {
        var active = Selection.activeTransform;
        if (!active) return;

        if (active)
        {

        }
        if (GUILayout.Button("Remove Mesh Colliders"))
        {
            var colliders = active.GetComponentsInChildren<MeshCollider>();
            foreach (var collider in colliders)
            {
                DestroyImmediate(collider);
            }
        }
    }
    [ContextMenu("Tools/Remove Mesh Colliders")]
    private void RemoveColliders()
    {
        var active = Selection.activeTransform;
        if (!active) return;

        var colliders = active.GetComponentsInChildren<MeshCollider>();
        foreach (var collider in colliders)
        {
            DestroyImmediate(collider);
        }
    }
}


#endif
