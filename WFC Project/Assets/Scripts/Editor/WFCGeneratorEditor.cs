using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WFCGenerator))]
public class WFCGeneratorEditor : Editor {

    public bool advancedGridFoldOut = false;

    /*public override void OnInspectorGUI() {
        //DrawDefaultInspector();
        WFCGenerator gen = (WFCGenerator)target;

        GUILayout.Label("Advanced Settings", EditorStyles.boldLabel);
        gen.advancedSettings = EditorGUILayout.ToggleLeft("  Toggle Advanced Settings", gen.advancedSettings);
        gen.displayCellIDs = EditorGUILayout.ToggleLeft("   Toggle Cell Ids", gen.displayCellIDs);
        gen.displayCellLines = EditorGUILayout.ToggleLeft("   Toggle Cell Lines", gen.displayCellLines);
        GUILayout.Space(10);

        //Shows all simple options if advanced settings inst toggled
        if (!gen.advancedSettings) {
            GUILayout.BeginVertical();
            

            //labels and fields for the width, height and depth of the grid
            GUILayout.Label("Simple Generator Settings", EditorStyles.boldLabel);
            gen.cellStartingPos = (GameObject)EditorGUILayout.ObjectField("Grid Starting Position", gen.cellStartingPos, typeof(GameObject), true);
            gen.dataList = (assetDataList)EditorGUILayout.ObjectField("Given Data List", gen.dataList, typeof(assetDataList), true);


            GUILayout.Space(10);
            {
                gen.gridWidth = EditorGUILayout.IntField("Grid Width", gen.gridWidth);
                gen.gridHeight = EditorGUILayout.IntField("Grid Height", gen.gridHeight);
                gen.gridDepth = EditorGUILayout.IntField("Grid Depth", gen.gridDepth);
            }

            GUILayout.Space(5f);
            //Creating labels for the cell sizes
            {
                GUILayout.BeginHorizontal();

                GUILayout.Label("Cell Size X");
                GUILayout.Label("Cell Size Y");
                GUILayout.Label("Cell Size Z");

                GUILayout.EndHorizontal();
            }
            //Creating fields for the cell sizes
            {
                GUILayout.BeginHorizontal();

                gen.cellSizeX = EditorGUILayout.FloatField(gen.cellSizeX);
                gen.cellSizeY = EditorGUILayout.FloatField(gen.cellSizeY);
                gen.cellSizeZ = EditorGUILayout.FloatField(gen.cellSizeZ);

                GUILayout.EndHorizontal();
            }


            GUILayout.EndVertical();
        }

        //Shows all advanced options if advanced settings is toggled
        else if (gen.advancedSettings) {
            GUILayout.BeginVertical();
            GUILayout.Label("Advanced Generator Settings", EditorStyles.boldLabel);
            gen.cellStartingPos = (GameObject)EditorGUILayout.ObjectField("Grid Starting Position", gen.cellStartingPos, typeof(GameObject), true);
            gen.centerGridOnGeneration = EditorGUILayout.ToggleLeft("   Center grid at the spawn position", gen.centerGridOnGeneration);
            GUILayout.Space(10);


            GUILayout.Label("Grid Generation Settings", EditorStyles.boldLabel);
            {
                gen.gridWidth = EditorGUILayout.IntSlider("Grid Width", gen.gridWidth, 1, 10);
                gen.gridHeight = EditorGUILayout.IntSlider("Grid Height", gen.gridHeight, 1, 10);
                gen.gridDepth = EditorGUILayout.IntSlider("Grid Depth", gen.gridDepth, 1, 10);
            }
            GUILayout.EndVertical();

            
            advancedGridFoldOut = EditorGUILayout.Foldout(advancedGridFoldOut, "Advanced Grid Allowed Choices");
            if (advancedGridFoldOut) {
                GUILayout.Label("hahah");
            }
        }

        if (GUILayout.Button("Regenerate Grid")) gen.regenerateGrid();
        if (GUILayout.Button("Regenerate Enviroment")) gen.generateMap();
    }*/
}
