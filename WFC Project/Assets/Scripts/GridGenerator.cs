using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public static class GridGenerator {

    //A function to generate a grid that returns a 3d array, used in several places, mostly to create the grid that the assets will embody
    public static gridCell[,,] generateGrid(Vector3Int gridSize, Vector3 cellSizes, assetDataList listOfAssets, GameObject gridStartingPos) {

        //makes sure there are no errors for grid size
        if (gridSize.x == 0 || gridSize.y == 0 || gridSize.z == 0) Debug.LogError("The grid given, does not have a size above 0 in one of its axis");

        //Creates a new 3d array with the spesified sizes
        gridCell[,,] gridToReturn = new gridCell[gridSize.x,gridSize.y,gridSize.z];


        //Itterates through each axis of the array and sets its allowed assets for the cell into what is allowed in it.
        for (int y  = 0; y < gridSize.y; y++) {
            for (int x = 0; x < gridSize.z; x++) {
                for (int z = 0; z < gridSize.z; z++) {

                    Vector3Int tempCellId = new Vector3Int(x,y,z);

                    //Calls the get allowed starter assets, and sets a new assetData list to these allowed starter assets
                    List<assetData> allowedAssetsForThisCell = GridGenerator.getAllowedStarterAssets(listOfAssets, tempCellId, gridSize);

                    //Then creates a new gridcell at the index, and assigns its ID, its world position of the cell, its size, and its allowed starter assets
                    gridToReturn[x, y, z] = new gridCell(tempCellId, getWorldPosOfCell(tempCellId, cellSizes, gridStartingPos), cellSizes, allowedAssetsForThisCell);
                }

            }
        }

        //Then returns that grid ending the function
        return gridToReturn;
    }

    //A function to return the allowed starter assets within a given cell
    public static List<assetData> getAllowedStarterAssets(assetDataList listOfAssets, Vector3Int cellId, Vector3Int gridSize) {
        List<assetData> dataListToReturn = new List<assetData>();
        //Debug line //string allAssets = "Assets At " + cellId + " include ";

        for (int i = 0; i < listOfAssets.listOfAssets.Count; i++) {
            
            //A precaution in order to set the maximum asset limit to the current maxiumum size its allowed, due to grid being modular and unpredictable
            if (listOfAssets.listOfAssets[i].maximumAxisLimit.x == 0 || listOfAssets.listOfAssets[i].maximumAxisLimit.x > gridSize.x) listOfAssets.listOfAssets[i].maximumAxisLimit.x = gridSize.x - listOfAssets.listOfAssets[i].minimumAxisLimit.x;
            if (listOfAssets.listOfAssets[i].maximumAxisLimit.y == 0 || listOfAssets.listOfAssets[i].maximumAxisLimit.y > gridSize.y) listOfAssets.listOfAssets[i].maximumAxisLimit.y = gridSize.y - listOfAssets.listOfAssets[i].minimumAxisLimit.y;
            if (listOfAssets.listOfAssets[i].maximumAxisLimit.z == 0 || listOfAssets.listOfAssets[i].maximumAxisLimit.z > gridSize.z) listOfAssets.listOfAssets[i].maximumAxisLimit.z = gridSize.z - listOfAssets.listOfAssets[i].minimumAxisLimit.z;

            //Detects if the cell is within a location inside the minimum and maximum axis limit of that asset, 
            //If true, it then adds that asset to the list to return.
            if (cellId.x >= listOfAssets.listOfAssets[i].minimumAxisLimit.x && cellId.x < listOfAssets.listOfAssets[i].maximumAxisLimit.x
                && cellId.y >= listOfAssets.listOfAssets[i].minimumAxisLimit.y  && cellId.y < listOfAssets.listOfAssets[i].maximumAxisLimit.y 
                && cellId.z >= listOfAssets.listOfAssets[i].minimumAxisLimit.z  && cellId.z < listOfAssets.listOfAssets[i].maximumAxisLimit.z) {
                    dataListToReturn.Add(listOfAssets.listOfAssets[i]);
                //Debug line //allAssets = allAssets + listOfAssets.listOfAssets[i].name + ", ";
            }
        }

        //Debug line //Debug.Log(allAssets);
        //This resorts the data list to return by the percentage chance of each asset 
        dataListToReturn = dataListToReturn.OrderBy(x => x.percentageChanceOfAsset).ToList();

        return dataListToReturn;


    }

    public static Vector3 getWorldPosOfCell(Vector3Int cellId, Vector3 cellSize, GameObject gridStartingPos) {
        Vector3 posToReturn = new Vector3();

        posToReturn = new Vector3(cellSize.x * cellId.x, cellSize.y * cellId.y, cellSize.z * cellId.z) + gridStartingPos.transform.position;

        return posToReturn;
    }

    public static List<Vector3Int> getAdjacentCellIds(Vector3Int cellId, gridCell[,,] arrayToCheck, axis axisToCheck) {
        List<Vector3Int> cellIdsToReturn = new List<Vector3Int>();

        if (cellId.x != 0 && axisToCheck.HasFlag(axis.xAxis)) cellIdsToReturn.Add(new Vector3Int(cellId.x - 1, cellId.y, cellId.z));
        if (cellId.y != 0 && axisToCheck.HasFlag(axis.yAxis)) cellIdsToReturn.Add(new Vector3Int(cellId.x, cellId.y - 1, cellId.z));
        if (cellId.z != 0 && axisToCheck.HasFlag(axis.zAxis)) cellIdsToReturn.Add(new Vector3Int(cellId.x, cellId.y, cellId.z - 1));

        if (cellId.x != arrayToCheck.GetLength(0) - 1 && axisToCheck.HasFlag(axis.xAxis)) cellIdsToReturn.Add(new Vector3Int(cellId.x + 1, cellId.y, cellId.z));
        if (cellId.y != arrayToCheck.GetLength(1) - 1 && axisToCheck.HasFlag(axis.yAxis)) cellIdsToReturn.Add(new Vector3Int(cellId.x, cellId.y + 1, cellId.z));
        if (cellId.z != arrayToCheck.GetLength(2) - 1 && axisToCheck.HasFlag(axis.zAxis)) cellIdsToReturn.Add(new Vector3Int(cellId.x, cellId.y, cellId.z + 1));


        return cellIdsToReturn;

    }

    public static List<Vector3Int> getEmptyAdjacentCellIds(Vector3Int cellId, gridCell[,,] arrayToCheck, axis axisToCheck) {
        List<Vector3Int> cellIdsToReturn = new List<Vector3Int>();

        if (cellId.x > 0 && axisToCheck.HasFlag(axis.xAxis)) if (arrayToCheck[cellId.x - 1, cellId.y, cellId.z].dataAssigned == null) cellIdsToReturn.Add(new Vector3Int(cellId.x - 1, cellId.y, cellId.z));
        if (cellId.y > 0 && axisToCheck.HasFlag(axis.yAxis)) if (arrayToCheck[cellId.x, cellId.y - 1, cellId.z].dataAssigned == null) cellIdsToReturn.Add(new Vector3Int(cellId.x, cellId.y - 1, cellId.z));
        if (cellId.z > 0 && axisToCheck.HasFlag(axis.zAxis)) if (arrayToCheck[cellId.x, cellId.y, cellId.z - 1].dataAssigned == null) cellIdsToReturn.Add(new Vector3Int(cellId.x, cellId.y, cellId.z - 1));

        if (cellId.x < arrayToCheck.GetLength(0) - 1 && axisToCheck.HasFlag(axis.xAxis)) if (arrayToCheck[cellId.x + 1, cellId.y, cellId.z].dataAssigned == null) cellIdsToReturn.Add(new Vector3Int(cellId.x + 1, cellId.y, cellId.z));
        if (cellId.y < arrayToCheck.GetLength(1) - 1 && axisToCheck.HasFlag(axis.yAxis)) if (arrayToCheck[cellId.x, cellId.y + 1, cellId.z].dataAssigned == null) cellIdsToReturn.Add(new Vector3Int(cellId.x, cellId.y + 1, cellId.z));
        if (cellId.z < arrayToCheck.GetLength(2) - 1 && axisToCheck.HasFlag(axis.zAxis)) if (arrayToCheck[cellId.x, cellId.y, cellId.z + 1].dataAssigned == null) cellIdsToReturn.Add(new Vector3Int(cellId.x, cellId.y, cellId.z + 1));


        return cellIdsToReturn;

    }
}

[Flags]
public enum axis {

    HorizontalAxis = xAxis|zAxis,


    xAxis = 0,
    yAxis = 1,
    zAxis = 2
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

    public gridCell(Vector3Int Id, Vector3 Pos, Vector3 Size, List<assetData> dataList) {
        cellId = Id;
        cellPos = Pos;
        cellSize = Size;
        allowedAsssetsInCell = dataList;
        dataAssigned = null;
        cellObj = null;
    }
}