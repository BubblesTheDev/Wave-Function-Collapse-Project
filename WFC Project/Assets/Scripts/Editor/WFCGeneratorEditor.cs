using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WFCGenerator))]
public class WFCGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        WFCGenerator gen = (WFCGenerator)target;

        GUILayout.BeginVertical();
        if (GUILayout.Button("Regenerate Grid")) gen.regenerateGrid();
        gen.advancedSettings = EditorGUILayout.Toggle("Toggle Advanced Settings", gen.advancedSettings);
        GUILayout.EndVertical();

    }
}
