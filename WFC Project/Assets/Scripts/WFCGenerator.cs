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
    //public List<gridCell> emptyCells = new List<gridCell>();

    //Debug options settings
    public bool displayCellIDs = false;
    public bool displayCellLines = false;
    public bool advancedSettings = false;


    //Hidden variables
    private gridCell[,,] gridArray;

    private void Awake()
    {
        regenerateGrid();
        /*foreach (gridCell cell in gridArray)
        {
            emptyCells.Add(cell);
        }*/
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space)) printGridInfo();
        if (Input.GetKeyUp(KeyCode.F1))
        {
            print(gridWidth);
            print(gridHeight);
            print(gridDepth);
        }
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
        for (int x = 0; x < gridWidth; x++)
        {
            //print("On the x axis");
            for (int y = 0; y < gridHeight; y++)
            {
                //print("On the y axis");

                for (int z = 0; z < gridDepth; z++)
                {
                    //print("On the z axis");

                    //logs each grid cell, at their position, with their id, and how big they are
                    Debug.Log("Cell of id " + gridArray[x, y, z].cellId + " Is Collapsed with " + gridArray[x, y, z].dataAssigned.assetName); ;
                }
            }
        }
    }

    //This part of the code is now generating the map overall
    public void generateMap()
    {
        for (int i = 0; i < gridWidth * gridHeight * gridDepth; i++)
        {
            Vector3Int randomCellId = new Vector3Int(Random.Range(0, gridWidth), Random.Range(0, gridHeight), Random.Range(0, gridDepth));
            while (gridArray[randomCellId.x, randomCellId.y, randomCellId.z].dataAssigned != null)
            {
                randomCellId = new Vector3Int(Random.Range(0, gridWidth), Random.Range(0, gridHeight), Random.Range(0, gridDepth));
                if (gridArray[randomCellId.x, randomCellId.y, randomCellId.z].dataAssigned == null) break;
            }
            Vector3 worldPositionForCell = getGridPos(randomCellId.x, randomCellId.y, randomCellId.z);
            Vector3 posOffset = new Vector3(cellSizeX / 2, cellSizeY / 2, cellSizeZ / 2);

            gridArray[randomCellId.x, randomCellId.y, randomCellId.z].dataAssigned = gridArray[randomCellId.x, randomCellId.y, randomCellId.z].allowedAsssetsInCell[Random.Range(0, gridArray[randomCellId.x, randomCellId.y, randomCellId.z].allowedAsssetsInCell.Count)];

            //setAdjacentRules(currentCell.cellId, currentCell.dataAssigned);

            gridArray[randomCellId.x, randomCellId.y, randomCellId.z].cellObj = Instantiate(gridArray[randomCellId.x, randomCellId.y, randomCellId.z].dataAssigned.primaryAsset, worldPositionForCell + posOffset, Quaternion.identity, GameObject.Find("Enviroment Holder").transform);
        }
    }

    //This function will give all the adacent cells their requirment changes 
    //For example, a cell is collapsed into a sand block, and will tell all the adjacent cells that they can now only be either sand or water

    void setAdjacentRules(Vector3 cellId, assetData assetDataForAssignedCell)
    {
        //Set The Upper Adjacent Limits
        if (cellId.y != gridHeight - 1)
        {
            //For each allowed cell in the above cell, it will run these commands
            for (int i = 0; i < gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].allowedAsssetsInCell.Count; i++)
            {
                //This runs through all of the allowed assets for cell above, in each allowed asset in the prime cell
                for (int y = 0; y < assetDataForAssignedCell.allowedAssetsAbove.Length; y++)
                {
                    //This uses the Linq net framework to compare the allowed assets within the above adjacent cell, and if that array does not contain the allowed above asset of this spesific array within the prime array, it will remove it.
                    if (!gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].allowedAsssetsInCell.Contains(assetDataForAssignedCell.allowedAssetsAbove[y])) gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].allowedAsssetsInCell.RemoveAt(i);

                }
            }
        }
        //Set The Lower Adjacent Limits
        if (cellId.y != 0)
        {
            //For each allowed cell in the above cell, it will run these commands
            for (int i = 0; i < gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].allowedAsssetsInCell.Count; i++)
            {
                //This runs through all of the allowed assets for cell above, in each allowed asset in the prime cell
                for (int y = 0; y < assetDataForAssignedCell.allowedAssetsBelow.Length; y++)
                {
                    //This uses the Linq net framework to compare the allowed assets within the above adjacent cell, and if that array does not contain the allowed below asset of this spesific array within the prime array, it will remove it.
                    if (!gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].allowedAsssetsInCell.Contains(assetDataForAssignedCell.allowedAssetsBelow[y])) gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].allowedAsssetsInCell.RemoveAt(i);

                }
            }
        }
        //Set The Right Adjacent Limits
        if (cellId.y != gridWidth - 1)
        {
            //For each allowed cell in the above cell, it will run these commands
            for (int i = 0; i < gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].allowedAsssetsInCell.Count; i++)
            {
                //This runs through all of the allowed assets for cell above, in each allowed asset in the prime cell
                for (int y = 0; y < assetDataForAssignedCell.allowedAssetsRight.Length; y++)
                {
                    //This uses the Linq net framework to compare the allowed assets within the above adjacent cell, and if that array does not contain the allowed above asset of this spesific array within the prime array, it will remove it.
                    if (!gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].allowedAsssetsInCell.Contains(assetDataForAssignedCell.allowedAssetsRight[y])) gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].allowedAsssetsInCell.RemoveAt(i);

                }
            }
        }
        //Set The Left Adjacent Limits
        if (cellId.y != 0)
        {
            //For each allowed cell in the above cell, it will run these commands
            for (int i = 0; i < gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].allowedAsssetsInCell.Count; i++)
            {
                //This runs through all of the allowed assets for cell above, in each allowed asset in the prime cell
                for (int y = 0; y < assetDataForAssignedCell.allowedAssetsLeft.Length; y++)
                {
                    //This uses the Linq net framework to compare the allowed assets within the above adjacent cell, and if that array does not contain the allowed above asset of this spesific array within the prime array, it will remove it.
                    if (!gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].allowedAsssetsInCell.Contains(assetDataForAssignedCell.allowedAssetsLeft[y])) gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].allowedAsssetsInCell.RemoveAt(i);

                }
            }
        }
        //Set The Forward Adjacent Limits
        if (cellId.y != gridDepth - 1)
        {
            //For each allowed cell in the above cell, it will run these commands
            for (int i = 0; i < gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].allowedAsssetsInCell.Count; i++)
            {
                //This runs through all of the allowed assets for cell above, in each allowed asset in the prime cell
                for (int y = 0; y < assetDataForAssignedCell.allowedAssetsForward.Length; y++)
                {
                    //This uses the Linq net framework to compare the allowed assets within the above adjacent cell, and if that array does not contain the allowed above asset of this spesific array within the prime array, it will remove it.
                    if (!gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].allowedAsssetsInCell.Contains(assetDataForAssignedCell.allowedAssetsForward[y])) gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].allowedAsssetsInCell.RemoveAt(i);

                }
            }
        }
        //Set The Backward Adjacent Limits
        if (cellId.y != 0)
        {
            //For each allowed cell in the above cell, it will run these commands
            for (int i = 0; i < gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].allowedAsssetsInCell.Count; i++)
            {
                //This runs through all of the allowed assets for cell above, in each allowed asset in the prime cell
                for (int y = 0; y < assetDataForAssignedCell.allowedAssetsBackward.Length; y++)
                {
                    //This uses the Linq net framework to compare the allowed assets within the above adjacent cell, and if that array does not contain the allowed above asset of this spesific array within the prime array, it will remove it.
                    if (!gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].allowedAsssetsInCell.Contains(assetDataForAssignedCell.allowedAssetsBackward[y])) gridArray[(int)cellId.x, (int)cellId.y + 1, (int)cellId.z].allowedAsssetsInCell.RemoveAt(i);

                }
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

[System.Serializable]
//Cell struct with basic information required
public struct gridCell
{
    public Vector3Int cellId;
    public Vector3 cellPos;
    public Vector3 cellSize;
    public List<assetData> allowedAsssetsInCell;
    public assetData dataAssigned;
    public GameObject cellObj;

    public gridCell(Vector3Int Id, Vector3 Pos, Vector3 Size, assetDataList dataList)
    {
        cellId = Id;
        cellPos = Pos;
        cellSize = Size;
        allowedAsssetsInCell = dataList.listOfAssets;
        dataAssigned = null;
        cellObj = null;
    }
}