using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WFCGenerator : MonoBehaviour {
    
    [Space , Header("Generator Settings")]
    public GameObject cellStartingPos;
    public int gridWidth, gridLength, gridHeight;
    public float cellSizeX, cellSizeY, cellSizeZ;

    [Space, Header("Debug Settings")]

    private gridCell[,,] gridArray;


    private void Awake() {
        regenerateGrid();
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.Space)) printGridInfo();
    }

    //Overall script to generate grid 
    void regenerateGrid() {
        //Remake Array WIth Desired Size
        gridArray = new gridCell[gridWidth, gridHeight, gridLength];

        //iterates through the entier grid and initializes the struct at that spesific ID, with all the required information
        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y < gridLength; y++) {
                for (int z = 0; z < gridHeight; z++) {
                    
                    //temp variables to do basic calculation
                    Vector3Int tempId = new Vector3Int(x, y ,z);
                    Vector3 tempSize = new Vector3(cellSizeX, cellSizeY, cellSizeZ);

                    //initializing the cell at a spesific grid ID
                    gridArray[x, y, z] = new gridCell(tempId, getGridPos(x, y, z), tempSize);
                }
            }
        }
    }

    void printGridInfo() {
        if (gridArray == null) return;
        //iterates through the entier grid 
        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                for (int z = 0; z < gridArray.GetLength(2); z++) {
                    //logs each grid cell, at their position, with their id, and how big they are
                    Debug.Log("Cell at: " + gridArray[x, y, z].cellPos + ", Has an ID of: " + gridArray[x, y, z].cellId + ", and a size of: " + gridArray[x, y, z].cellSize);
                }
            }
        }
    }

    private void OnDrawGizmos() {
        //to stop errors from happening this just allows me to turn on and off gizmos at will
        if (gridArray == null) return;

        //itterates through each cell and places a lable explaining its Cell ID
        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                for (int z = 0; z < gridArray.GetLength(2); z++) {
                    Handles.color = Color.blue;

                    Vector3 handleOffset = new Vector3(gridArray[x, y, z].cellPos.x + (cellSizeX/2), gridArray[x, y, z].cellPos.y + (cellSizeY/2), gridArray[x, y, z].cellPos.z + (cellSizeZ/2));
                    Handles.Label(handleOffset, gridArray[x, y, z].cellId.ToString());
                }
            }
        }

        //Generates top and left lines
        /*Gizmos.color = Color.black;
        {
            //draws the width line
            float widthLineEndPosX = cellStartingPos.transform.position.x + (cellSizeX * gridWidth);
            Vector3 widthLineEndPos = new Vector3(widthLineEndPosX, 0, cellStartingPos.transform.position.z);
            Gizmos.DrawLine(cellStartingPos.transform.position, widthLineEndPos);

            //draws the length line
            float lengthLineEndPosZ = cellStartingPos.transform.position.z + (cellSizeZ * gridLength);
            Vector3 lengthLineEndPos = new Vector3(cellStartingPos.transform.position.x, 0, lengthLineEndPosZ);
            Gizmos.DrawLine(cellStartingPos.transform.position, lengthLineEndPos);
        }

        //itterates through each horizontal grid cell to place a line down
        /*for (int x = 0; x < gridArray.GetLength(0); x++) {
            Gizmos.color = Color.white;
            if (x == gridArray.GetLength(0) - 1) Gizmos.color = Color.black;
            float tempStartPosX = gridArray[x, 0].cellPos.x + cellSizeX;
            float tempStartPosZ = gridArray[x, 0].cellPos.z;
            Vector3 tempStartPos = new Vector3(tempStartPosX, 0, tempStartPosZ);

            float tempEndPosX = gridArray[x, 0].cellPos.x + cellSizeX;
            float tempEndPosZ = gridArray[x, 0].cellPos.z + cellSizeZ * gridLength;
            Vector3 tempEndingPos = new Vector3(tempEndPosX, 0, tempEndPosZ);

            Gizmos.DrawLine(tempStartPos, tempEndingPos);
        }

        //Iterates through each vertical grid cell to place a line down
        for (int z = 0; z < gridArray.GetLength(1); z++) {
            Gizmos.color = Color.white;
            if (z == gridArray.GetLength(1) - 1) Gizmos.color = Color.black;
            float tempStartPosZ = gridArray[0, z].cellPos.z + cellSizeZ;
            float tempStartPosX = gridArray[0, z].cellPos.x;
            Vector3 tempStartPos = new Vector3(tempStartPosX, 0, tempStartPosZ);

            float tempEndPosZ = gridArray[0, z].cellPos.z + cellSizeZ;
            float tempEndPosX = gridArray[0, z].cellPos.x + cellSizeX * gridWidth;
            Vector3 tempEndingPos = new Vector3(tempEndPosX, 0, tempEndPosZ);

            Gizmos.DrawLine(tempStartPos, tempEndingPos);
        }*/

    }

    //gets the grid position of the cell in real world space
    public Vector3 getGridPos(int xId, int yId, int zId) {
        float tempPosX;
        float tempPosY;
        float tempPosZ;

        tempPosX = cellStartingPos.transform.position.x + (xId * cellSizeX);
        tempPosY = cellStartingPos.transform.position.y + (yId * cellSizeY);
        tempPosZ = cellStartingPos.transform.position.z + (zId * cellSizeZ);
        
        return new Vector3(tempPosX, tempPosY, tempPosZ);
    }

}



//Cell struct with basic information required
public struct gridCell {
    public Vector3Int cellId;
    public Vector3 cellPos;
    public Vector3 cellSize;

    public gridCell(Vector3Int Id, Vector3 Pos, Vector3 Size) {
        cellId = Id;
        cellPos = Pos;
        cellSize = Size;
    }
}