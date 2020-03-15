using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Bay)), CanEditMultipleObjects]
public class BayEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Bay bay = (Bay)target;
        base.OnInspectorGUI();

        bay.Init();
        if (GUILayout.Button("Fill"))
        {
            bay.Fill(false);
        }
        if (GUILayout.Button("Unload All"))
        {
            bay.UnloadAll();
        }
        PrefabUtility.RecordPrefabInstancePropertyModifications(bay.transform);
        SceneView.RepaintAll();
    }
}