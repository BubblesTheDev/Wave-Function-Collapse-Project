using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.SceneManagement;

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
    public assetData voidAsset;
    public Vector3Int startingCellId;
    Vector3Int randomCellId;
    public assetPercentage[] currentEnviromentPercentages;
    List<Vector3Int> adjacentEmptyCells;
    List<assetData> fullAssets;
     assetData chosenData;
    //Debug options settings
    public bool displayCellIDs = false;
    public bool displayCellLines = false;
    public bool advancedSettings = false;
    public float timeBetweenSpawning;


    //Hidden variables
    private gridCell[,,] gridArray;

    private void Awake()
    {
        regenerateGrid();
        currentEnviromentPercentages = new assetPercentage[dataList.listOfAssets.Count];
        for (int i = 0; i < currentEnviromentPercentages.Length; i++)
        {
            currentEnviromentPercentages[i] = new assetPercentage(dataList.listOfAssets[i]);
        }
        fullAssets = new List<assetData>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space)) SceneManager.LoadScene(0);
        if (Input.GetKeyUp(KeyCode.F1)) StartCoroutine(generateMap());
        if(Input.GetKeyUp(KeyCode.LeftAlt))
        {
            foreach (assetData item in gridArray[randomCellId.x, randomCellId.y, randomCellId.z].allowedAsssetsInCell)
            {
                print(item);
            }
        }

        if(currentEnviromentPercentages.Length != 0) updatePercentageCount();

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
                    gridArray[x, y, z] = new gridCell(tempId, getCellPosInWorldspace(x, y, z), tempSize, dataList);
                }
            }
        }
    }

    //This part of the code is now generating the map overall
    public IEnumerator generateMap()
    {
        if(gridArray[0,0,0].cellObj != null)
        {
            Debug.LogWarning("You have already generated the enviroment, press the space key to restart the scene");
            yield return null;
        }



        for (int x = 0; x < gridHeight; x++)
        {
            List<Vector3Int> emptySpaceIds = new List<Vector3Int>();
            for (int a = 0; a < gridWidth; a++)
            {
                for (int b = 0; b < gridDepth; b++)
                {
                    if(gridArray[a, x, b].dataAssigned == null) emptySpaceIds.Add(gridArray[a, x, b].cellId);
                }
            }

            for (int emptySpacesLeftOnLevel = gridWidth * gridDepth; emptySpacesLeftOnLevel >= 1;)
            {

                if (emptySpaceIds.Count == 0) break;
                if (emptySpacesLeftOnLevel == gridWidth * gridDepth && x == 0)
                {
                    randomCellId = startingCellId;
                }
                else if (emptySpacesLeftOnLevel < gridWidth * gridDepth && adjacentEmptyCells.Count > 0)
                {
                    randomCellId = adjacentEmptyCells[Random.Range(0, adjacentEmptyCells.Count)];
                }
                else if (emptySpacesLeftOnLevel < gridWidth * gridDepth && adjacentEmptyCells.Count == 0)
                {
                    randomCellId = emptySpaceIds[Random.Range(0, emptySpaceIds.Count)];
                }


                adjacentEmptyCells = getAdjacentCells(randomCellId,true);
                Vector3 worldPositionForCell = getCellPosInWorldspace(randomCellId.x, randomCellId.y, randomCellId.z);
                Vector3 posOffset = new Vector3(cellSizeX / 2, cellSizeY / 2, cellSizeZ / 2);

                if (fullAssets.Count > 0)
                    gridArray[randomCellId.x, randomCellId.y, randomCellId.z].allowedAsssetsInCell = gridArray[randomCellId.x, randomCellId.y, randomCellId.z].allowedAsssetsInCell.Except(fullAssets).ToList();

                //print("The allowed cells count of cell id " + randomCellId + " is: " + gridArray[randomCellId.x, randomCellId.y, randomCellId.z].allowedAsssetsInCell.Count);


                
                if (gridArray[randomCellId.x, randomCellId.y, randomCellId.z].allowedAsssetsInCell.Count >= 1
                    || gridArray[randomCellId.x, randomCellId.y, randomCellId.z].allowedAsssetsInCell.Count == 1 
                    && gridArray[randomCellId.x, randomCellId.y, randomCellId.z].allowedAsssetsInCell[0] == voidAsset)
                {
                    chosenData = gridArray[randomCellId.x, randomCellId.y, randomCellId.z].allowedAsssetsInCell[(int)Random.Range(0, gridArray[randomCellId.x, randomCellId.y, randomCellId.z].allowedAsssetsInCell.Count)];

                }
                //else if (gridArray[randomCellId.x, randomCellId.y, randomCellId.z].allowedAsssetsInCell.Count == 1 && gridArray[randomCellId.x, randomCellId.y, randomCellId.z].allowedAsssetsInCell[0] != voidAsset) chosenData = gridArray[randomCellId.x, randomCellId.y, randomCellId.z].allowedAsssetsInCell[0];
                else chosenData = bruteForceData(randomCellId);
                while (chosenData.minVerticalLevel > x || chosenData.maxVerticalLevel < x )
                {
                    if(gridArray[randomCellId.x, randomCellId.y, randomCellId.z].allowedAsssetsInCell.Count <= 0 
                        || gridArray[randomCellId.x, randomCellId.y, randomCellId.z].allowedAsssetsInCell.Count == 1 
                        && gridArray[randomCellId.x, randomCellId.y, randomCellId.z].allowedAsssetsInCell[0] == voidAsset) chosenData = bruteForceData(randomCellId);
                    else chosenData = gridArray[randomCellId.x, randomCellId.y, randomCellId.z].allowedAsssetsInCell[(int)Random.Range(0, gridArray[randomCellId.x, randomCellId.y, randomCellId.z].allowedAsssetsInCell.Count)];
                    if (chosenData.minVerticalLevel <= x && chosenData.maxVerticalLevel >= x)
                    {
                        break;
                    }
                    yield return null;
                }

                gridArray[randomCellId.x, randomCellId.y, randomCellId.z].dataAssigned = chosenData;
                setAdjacentRules(gridArray[randomCellId.x, randomCellId.y, randomCellId.z].cellId);


                if (currentEnviromentPercentages[returnProperAssetPercentage(chosenData)].numOfAssetCollapsed == 0)
                {
                    GameObject tempObj = new GameObject(chosenData.name + " Holder");
                    tempObj.transform.parent = GameObject.Find("Enviroment Holder").transform;
                }

                switch (chosenData.facingDir)
                {
                    case possibleFacingDirections.Right:
                        gridArray[randomCellId.x, randomCellId.y, randomCellId.z].cellObj = 
                            Instantiate(gridArray[randomCellId.x, randomCellId.y, randomCellId.z].dataAssigned.primaryAsset, worldPositionForCell + posOffset, Quaternion.LookRotation(Vector3.right,Vector3.up), GameObject.Find(chosenData.name + " Holder").transform);
                        break;
                    case possibleFacingDirections.Left:
                        gridArray[randomCellId.x, randomCellId.y, randomCellId.z].cellObj =
                            Instantiate(gridArray[randomCellId.x, randomCellId.y, randomCellId.z].dataAssigned.primaryAsset, worldPositionForCell + posOffset, Quaternion.LookRotation(-Vector3.right, Vector3.up), GameObject.Find(chosenData.name + " Holder").transform);
                        break;
                    case possibleFacingDirections.Forward:
                        gridArray[randomCellId.x, randomCellId.y, randomCellId.z].cellObj =
                            Instantiate(gridArray[randomCellId.x, randomCellId.y, randomCellId.z].dataAssigned.primaryAsset, worldPositionForCell + posOffset, Quaternion.LookRotation(Vector3.forward, Vector3.up), GameObject.Find(chosenData.name + " Holder").transform);
                        break;
                    case possibleFacingDirections.Backward:
                        gridArray[randomCellId.x, randomCellId.y, randomCellId.z].cellObj =
                            Instantiate(gridArray[randomCellId.x, randomCellId.y, randomCellId.z].dataAssigned.primaryAsset, worldPositionForCell + posOffset, Quaternion.LookRotation(-Vector3.forward, Vector3.up), GameObject.Find(chosenData.name + " Holder").transform);
                        break;
                }
                if(gridArray[randomCellId.x, randomCellId.y, randomCellId.z].cellObj != null) currentEnviromentPercentages[returnProperAssetPercentage(chosenData)].numOfAssetCollapsed++;
                if (currentEnviromentPercentages[returnProperAssetPercentage(chosenData)].currentAssetPercentage >= chosenData.percentageAllowed && !fullAssets.Contains(chosenData))
                {
                    //print("Removing " + chosenData.name + " from the allowed spawning pool");
                    fullAssets.Add(chosenData);
                }

                emptySpaceIds.Remove(gridArray[randomCellId.x, randomCellId.y, randomCellId.z].cellId);
                emptySpacesLeftOnLevel--;

                if (fullAssets.Count == dataList.listOfAssets.Count - 1 && !fullAssets.Contains(voidAsset)) yield break;

                yield return new WaitForSeconds(timeBetweenSpawning);
            }
            if (fullAssets.Count == dataList.listOfAssets.Count - 1 && !fullAssets.Contains(voidAsset)) yield break;
        }
        if (voidAsset != null) 
        {
            Destroy(GameObject.Find(voidAsset.name + " Holder"));
        }

        print("Finished Generating Enviroment");
    }

    //This will brute force place objects no matter the percentile limit, as a last resort so no errors will appear
    assetData bruteForceData(Vector3Int cellId)
    {

        List<assetData> tempDataList;

        tempDataList = dataList.listOfAssets;

        if(cellId.x !=0 && gridArray[cellId.x - 1, cellId.y, cellId.z].cellObj != null) tempDataList = tempDataList.Intersect(gridArray[cellId.x-1, cellId.y, cellId.z].dataAssigned.allowedAssetsRight).ToList();
        if (cellId.x != gridWidth-1 && gridArray[cellId.x + 1, cellId.y, cellId.z].cellObj != null) tempDataList = tempDataList.Intersect(gridArray[cellId.x+1, cellId.y, cellId.z].dataAssigned.allowedAssetsLeft).ToList();
        if (cellId.z != 0 && gridArray[cellId.x, cellId.y, cellId.z - 1].cellObj != null) tempDataList = tempDataList.Intersect(gridArray[cellId.x, cellId.y, cellId.z-1].dataAssigned.allowedAssetsForward).ToList();
        if (cellId.z != gridDepth-1 && gridArray[cellId.x, cellId.y, cellId.z + 1].cellObj != null) tempDataList = tempDataList.Intersect(gridArray[cellId.x, cellId.y, cellId.z+1].dataAssigned.allowedAssetsBackward).ToList();

        fullAssets = new List<assetData>();
        tempDataList.Remove(voidAsset);
        return (tempDataList[Random.Range(0, tempDataList.Count)]);
    }

    //This function sets the adjacent rules of a prime cell
    void setAdjacentRules(Vector3Int cellId)
    {
        //Set The Upper Adjacent Limits
        if (cellId.y != gridHeight - 1 && gridArray[cellId.x, cellId.y + 1, cellId.z].dataAssigned == null)
        {
            gridArray[cellId.x, cellId.y + 1, cellId.z].allowedAsssetsInCell = gridArray[cellId.x, cellId.y + 1, cellId.z].allowedAsssetsInCell.Intersect(gridArray[cellId.x, cellId.y, cellId.z].dataAssigned.allowedAssetsAbove).ToList();
        }

        //Set The Lower Adjacent Limits
        if (cellId.y != 0 && gridArray[cellId.x, cellId.y - 1, cellId.z].dataAssigned == null)
        {
            gridArray[cellId.x, cellId.y - 1, cellId.z].allowedAsssetsInCell = gridArray[cellId.x, cellId.y - 1, cellId.z].allowedAsssetsInCell.Intersect(gridArray[cellId.x, cellId.y, cellId.z].dataAssigned.allowedAssetsBelow).ToList();

        }

        //Set The Right Adjacent Limits
        if (cellId.x != gridWidth - 1 && gridArray[cellId.x + 1, cellId.y, cellId.z].dataAssigned == null)
        {
            gridArray[cellId.x + 1, cellId.y, cellId.z].allowedAsssetsInCell = gridArray[cellId.x + 1, cellId.y, cellId.z].allowedAsssetsInCell.Intersect(gridArray[cellId.x, cellId.y, cellId.z].dataAssigned.allowedAssetsRight).ToList();


        }

        //Set The Left Adjacent Limits
        if (cellId.x != 0 && gridArray[cellId.x - 1, cellId.y, cellId.z].dataAssigned == null)
        {
            gridArray[cellId.x - 1, cellId.y, cellId.z].allowedAsssetsInCell = gridArray[cellId.x - 1, cellId.y, cellId.z].allowedAsssetsInCell.Intersect(gridArray[cellId.x, cellId.y, cellId.z].dataAssigned.allowedAssetsLeft).ToList();

        }

        //Set The Forward Adjacent Limits
        if (cellId.z != gridDepth - 1 && gridArray[cellId.x, cellId.y, cellId.z + 1].dataAssigned == null)
        {
            gridArray[cellId.x, cellId.y, cellId.z + 1].allowedAsssetsInCell = gridArray[cellId.x, cellId.y, cellId.z + 1].allowedAsssetsInCell.Intersect(gridArray[cellId.x, cellId.y, cellId.z].dataAssigned.allowedAssetsForward).ToList();
            
        }

        //Set The Backward Adjacent Limits
        if (cellId.z != 0 && gridArray[cellId.x, cellId.y, cellId.z - 1].dataAssigned == null)
        {
            gridArray[cellId.x, cellId.y, cellId.z - 1].allowedAsssetsInCell = gridArray[cellId.x, cellId.y, cellId.z - 1].allowedAsssetsInCell.Intersect(gridArray[cellId.x, cellId.y, cellId.z].dataAssigned.allowedAssetsBackward).ToList();

        }
    }

    //This function returns all empty adjacent cells of a prime cell
    List<Vector3Int> getAdjacentCells(Vector3 cellId, bool getEmpty)
    {
        List<Vector3Int> cellsToReturn = new List<Vector3Int>(0);
        if (getEmpty)
        {
            if (cellId.x != gridWidth - 1)
                if (gridArray[(int)cellId.x + 1, (int)cellId.y, (int)cellId.z].dataAssigned == null) cellsToReturn.Add(gridArray[(int)cellId.x + 1, (int)cellId.y, (int)cellId.z].cellId);
            if (cellId.x != 0)
                if (gridArray[(int)cellId.x - 1, (int)cellId.y, (int)cellId.z].dataAssigned == null) cellsToReturn.Add(gridArray[(int)cellId.x - 1, (int)cellId.y, (int)cellId.z].cellId);
            if (cellId.z != gridDepth - 1)
                if (gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z + 1].dataAssigned == null) cellsToReturn.Add(gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z + 1].cellId);
            if (cellId.z != 0)
                if (gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z - 1].dataAssigned == null) cellsToReturn.Add(gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z - 1].cellId);
        } else if (!getEmpty)
        {
            if (cellId.x != gridWidth - 1)
                cellsToReturn.Add(gridArray[(int)cellId.x + 1, (int)cellId.y, (int)cellId.z].cellId);
            if (cellId.x != 0)
                cellsToReturn.Add(gridArray[(int)cellId.x - 1, (int)cellId.y, (int)cellId.z].cellId);
            if (cellId.z != gridDepth - 1)
                cellsToReturn.Add(gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z + 1].cellId);
            if (cellId.z != 0)
                cellsToReturn.Add(gridArray[(int)cellId.x, (int)cellId.y, (int)cellId.z - 1].cellId);
        }



        return cellsToReturn;
    }

    //gets the grid position of the cell in real world space
    public Vector3 getCellPosInWorldspace(int xId, int yId, int zId)
    {
        float tempPosX;
        float tempPosY;
        float tempPosZ;

        tempPosX = cellStartingPos.transform.position.x + (xId * cellSizeX);
        tempPosY = cellStartingPos.transform.position.y + (yId * cellSizeY);
        tempPosZ = cellStartingPos.transform.position.z + (zId * cellSizeZ);

        return new Vector3(tempPosX, tempPosY, tempPosZ);
    }

    //Updates The Current percentage Count of an asset
    void updatePercentageCount()
    {
        for (int i = 0; i < currentEnviromentPercentages.Length - 1; i++)
        {
            currentEnviromentPercentages[i].currentAssetPercentage = (float)currentEnviromentPercentages[i].numOfAssetCollapsed / (gridWidth * gridDepth * gridHeight);
        }
    }

    //Returns the asset percentage index of the asset given
    int returnProperAssetPercentage(assetData assetToGive)
    {
        for (int i = 0; i < currentEnviromentPercentages.Length; i++)
        {
            if (currentEnviromentPercentages[i].assetDataForPercentage == assetToGive) return i;
        }
        return 0;
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
        allowedAsssetsInCell = new List<assetData>();
        for (int i = 0; i < dataList.listOfAssets.Count; i++)
        {
            allowedAsssetsInCell.Add(dataList.listOfAssets[i]);
        }
        dataAssigned = null;
        cellObj = null;
    }
}
[System.Serializable]
public class assetPercentage
{
    public float currentAssetPercentage;
    public int numOfAssetCollapsed;
    public assetData assetDataForPercentage;

    public assetPercentage(assetData dataTogive)
    {
        assetDataForPercentage = dataTogive;
        currentAssetPercentage = 0;
    }
}