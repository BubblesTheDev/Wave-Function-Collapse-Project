using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WFCGenerator : MonoBehaviour
{
    [Space , Header("Generator Settings")]
    public GameObject cellStartingPos;
    public int gridSizeX, gridSizeZ;
    public float cellSizeX, cellSizeZ;


    private gridCell[,] gridArray;


    private void Awake()
    {
        regenerateGrid();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space)) printGridInfo();
    }

    //Overall script to generate grid 
    void regenerateGrid()
    {
        //Remake Array WIth Desired Size
        gridArray = new gridCell[gridSizeX, gridSizeZ];

        //iterates through the entier grid and initializes the struct at that spesific ID, with all the required information
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                //temp variables to do basic calculation
                Vector2Int tempId = new Vector2Int(x, z);
                Vector3 tempSize = new Vector2(cellSizeX, cellSizeZ);

                //initializing the cell at a spesific grid ID
                gridArray[x, z] = new gridCell(tempId, getGridPos(x,z), tempSize);
            }
        }
    }

    void printGridInfo()
    {
        //iterates through the entier grid 
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                //logs each grid cell, at their position, with their id, and how big they are
                Debug.Log("Cell at: " + gridArray[x, z].cellPos + ", Has an ID of: " + gridArray[x, z].cellId + ", and a size of: " + gridArray[x, z].cellSize);
            }
        }
    }

    private void OnDrawGizmos()
    {
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int z = 0; z < gridArray.GetLength(1); z++)
            {
                
            }
        }
    }

    //gets the grid position of the cell in real world space
    public Vector3 getGridPos(int xId, int zId)
    {
        float tempPosX;
        float tempPosZ;

        //if is is the first cell, just set position equal to the starting position game object
        if (xId == 0) tempPosX = cellStartingPos.transform.position.x;
        else
        {
            tempPosX = cellStartingPos.transform.position.x + (xId * (cellSizeX / 2));
        }
        if (zId == 0) tempPosZ = cellStartingPos.transform.position.z;
        else
        {
            tempPosZ = cellStartingPos.transform.position.z + (zId * (cellSizeX / 2));
        }


        return new Vector3(tempPosX, 0, tempPosZ);
    }

}



//Cell struct with basic information required
public struct gridCell
{
    public Vector2Int cellId;
    public Vector3 cellPos;
    public Vector2 cellSize;

    public gridCell(Vector2Int Id, Vector3 Pos, Vector2 Size)
    {
        cellId = Id;
        cellPos = Pos;
        cellSize = Size;
    }
}