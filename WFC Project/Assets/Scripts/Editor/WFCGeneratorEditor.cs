using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WFCGenerator))]
public class WFCGeneratorEditor : Editor {

    public bool advancedGridFoldOut = false;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        WFCGenerator gen = (WFCGenerator)target;


        GUILayout.Space(10f);
        if (GUILayout.Button("Regenerate Grid")) gen.gridArray = GridGenerator.generateGrid(gen.gridSize, gen.cellSizes, gen.dataList, gen.cellStartingPos);
        if (GUILayout.Button("Regenerate Enviroment")) {
            if (gen.timeBetweenSpawning <= 0) gen.generateMapInstant();
            else gen.StartCoroutine(gen.generateMap());
        }


    }
}
