using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.SceneManagement;

public class WFCGenerator : MonoBehaviour {

    //Simple grid generator settings
    public GameObject cellStartingPos;
    public int gridWidth, gridHeight, gridDepth;
    public float cellSizeX, cellSizeY, cellSizeZ;

    //Advanced grid generator settings
    public bool centerGridOnGeneration = false;

    //Level Generator Settings
    public assetDataList dataList;
    public Vector3Int startingCellId;
    Vector3Int randomCellId;

    //Debug options settings
    public bool displayCellIDs = false;
    public bool displayCellLines = false;
    public bool advancedSettings = false;


    //Hidden variables
    private gridCell[,,] gridArray;

    private void Awake() {
        regenerateGrid();
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.Space)) printGridInfo();
        if (Input.GetKeyUp(KeyCode.Alpha0)) {
            for (int i = 0; i < gridArray[0, 0, 0].allowedAsssetsInCell.Count; i++) {
                print("The cell of 0,0,0 allows " + gridArray[0, 0, 0].allowedAsssetsInCell[i].assetName);
            }
        }
        if (Input.GetKeyUp(KeyCode.F1)) SceneManager.LoadScene(0);

    }

    //Overall script to generate grid 
    public void regenerateGrid() {
        //Remake Array WIth Desired Size
        gridArray = new gridCell[gridWidth, gridHeight, gridDepth];

        //iterates through the entier grid and initializes the struct at that spesific ID, with all the required information
        for (int x = 0; x < gridWidth; x++) {
            for (int y = 0; y < gridHeight; y++) {
                for (int z = 0; z < gridDepth; z++) {

                    //temp variables to do basic calculation
                    Vector3Int tempId = new Vector3Int(x, y, z);
                    Vector3 tempSize = new Vector3(cellSizeX, cellSizeY, cellSizeZ);

                    //initializing the cell at a spesific grid ID
                    gridArray[x, y, z] = new gridCell(tempId, getCellPosInWorldspace(x, y, z), tempSize, dataList);
                }
            }
        }
    }

    //Prints off every grid id as well as the size of that speisfic cell
    void printGridInfo() {
        if (gridArray == null) return;
        //iterates through the entier grid 
        for (int x = 0; x < gridWidth; x++) {
            //print("On the x axis");
            for (int y = 0; y < gridHeight; y++) {
                //print("On the y axis");

                for (int z = 0; z < gridDepth; z++) {
                    //print("On the z axis");

                    //logs each grid cell, at their position, with their id, and how big they are
                    Debug.Log("Cell of id " + gridArray[x, y, z].cellId + " Is Collapsed with " + gridArray[x, y, z].dataAssigned.assetName); ;
                }
            }
        }
    }

    //This part of the code is now generating the map overall
    public void generateMap() {
        for (int emptySpacesLeft = gridWidth * gridHeight * gridDepth; emptySpacesLeft >= 0;) {
            Vector3Int[] EmptyAdjacentCells;

            if (emptySpacesLeft == gridWidth * gridHeight * gridDepth) randomCellId = startingCellId;
            else if(emptySpacesLeft < gridWidth * gridHeight * gridDepth) {
                EmptyAdjacentCells = getEmptyAdjacentCells(randomCellId).ToArray();

                if(EmptyAdjacentCells.Length == 0) {

                }

                randomCellId = EmptyAdjacentCells[Random.Range(0, EmptyAdjacentCells.Length-1)];
            }

            print(randomCellId);
            Vector3 worldPositionForCell = getCellPosInWorldspace(randomCellId.x, randomCellId.y, randomCellId.z);
            Vector3 posOffset = new Vector3(cellSizeX / 2, cellSizeY / 2, cellSizeZ / 2);

            gridArray[randomCellId.x, randomCellId.y, randomCellId.z].dataAssigned = gridArray[randomCellId.x, randomCellId.y, randomCellId.z].allowedAsssetsInCell[Random.Range(0, gridArray[randomCellId.x, randomCellId.y, randomCellId.z].allowedAsssetsInCell.Count)];

            setAdjacentRules(gridArray[randomCellId.x, randomCellId.y, randomCellId.z].cellId);
            gridArray[randomCellId.x, randomCellId.y, randomCellId.z].cellObj = Instantiate(gridArray[randomCellId.x, randomCellId.y, randomCellId.z].dataAssigned.primaryAsset, worldPositionForCell + posOffset, Quaternion.identity, GameObject.Find("Enviroment Holder").transform);
            emptySpacesLeft--;
        }
    }

    void setAdjacentRules(Vector3 cellId) {
        //Set The Upper Adjacent Limits
        if (cellId.y != gridHeight - 1) {

            //for each of the allowed assets above the prime number
            for (int x = 0; x < gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z].dataAssigned.allowedAssetsAbove.Length - 1; x++) {

                //Runs through each of the current allowed assets within the above adjacent cell
                for (int y = 0; y < gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].allowedAsssetsInCell.Count - 1; y++) {

                    //If the prime number's above rules do *NOT* contain the current allowed asset within the adjacent cell
                    //remove that asset from the allowed cells
                    if (!gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z].dataAssigned.allowedAssetsAbove.
                        Contains(gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].allowedAsssetsInCell[y])) {
                        gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].allowedAsssetsInCell.RemoveAt(y);
                    }
                }
            }
        }

        //Set The Lower Adjacent Limits
        if (cellId.y != 0) {
            //for each of the allowed assets below the prime number
            for (int x = 0; x < gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z].dataAssigned.allowedAssetsBelow.Length - 1; x++) {

                //Runs through each of the current allowed assets within the below adjacent cell
                for (int y = 0; y < gridArray[(int)cellId.x, (int)cellId.y - 1, (int)cellId.z].allowedAsssetsInCell.Count - 1; y++) {

                    //If the prime number's below rules do *NOT* contain the current allowed asset within the adjacent cell
                    //remove that asset from the allowed cells
                    if (!gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z].dataAssigned.allowedAssetsBelow.
                        Contains(gridArray[(int)cellId.x, (int)cellId.y - 1, (int)cellId.z].allowedAsssetsInCell[y])) {
                        gridArray[(int)cellId.x, (int)cellId.y - 1, (int)cellId.z].allowedAsssetsInCell.RemoveAt(y);
                    }
                }
            }
        }

        //Set The Right Adjacent Limits
        if (cellId.x != gridWidth - 1) {

            //for each of the allowed assets to the right of the prime number
            for (int x = 0; x < gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z].dataAssigned.allowedAssetsRight.Length - 1; x++) {

                //Runs through each of the current allowed assets within the right adjacent cell
                for (int y = 0; y < gridArray[(int)cellId.x + 1, (int)cellId.y, (int)cellId.z].allowedAsssetsInCell.Count - 1; y++) {

                    //If the prime number's right side rules do *NOT* contain the current allowed asset within the adjacent cell
                    //remove that asset from the allowed cells
                    if (!gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z].dataAssigned.allowedAssetsRight.
                        Contains(gridArray[(int)cellId.x + 1, (int)cellId.y, (int)cellId.z].allowedAsssetsInCell[y])) {
                        gridArray[(int)cellId.x + 1, (int)cellId.y, (int)cellId.z].allowedAsssetsInCell.RemoveAt(y);
                    }
                }
            }
        }

        //Set The Left Adjacent Limits
        if (cellId.x != 0) {
            //for each of the allowed assets to the left of the prime number
            for (int x = 0; x < gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z].dataAssigned.allowedAssetsLeft.Length - 1; x++) {

                //Runs through each of the current allowed assets within the left adjacent cell
                for (int y = 0; y < gridArray[(int)cellId.x - 1, (int)cellId.y, (int)cellId.z].allowedAsssetsInCell.Count - 1; y++) {

                    //If the prime number's left side rules do *NOT* contain the current allowed asset within the adjacent cell
                    //remove that asset from the allowed cells
                    if (!gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z].dataAssigned.allowedAssetsLeft.
                        Contains(gridArray[(int)cellId.x - 1, (int)cellId.y, (int)cellId.z].allowedAsssetsInCell[y])) {
                        gridArray[(int)cellId.x - 1, (int)cellId.y, (int)cellId.z].allowedAsssetsInCell.RemoveAt(y);
                    }
                }
            }
        }

        //Set The Forward Adjacent Limits
        if (cellId.z != gridDepth - 1) {
            //for each of the allowed assets in front of the prime number
            for (int x = 0; x < gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z].dataAssigned.allowedAssetsForward.Length - 1; x++) {

                //Runs through each of the current allowed assets within the front adjacent cell
                for (int y = 0; y < gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z + 1].allowedAsssetsInCell.Count - 1; y++) {

                    //If the prime number's front side rules do *NOT* contain the current allowed asset within the adjacent cell
                    //remove that asset from the allowed cells
                    if (!gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z].dataAssigned.allowedAssetsForward.
                        Contains(gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z + 1].allowedAsssetsInCell[y])) {
                        gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z + 1].allowedAsssetsInCell.RemoveAt(y);
                    }
                }
            }
        }

        //Set The Backward Adjacent Limits
        if (cellId.z != 0) {
            //for each of the allowed assets behind of the prime number
            for (int x = 0; x < gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z].dataAssigned.allowedAssetsBackward.Length - 1; x++) {

                //Runs through each of the current allowed assets within the behind adjacent cell
                for (int y = 0; y < gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z - 1].allowedAsssetsInCell.Count - 1; y++) {

                    //If the prime number's back side rules do *NOT* contain the current allowed asset within the adjacent cell
                    //remove that asset from the allowed cells
                    if (!gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z].dataAssigned.allowedAssetsBackward.
                        Contains(gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z - 1].allowedAsssetsInCell[y])) {
                        gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z - 1].allowedAsssetsInCell.RemoveAt(y);
                    }
                }
            }
        }
    }

    List<Vector3Int> getEmptyAdjacentCells(Vector3 cellId) {
        List<Vector3Int> cellsToReturn = new List<Vector3Int>(0);
        if (cellId.x != gridWidth - 1)
            if (gridArray[(int)cellId.x + 1, (int)cellId.y, (int)cellId.z].dataAssigned == null) cellsToReturn.Add(gridArray[(int)cellId.x + 1, (int)cellId.y, (int)cellId.z].cellId);
        if (cellId.x != 0)
            if (gridArray[(int)cellId.x - 1, (int)cellId.y, (int)cellId.z].dataAssigned == null) cellsToReturn.Add(gridArray[(int)cellId.x - 1, (int)cellId.y, (int)cellId.z].cellId);
        if (cellId.y != gridHeight - 1)
            if (gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].dataAssigned == null) cellsToReturn.Add(gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].cellId);
        if (cellId.y != 0)
            if (gridArray[(int)cellId.x, (int)cellId.y - 1, (int)cellId.z].dataAssigned == null) cellsToReturn.Add(gridArray[(int)cellId.x, (int)cellId.y - 1, (int)cellId.z].cellId);
        if (cellId.z != gridDepth - 1)
            if (gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z + 1].dataAssigned == null) cellsToReturn.Add(gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z + 1].cellId);
        if (cellId.z != 0)
            if (gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z - 1].dataAssigned == null) cellsToReturn.Add(gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z - 1].cellId);

        

        return cellsToReturn;
    }

    private void OnDrawGizmos() {
        //to stop errors from happening if the array is not initalized
        if (gridArray == null) return;

        if (displayCellIDs) {
            //itterates through each cell and places a lable explaining its Cell ID
            for (int x = 0; x < gridArray.GetLength(0); x++) {
                for (int y = 0; y < gridArray.GetLength(1); y++) {
                    for (int z = 0; z < gridArray.GetLength(2); z++) {

                        Vector3 handleOffset = new Vector3(gridArray[x, y, z].cellPos.x + (cellSizeX / 2), gridArray[x, y, z].cellPos.y + (cellSizeY / 2), gridArray[x, y, z].cellPos.z + (cellSizeZ / 2));
                        Handles.Label(handleOffset, gridArray[x, y, z].cellId.ToString());
                    }
                }
            }
        }
        if (displayCellLines) {
            //Generates the inside grid lines
            //Draws the Width Lines
            for (int z = 0; z < gridArray.GetLength(2) + 1; z++) {
                for (int y = 0; y < gridArray.GetLength(1) + 1; y++) {
                    //Create a border of colour within the grid
                    if (z == 0 || y == 0 || z == gridArray.GetLength(2) || y == gridArray.GetLength(1)) Gizmos.color = Color.black;
                    else Gizmos.color = Color.grey;

                    //Calculations
                    float lineZPos = cellStartingPos.transform.position.z + (z * cellSizeZ);
                    float lineYPos = cellStartingPos.transform.position.y + (y * cellSizeY);
                    Vector3 lineStartPos = new Vector3(cellStartingPos.transform.position.x, lineYPos, lineZPos);
                    Vector3 lineEndPos = new Vector3(cellStartingPos.transform.position.x + (gridWidth * cellSizeX), lineYPos, lineZPos);

                    Gizmos.DrawLine(lineStartPos, lineEndPos);
                }
            }

            //Draws the height lines
            for (int x = 0; x < gridArray.GetLength(0) + 1; x++) {
                for (int z = 0; z < gridArray.GetLength(2) + 1; z++) {
                    //Create a border of colour within the grid
                    if (x == 0 || z == 0 || x == gridArray.GetLength(0) || z == gridArray.GetLength(2)) Gizmos.color = Color.black;
                    else Gizmos.color = Color.grey;

                    //Calculations
                    float lineXPos = cellStartingPos.transform.position.x + (x * cellSizeX);
                    float lineZPos = cellStartingPos.transform.position.z + (z * cellSizeZ);
                    Vector3 lineStartPos = new Vector3(lineXPos, cellStartingPos.transform.position.y, lineZPos);
                    Vector3 lineEndPos = new Vector3(lineXPos, cellStartingPos.transform.position.y + (gridHeight * cellSizeY), lineZPos);

                    Gizmos.DrawLine(lineStartPos, lineEndPos);
                }
            }

            //Draws the depth lines
            for (int x = 0; x < gridArray.GetLength(0) + 1; x++) {
                for (int y = 0; y < gridArray.GetLength(1) + 1; y++) {
                    //Create a border of colour within the grid
                    if (x == 0 || y == 0 || x == gridArray.GetLength(0) || y == gridArray.GetLength(1)) Gizmos.color = Color.black;
                    else Gizmos.color = Color.grey;

                    //Calculations
                    float lineXPos = cellStartingPos.transform.position.x + (x * cellSizeX);
                    float lineYPos = cellStartingPos.transform.position.y + (y * cellSizeY);
                    Vector3 lineStartPos = new Vector3(lineXPos, lineYPos, cellStartingPos.transform.position.z);
                    Vector3 lineEndPos = new Vector3(lineXPos, lineYPos, cellStartingPos.transform.position.z + (gridDepth * cellSizeZ));

                    Gizmos.DrawLine(lineStartPos, lineEndPos);
                }
            }
        }
    }

    //gets the grid position of the cell in real world space
    public Vector3 getCellPosInWorldspace(int xId, int yId, int zId) {
        float tempPosX;
        float tempPosY;
        float tempPosZ;

        tempPosX = cellStartingPos.transform.position.x + (xId * cellSizeX);
        tempPosY = cellStartingPos.transform.position.y + (yId * cellSizeY);
        tempPosZ = cellStartingPos.transform.position.z + (zId * cellSizeZ);

        return new Vector3(tempPosX, tempPosY, tempPosZ);
    }

    //Gets the adjacent cells around the given ID

}

[System.Serializable]
//Cell struct with basic information required
public struct gridCell {
    public Vector3Int cellId;
    public Vector3 cellPos;
    public Vector3 cellSize;
    public List<assetData> allowedAsssetsInCell;
    public assetData dataAssigned;
    public GameObject cellObj;

    public gridCell(Vector3Int Id, Vector3 Pos, Vector3 Size, assetDataList dataList) {
        cellId = Id;
        cellPos = Pos;
        cellSize = Size;
        allowedAsssetsInCell = new List<assetData>();
        for (int i = 0; i < dataList.listOfAssets.Count - 1; i++) {
            allowedAsssetsInCell.Add(dataList.listOfAssets[i]);
        }
        dataAssigned = null;
        cellObj = null;
    }
}