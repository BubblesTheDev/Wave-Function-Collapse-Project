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
    public Vector3Int gridSize;
    public Vector3 cellSizes;


    //Level Generator Settings
    public assetDataList dataList;
    public assetData voidAsset;
    public Vector3Int startingCellId;
    Vector3Int currentCellId;
    [SerializeField] assetData chosenData;
    List<Vector3Int> emptyCellIds;
    public axis axisToCheck;
    public axis adjacentAxis;

    //Debug options settings
    public bool displayCellIDs = false;
    public bool displayCellLines = false;
    public float timeBetweenSpawning;
    public Vector3Int IdToCheck;


    //Hidden variables
    [HideInInspector] public gridCell[,,] gridArray;

    private void Awake() {
        emptyCellIds = new List<Vector3Int>();
    }

    private void Update() {
        if (Input.GetKeyUp(KeyCode.Space)) SceneManager.LoadScene(0);
        if (Input.GetKeyUp(KeyCode.F2)) {
            foreach (assetData item in gridArray[IdToCheck.x, IdToCheck.y, IdToCheck.z].allowedAsssetsInCell) {
                print(item.name);
            }

        }
    }


    //This part of the code is now generating the map overall
    public IEnumerator generateMap() {
        if (gridArray == null) {
            Debug.LogWarning("There is no grid to generate an enviroment in");
            yield break;
        }

        if (startingCellId.x > gridSize.x || startingCellId.y > gridSize.y || startingCellId.z > gridSize.z) {
            Debug.LogWarning("The starting cell ID is greater than the grid size");
            yield break;
        }



        currentCellId = startingCellId;
        for (int y = 0; y < gridSize.y; y++) {

            //itterates through the grid on the horizontal axis adding the cell ids
            for (int i = 0; i < gridSize.x; i++) {
                for (int j = 0; j < gridSize.z; j++) {
                    emptyCellIds.Add(gridArray[i, y, j].cellId);
                }
            }

            for (int x = 0; x < gridSize.x; x++) {
                for (int z = 0; z < gridSize.z; z++) {

                    //This generates a new empty list with the empty adjacent cells compared to cell id we are currently at
                    List<Vector3Int> emptyAdjacentCells = GridGenerator.getEmptyAdjacentCellIds(currentCellId, gridArray, axisToCheck);

                    //If the itteration is at index 0 in all axies, the current cell id, should turn to the starting cell id, 
                    //otherwise it should go to a random adjacent cell, if there are no empty adjacent cells
                    //It will find a random cell on the same horizontal plane
                    if (x == 0 && y == 0 && z == 0) {
                        currentCellId = startingCellId;
                    } else if (emptyAdjacentCells.Count > 0) {
                        currentCellId = emptyAdjacentCells[Random.Range(0, emptyAdjacentCells.Count)];
                    } else if (emptyAdjacentCells.Count <= 0) {
                        currentCellId = emptyCellIds[Random.Range(0, emptyCellIds.Count - 1)];
                    }




                    emptyAdjacentCells = GridGenerator.getEmptyAdjacentCellIds(currentCellId, gridArray, axisToCheck);
                    
                    //This accesses the grid generator function to get the world position of a cell, and then creates a new position for it with the offset size of each cell to find the center of that cell
                    Vector3 worldPositionForCell = GridGenerator.getWorldPosOfCell(currentCellId, cellSizes, cellStartingPos);
                    Vector3 posOffset = new Vector3(cellSizes.x / 2, cellSizes.y / 2, cellSizes.z/ 2);


                    if (gridArray[currentCellId.x, currentCellId.y, currentCellId.z].allowedAsssetsInCell.Count > 0) {
                        
                        //This creates a random decimal from 0.01 to 1 to create an artifical percentage
                        float assetChance = Random.Range(0.01f, 1f);

                        for (int i = 0; i < gridArray[currentCellId.x, currentCellId.y, currentCellId.z].allowedAsssetsInCell.Count; i++) {
                            //This will then go through all the allowed assets within the current selected cell.
                            if (assetChance < gridArray[currentCellId.x, currentCellId.y, currentCellId.z].allowedAsssetsInCell[i].percentageChanceOfAsset) {

                                //If the asset chance is lower than the asset chance of the current allowed asset's spesific percentage, it will set the chosen date of this cell to its selected asset adn break out of this.
                                chosenData = gridArray[currentCellId.x, currentCellId.y, currentCellId.z].allowedAsssetsInCell[i];
                                break;
                            }
                        }

                        //If no asset data is chosen by the end, it will choose the default, which is the final asset (the asset with the highest chance)
                        if (chosenData == null) chosenData = gridArray[currentCellId.x, currentCellId.y, currentCellId.z].allowedAsssetsInCell[gridArray[currentCellId.x, currentCellId.y, currentCellId.z].allowedAsssetsInCell.Count - 1];
                    }

                    //This will then set the data assigned to the chosen data and set the new adjacent rules
                    gridArray[currentCellId.x, currentCellId.y, currentCellId.z].dataAssigned = chosenData;
                    setAdjacentRules(currentCellId, adjacentAxis);


                    //This then detects if there is a holder for that spesific asset, if there isnt a holder for that asset, it will then create an empty game object under
                    //the "Enviroment Holder" game object to then hold any asset that is created.
                    if (!GameObject.Find(chosenData.name.Split(" ").FirstOrDefault() + " Holder")) {
                        GameObject tempObj = new GameObject(chosenData.name.Split(" ").FirstOrDefault() + " Holder");
                        tempObj.transform.parent = GameObject.Find("Enviroment Holder").transform;
                    }

                    //This is just a switch statement to rotate the asset to spawn depending on which facing direction it is set to.
                    switch (chosenData.facingDir) {
                        case possibleFacingDirections.Right:
                        gridArray[currentCellId.x, currentCellId.y, currentCellId.z].cellObj =
                            Instantiate(gridArray[currentCellId.x, currentCellId.y, currentCellId.z].dataAssigned.primaryAsset, worldPositionForCell + posOffset, Quaternion.LookRotation(Vector3.right, Vector3.up), GameObject.Find(chosenData.name.Split(" ").FirstOrDefault() + " Holder").transform);
                        break;
                        case possibleFacingDirections.Left:
                        gridArray[currentCellId.x, currentCellId.y, currentCellId.z].cellObj =
                            Instantiate(gridArray[currentCellId.x, currentCellId.y, currentCellId.z].dataAssigned.primaryAsset, worldPositionForCell + posOffset, Quaternion.LookRotation(-Vector3.right, Vector3.up), GameObject.Find(chosenData.name.Split(" ").FirstOrDefault() + " Holder").transform);
                        break;
                        case possibleFacingDirections.Forward:
                        gridArray[currentCellId.x, currentCellId.y, currentCellId.z].cellObj =
                            Instantiate(gridArray[currentCellId.x, currentCellId.y, currentCellId.z].dataAssigned.primaryAsset, worldPositionForCell + posOffset, Quaternion.LookRotation(Vector3.forward, Vector3.up), GameObject.Find(chosenData.name.Split(" ").FirstOrDefault() + " Holder").transform);
                        break;
                        case possibleFacingDirections.Backward:
                        gridArray[currentCellId.x, currentCellId.y, currentCellId.z].cellObj =
                            Instantiate(gridArray[currentCellId.x, currentCellId.y, currentCellId.z].dataAssigned.primaryAsset, worldPositionForCell + posOffset, Quaternion.LookRotation(-Vector3.forward, Vector3.up), GameObject.Find(chosenData.name.Split(" ").FirstOrDefault() + " Holder").transform);
                        break;
                    }

                    //Then sets the cell obj's name to the Name plus its cell id,
                    gridArray[currentCellId.x, currentCellId.y, currentCellId.z].cellObj.name = gridArray[currentCellId.x, currentCellId.y, currentCellId.z].dataAssigned.name + ": " + currentCellId.x + "," + currentCellId.y + "," + currentCellId.z;

                    //Removes this spesific cell from the empty cell ids from a custom list
                    emptyCellIds.Remove(currentCellId);

                    yield return new WaitForSeconds(timeBetweenSpawning);
                }
            }
        }

        //Once all normal assets are created, it will then find the void game object holder and delete it if it exists
        if (voidAsset != null) {
            Destroy(GameObject.Find("(Void) Holder"));
        }

        print("Finished Generating Enviroment");
    }


    public void generateMapInstant() {
        if (gridArray == null)
        {
            Debug.LogWarning("There is no grid to generate an enviroment in");
            return;
        }

        if (startingCellId.x > gridSize.x || startingCellId.y > gridSize.y || startingCellId.z > gridSize.z)
        {
            Debug.LogWarning("The starting cell ID is greater than the grid size");
            return;
        }



        currentCellId = startingCellId;
        for (int y = 0; y < gridSize.y; y++)
        {

            //itterates through the grid on the horizontal axis adding the cell ids
            for (int i = 0; i < gridSize.x; i++)
            {
                for (int j = 0; j < gridSize.z; j++)
                {
                    emptyCellIds.Add(gridArray[i, y, j].cellId);
                }
            }

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {

                    //This generates a new empty list with the empty adjacent cells compared to cell id we are currently at
                    List<Vector3Int> emptyAdjacentCells = GridGenerator.getEmptyAdjacentCellIds(currentCellId, gridArray, axisToCheck);

                    //If the itteration is at index 0 in all axies, the current cell id, should turn to the starting cell id, 
                    //otherwise it should go to a random adjacent cell, if there are no empty adjacent cells
                    //It will find a random cell on the same horizontal plane
                    if (x == 0 && y == 0 && z == 0)
                    {
                        currentCellId = startingCellId;
                    }
                    else if (emptyAdjacentCells.Count > 0)
                    {
                        currentCellId = emptyAdjacentCells[Random.Range(0, emptyAdjacentCells.Count)];
                    }
                    else if (emptyAdjacentCells.Count <= 0)
                    {
                        currentCellId = emptyCellIds[Random.Range(0, emptyCellIds.Count - 1)];
                    }




                    emptyAdjacentCells = GridGenerator.getEmptyAdjacentCellIds(currentCellId, gridArray, axisToCheck);

                    //This accesses the grid generator function to get the world position of a cell, and then creates a new position for it with the offset size of each cell to find the center of that cell
                    Vector3 worldPositionForCell = GridGenerator.getWorldPosOfCell(currentCellId, cellSizes, cellStartingPos);
                    Vector3 posOffset = new Vector3(cellSizes.x / 2, cellSizes.y / 2, cellSizes.z / 2);


                    if (gridArray[currentCellId.x, currentCellId.y, currentCellId.z].allowedAsssetsInCell.Count > 0)
                    {

                        //This creates a random decimal from 0.01 to 1 to create an artifical percentage
                        float assetChance = Random.Range(0.01f, 1f);

                        for (int i = 0; i < gridArray[currentCellId.x, currentCellId.y, currentCellId.z].allowedAsssetsInCell.Count; i++)
                        {
                            //This will then go through all the allowed assets within the current selected cell.
                            if (assetChance < gridArray[currentCellId.x, currentCellId.y, currentCellId.z].allowedAsssetsInCell[i].percentageChanceOfAsset)
                            {

                                //If the asset chance is lower than the asset chance of the current allowed asset's spesific percentage, it will set the chosen date of this cell to its selected asset adn break out of this.
                                chosenData = gridArray[currentCellId.x, currentCellId.y, currentCellId.z].allowedAsssetsInCell[i];
                                break;
                            }
                        }

                        //If no asset data is chosen by the end, it will choose the default, which is the final asset (the asset with the highest chance)
                        if (chosenData == null) chosenData = gridArray[currentCellId.x, currentCellId.y, currentCellId.z].allowedAsssetsInCell[gridArray[currentCellId.x, currentCellId.y, currentCellId.z].allowedAsssetsInCell.Count - 1];
                    }

                    //This will then set the data assigned to the chosen data and set the new adjacent rules
                    gridArray[currentCellId.x, currentCellId.y, currentCellId.z].dataAssigned = chosenData;
                    setAdjacentRules(currentCellId, adjacentAxis);


                    //This then detects if there is a holder for that spesific asset, if there isnt a holder for that asset, it will then create an empty game object under
                    //the "Enviroment Holder" game object to then hold any asset that is created.
                    if (!GameObject.Find(chosenData.name.Split(" ").FirstOrDefault() + " Holder"))
                    {
                        GameObject tempObj = new GameObject(chosenData.name.Split(" ").FirstOrDefault() + " Holder");
                        tempObj.transform.parent = GameObject.Find("Enviroment Holder").transform;
                    }

                    //This is just a switch statement to rotate the asset to spawn depending on which facing direction it is set to.
                    switch (chosenData.facingDir)
                    {
                        case possibleFacingDirections.Right:
                            gridArray[currentCellId.x, currentCellId.y, currentCellId.z].cellObj =
                                Instantiate(gridArray[currentCellId.x, currentCellId.y, currentCellId.z].dataAssigned.primaryAsset, worldPositionForCell + posOffset, Quaternion.LookRotation(Vector3.right, Vector3.up), GameObject.Find(chosenData.name.Split(" ").FirstOrDefault() + " Holder").transform);
                            break;
                        case possibleFacingDirections.Left:
                            gridArray[currentCellId.x, currentCellId.y, currentCellId.z].cellObj =
                                Instantiate(gridArray[currentCellId.x, currentCellId.y, currentCellId.z].dataAssigned.primaryAsset, worldPositionForCell + posOffset, Quaternion.LookRotation(-Vector3.right, Vector3.up), GameObject.Find(chosenData.name.Split(" ").FirstOrDefault() + " Holder").transform);
                            break;
                        case possibleFacingDirections.Forward:
                            gridArray[currentCellId.x, currentCellId.y, currentCellId.z].cellObj =
                                Instantiate(gridArray[currentCellId.x, currentCellId.y, currentCellId.z].dataAssigned.primaryAsset, worldPositionForCell + posOffset, Quaternion.LookRotation(Vector3.forward, Vector3.up), GameObject.Find(chosenData.name.Split(" ").FirstOrDefault() + " Holder").transform);
                            break;
                        case possibleFacingDirections.Backward:
                            gridArray[currentCellId.x, currentCellId.y, currentCellId.z].cellObj =
                                Instantiate(gridArray[currentCellId.x, currentCellId.y, currentCellId.z].dataAssigned.primaryAsset, worldPositionForCell + posOffset, Quaternion.LookRotation(-Vector3.forward, Vector3.up), GameObject.Find(chosenData.name.Split(" ").FirstOrDefault() + " Holder").transform);
                            break;
                    }

                    //Then sets the cell obj's name to the Name plus its cell id,
                    gridArray[currentCellId.x, currentCellId.y, currentCellId.z].cellObj.name = gridArray[currentCellId.x, currentCellId.y, currentCellId.z].dataAssigned.name + ": " + currentCellId.x + "," + currentCellId.y + "," + currentCellId.z;

                    //Removes this spesific cell from the empty cell ids from a custom list
                    emptyCellIds.Remove(currentCellId);

                }
            }
        }

        //Once all normal assets are created, it will then find the void game object holder and delete it if it exists
        if (voidAsset != null)
        {
            Destroy(GameObject.Find("(Void) Holder"));
        }

        print("Finished Generating Enviroment");
    }

    //This function sets the adjacent rules of a prime cell
    void setAdjacentRules(Vector3Int cellId, axis axisToCheck) {
        //Set The Upper Adjacent Limits
        if (cellId.y != gridSize.y - 1 && gridArray[cellId.x, cellId.y + 1, cellId.z].dataAssigned == null && axisToCheck.HasFlag(axis.yAxis)) {
            //This sets the list of the cell above to only assets that intersect with the allowed assets within the prime asset
            gridArray[cellId.x, cellId.y + 1, cellId.z].allowedAsssetsInCell =
                gridArray[cellId.x, cellId.y + 1, cellId.z].allowedAsssetsInCell.
                Intersect(gridArray[cellId.x, cellId.y, cellId.z].dataAssigned.allowedAssetsAbove).ToList();
        }

        //Set The Lower Adjacent Limits
        if (cellId.y != 0 && gridArray[cellId.x, cellId.y - 1, cellId.z].dataAssigned == null && axisToCheck.HasFlag(axis.yAxis)) {
            //This sets the list of the cell below to only assets that intersect with the allowed assets within the prime asset
            gridArray[cellId.x, cellId.y - 1, cellId.z].allowedAsssetsInCell = 
                gridArray[cellId.x, cellId.y - 1, cellId.z].allowedAsssetsInCell.
                Intersect(gridArray[cellId.x, cellId.y, cellId.z].dataAssigned.allowedAssetsBelow).ToList();

        }

        //Set The Right Adjacent Limits
        if (cellId.x != gridSize.x - 1 && gridArray[cellId.x + 1, cellId.y, cellId.z].dataAssigned == null && axisToCheck.HasFlag(axis.yAxis)) {
            //This sets the list of the cell to the right to only assets that intersect with the allowed assets within the prime asset
            gridArray[cellId.x + 1, cellId.y, cellId.z].allowedAsssetsInCell = 
                gridArray[cellId.x + 1, cellId.y, cellId.z].allowedAsssetsInCell.
                Intersect(gridArray[cellId.x, cellId.y, cellId.z].dataAssigned.allowedAssetsRight).ToList();


        }

        //Set The Left Adjacent Limits
        if (cellId.x != 0 && gridArray[cellId.x - 1, cellId.y, cellId.z].dataAssigned == null && axisToCheck.HasFlag(axis.yAxis)) {
            //This sets the list of the cell to the left to only assets that intersect with the allowed assets within the prime asset
            gridArray[cellId.x - 1, cellId.y, cellId.z].allowedAsssetsInCell = 
                gridArray[cellId.x - 1, cellId.y, cellId.z].allowedAsssetsInCell.
                Intersect(gridArray[cellId.x, cellId.y, cellId.z].dataAssigned.allowedAssetsLeft).ToList();

        }

        //Set The Forward Adjacent Limits
        if (cellId.z != gridSize.z - 1 && gridArray[cellId.x, cellId.y, cellId.z + 1].dataAssigned == null && axisToCheck.HasFlag(axis.zAxis)) {
            //This sets the list of the cell in front to only assets that intersect with the allowed assets within the prime asset
            gridArray[cellId.x, cellId.y, cellId.z + 1].allowedAsssetsInCell = 
                gridArray[cellId.x, cellId.y, cellId.z + 1].allowedAsssetsInCell.
                Intersect(gridArray[cellId.x, cellId.y, cellId.z].dataAssigned.allowedAssetsForward).ToList();

        }

        //Set The Backward Adjacent Limits
        if (cellId.z != 0 && gridArray[cellId.x, cellId.y, cellId.z - 1].dataAssigned == null && axisToCheck.HasFlag(axis.zAxis)) {
            //This sets the list of the cell behind to only assets that intersect with the allowed assets within the prime asset
            gridArray[cellId.x, cellId.y, cellId.z - 1].allowedAsssetsInCell = 
                gridArray[cellId.x, cellId.y, cellId.z - 1].allowedAsssetsInCell.
                Intersect(gridArray[cellId.x, cellId.y, cellId.z].dataAssigned.allowedAssetsBackward).ToList();

        }
    }


    private void OnDrawGizmos() {
        //to stop errors from happening if the array is not initalized
        if (gridArray != null) {
            if (displayCellIDs) {
                //itterates through each cell and places a lable explaining its Cell ID
                for (int x = 0; x < gridArray.GetLength(0); x++) {
                    for (int y = 0; y < gridArray.GetLength(1); y++) {
                        for (int z = 0; z < gridArray.GetLength(2); z++) {

                            Vector3 handleOffset = new Vector3(gridArray[x, y, z].cellPos.x + (cellSizes.x / 2), gridArray[x, y, z].cellPos.y + (cellSizes.y / 2), gridArray[x, y, z].cellPos.z + (cellSizes.z/ 2));
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
                        float lineZPos = cellStartingPos.transform.position.z + (z * cellSizes.z);
                        float lineYPos = cellStartingPos.transform.position.y + (y * cellSizes.y);
                        Vector3 lineStartPos = new Vector3(cellStartingPos.transform.position.x, lineYPos, lineZPos);
                        Vector3 lineEndPos = new Vector3(cellStartingPos.transform.position.x + (gridSize.x * cellSizes.x), lineYPos, lineZPos);

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
                        float lineXPos = cellStartingPos.transform.position.x + (x * cellSizes.x);
                        float lineZPos = cellStartingPos.transform.position.z + (z * cellSizes.z);
                        Vector3 lineStartPos = new Vector3(lineXPos, cellStartingPos.transform.position.y, lineZPos);
                        Vector3 lineEndPos = new Vector3(lineXPos, cellStartingPos.transform.position.y + (gridSize.y * cellSizes.y), lineZPos);

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
                        float lineXPos = cellStartingPos.transform.position.x + (x * cellSizes.x);
                        float lineYPos = cellStartingPos.transform.position.y + (y * cellSizes.y);
                        Vector3 lineStartPos = new Vector3(lineXPos, lineYPos, cellStartingPos.transform.position.z);
                        Vector3 lineEndPos = new Vector3(lineXPos, lineYPos, cellStartingPos.transform.position.z + (gridSize.z * cellSizes.z));

                        Gizmos.DrawLine(lineStartPos, lineEndPos);
                    }
                }
            }
        }


    }
}