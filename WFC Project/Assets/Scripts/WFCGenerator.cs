using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Random = UnityEngine.Random;
using System.Linq;

public class WFCGenerator : MonoBehaviour
{

    //Simple grid generator settings
    public GameObject cellStartingPos;
    public int gridWidth, gridHeight, gridDepth;
    public float cellSizeX, cellSizeY, cellSizeZ;

    //Advanced grid generator settings
    public bool centerGridOnGeneration = false;

    //Level Generator Settings
    public assetDataList dataList;
    public List<gridCell> emptyCells = new List<gridCell>();

    //Debug options settings
    public bool displayCellIDs = false;
    public bool displayCellLines = false;
    public bool advancedSettings = false;


    //Hidden variables
    private gridCell[,,] gridArray;

    private void Awake()
    {
        regenerateGrid();
        foreach (gridCell cell in gridArray)
        {
            emptyCells.Add(cell);
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space)) printGridInfo();
    }

    //Overall script to generate grid 
    public void regenerateGrid()
    {
        //Remake Array WIth Desired Size
        gridArray = new gridCell[gridWidth, gridHeight, gridDepth];

        //iterates through the entier grid and initializes the struct at that spesific ID, with all the required information
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                for (int z = 0; z < gridDepth; z++)
                {

                    //temp variables to do basic calculation
                    Vector3Int tempId = new Vector3Int(x, y, z);
                    Vector3 tempSize = new Vector3(cellSizeX, cellSizeY, cellSizeZ);

                    //initializing the cell at a spesific grid ID
                    gridArray[x, y, z] = new gridCell(tempId, getGridPos(x, y, z), tempSize, dataList);
                }
            }
        }
    }

    //Prints off every grid id as well as the size of that speisfic cell
    void printGridInfo()
    {
        if (gridArray == null) return;
        //iterates through the entier grid 
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                for (int z = 0; z < gridArray.GetLength(2); z++)
                {
                    //logs each grid cell, at their position, with their id, and how big they are
                    Debug.Log("Has an ID of: " + gridArray[x, y, z].cellId + ", and a size of: " + gridArray[x, y, z].cellSize + ", Its asset is " + gridArray[x,y,z].dataAssigned.assetName);
                }
            }
        }
    }

    //This part of the code is now generating the map overall
    public void generateMap()
    {

        for (int i = 0; i < gridWidth * gridHeight * gridDepth; i++)
        {
            gridCell currentCell = emptyCells[Random.Range(0, emptyCells.Count)];
            if (currentCell.dataAssigned == null)
            {
                Vector3 worldPositionForCell = getGridPos(currentCell.cellId.x, currentCell.cellId.y, currentCell.cellId.z);
                Vector3 posOffset = new Vector3(cellSizeX / 2, cellSizeY / 2, cellSizeZ / 2);

                assetData dataToAssign = currentCell.allowedAsssetsInCell[Random.Range(0, currentCell.allowedAsssetsInCell.Count)];
                currentCell.dataAssigned = dataToAssign;

                //setAdjacentRules(currentCell.cellId, currentCell.dataAssigned);
                
                Instantiate(currentCell.dataAssigned.primaryAsset, worldPositionForCell + posOffset, Quaternion.identity, GameObject.Find("Enviroment Holder").transform);

                emptyCells.Remove(currentCell);
            }
        }
    }
    private void OnDrawGizmos()
    {
        //to stop errors from happening if the array is not initalized
        if (gridArray == null) return;

        if (displayCellIDs)
        {
            //itterates through each cell and places a lable explaining its Cell ID
            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    for (int z = 0; z < gridArray.GetLength(2); z++)
                    {

                        Vector3 handleOffset = new Vector3(gridArray[x, y, z].cellPos.x + (cellSizeX / 2), gridArray[x, y, z].cellPos.y + (cellSizeY / 2), gridArray[x, y, z].cellPos.z + (cellSizeZ / 2));
                        Handles.Label(handleOffset, gridArray[x, y, z].cellId.ToString());
                    }
                }
            }
        }
        if (displayCellLines)
        {
            //Generates the inside grid lines
            //Draws the Width Lines
            for (int z = 0; z < gridArray.GetLength(2) + 1; z++)
            {
                for (int y = 0; y < gridArray.GetLength(1) + 1; y++)
                {
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
            for (int x = 0; x < gridArray.GetLength(0) + 1; x++)
            {
                for (int z = 0; z < gridArray.GetLength(2) + 1; z++)
                {
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
            for (int x = 0; x < gridArray.GetLength(0) + 1; x++)
            {
                for (int y = 0; y < gridArray.GetLength(1) + 1; y++)
                {
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
    public Vector3 getGridPos(int xId, int yId, int zId)
    {
        float tempPosX;
        float tempPosY;
        float tempPosZ;

        tempPosX = cellStartingPos.transform.position.x + (xId * cellSizeX);
        tempPosY = cellStartingPos.transform.position.y + (yId * cellSizeY);
        tempPosZ = cellStartingPos.transform.position.z + (zId * cellSizeZ);

        return new Vector3(tempPosX, tempPosY, tempPosZ);
    }

    //Gets the adjacent cells around the given ID
    public Vector3[] getAdjacentCells(Vector3 cellId)
    {
        return null;
    }

}


//Cell struct with basic information required
public struct gridCell
{
    public Vector3Int cellId;
    public Vector3 cellPos;
    public Vector3 cellSize;
    public List<assetData> allowedAsssetsInCell;
    public assetData dataAssigned;

    public gridCell(Vector3Int Id, Vector3 Pos, Vector3 Size, assetDataList dataList)
    {
        cellId = Id;
        cellPos = Pos;
        cellSize = Size;
        allowedAsssetsInCell = dataList.listOfAssets;
        dataAssigned = null;
    }
}