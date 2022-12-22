using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class assetDataPopulator : MonoBehaviour
{
    //General Variables
    public assetDataList list;
    public int gridWidth;
    public cellBlock[] cellBlocks;

    //UI Variables
    public TextMeshProUGUI assetNameDisplay;
    public GameObject basePopulateButton;
    GameObject[,] assetSidePopulatorButtons;
    public int assetIndex;
    Vector3 cameraOffset;

    private void Awake()
    {
        generateButtonSystem();
        generateGrid();
        spawnAssets();

        cameraOffset = new Vector3(-5f, 3f, -5f);
        Camera.main.transform.position = cellBlocks[assetIndex].primeCell.cellObj.transform.position + cameraOffset;
        Camera.main.transform.LookAt(cellBlocks[assetIndex].primeCell.cellObj.transform);

    }


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.D)) addToIndex(1);
        if (Input.GetKeyUp(KeyCode.A)) addToIndex(-1);
        
    }

    void generateGrid()
    {
        cellBlocks = new cellBlock[list.listOfAssets.Count];
        int tempZPos = 0;
        for (int i = 0; i < list.listOfAssets.Count; i++)
        {
            Vector3 tempPos = (transform.position + ((Vector3.right * 5) * (i % gridWidth)));
            tempPos -= Vector3.right * (gridWidth / 2);
            if (i % gridWidth == 0) tempZPos++;
            tempPos.z -= tempZPos * 5;


            cellBlock tempBlock = new cellBlock(tempPos, list.listOfAssets[i], i, assetSidePopulatorButtons);
            cellBlocks[i] = tempBlock;
        }
    }

    void spawnAssets()
    {
        for (int i = 0; i < cellBlocks.Length; i++)
        {
            switch (list.listOfAssets[i].facingDir)
            {
                case possibleFacingDirections.Right:
                    cellBlocks[i].primeCell.cellObj = Instantiate(cellBlocks[i].primeCell.dataAssigned.primaryAsset, cellBlocks[i].primeCell.cellPos, Quaternion.LookRotation(Vector3.right), GameObject.Find("Asset Holder").transform);
                    if (!cellBlocks[i].primeCell.cellObj.name.Contains(" Right") && !list.listOfAssets[i].name.Contains(" Right")) cellBlocks[i].primeCell.cellObj.name = list.listOfAssets[i].name + " Right";
                    else cellBlocks[i].primeCell.cellObj.name = list.listOfAssets[i].name;
                    break;
                case possibleFacingDirections.Left:
                    cellBlocks[i].primeCell.cellObj = Instantiate(cellBlocks[i].primeCell.dataAssigned.primaryAsset, cellBlocks[i].primeCell.cellPos, Quaternion.LookRotation(-Vector3.right), GameObject.Find("Asset Holder").transform);
                    if (!cellBlocks[i].primeCell.cellObj.name.Contains(" Left") && !list.listOfAssets[i].name.Contains(" Left")) cellBlocks[i].primeCell.cellObj.name = list.listOfAssets[i].name + " Left";
                    else cellBlocks[i].primeCell.cellObj.name = list.listOfAssets[i].name;
                    break;
                case possibleFacingDirections.Forward:
                    cellBlocks[i].primeCell.cellObj = Instantiate(cellBlocks[i].primeCell.dataAssigned.primaryAsset, cellBlocks[i].primeCell.cellPos, Quaternion.LookRotation(Vector3.forward), GameObject.Find("Asset Holder").transform);
                    if (!cellBlocks[i].primeCell.cellObj.name.Contains(" Forward") && !list.listOfAssets[i].name.Contains(" Forward")) cellBlocks[i].primeCell.cellObj.name = list.listOfAssets[i].name + " Forward";
                    else cellBlocks[i].primeCell.cellObj.name = list.listOfAssets[i].name;
                    break;
                case possibleFacingDirections.Backward:
                    cellBlocks[i].primeCell.cellObj = Instantiate(cellBlocks[i].primeCell.dataAssigned.primaryAsset, cellBlocks[i].primeCell.cellPos, Quaternion.LookRotation(-Vector3.forward), GameObject.Find("Asset Holder").transform);
                    if (!cellBlocks[i].primeCell.cellObj.name.Contains(" Backward") && !list.listOfAssets[i].name.Contains(" Backward")) cellBlocks[i].primeCell.cellObj.name = list.listOfAssets[i].name + " Backward";
                    else cellBlocks[i].primeCell.cellObj.name = list.listOfAssets[i].name;
                    break;
            }
        }
    }

    void generateButtonSystem()
    {
        assetSidePopulatorButtons = new GameObject[6, list.listOfAssets.Count];

        for (int i = 0; i < assetSidePopulatorButtons.GetLength(0); i++)
        {
            int maxSizeX = 0;
            int maxWidth = 0;

            for (int j = 0; j < assetSidePopulatorButtons.GetLength(1); j++)
            {
                if (j % 3 == 0 && j != 0) maxSizeX++;
                if (j % 3 == 0) maxWidth = 0;
                else maxWidth++;
                GameObject tempObj = null;
                switch (i + 1)
                {
                    case 1:
                        if (GameObject.Find("Top Side Button Holder")) tempObj = GameObject.Find("Top Side Button Holder");
                        else tempObj = new GameObject("Top Side Button Holder");
                        break;

                    case 2:
                        if (GameObject.Find("Bot Side Button Holder")) tempObj = GameObject.Find("Bot Side Button Holder");
                        else tempObj = new GameObject("Bot Side Button Holder");
                        break;
                    case 3:
                        if (GameObject.Find("Right Side Button Holder")) tempObj = GameObject.Find("Right Side Button Holder");
                        else tempObj = new GameObject("Right Side Button Holder");
                        break;
                    case 4:
                        if (GameObject.Find("Left Side Button Holder")) tempObj = GameObject.Find("Left Side Button Holder");
                        else tempObj = new GameObject("Left Side Button Holder");
                        break;
                    case 5:
                        if (GameObject.Find("Front Side Button Holder")) tempObj = GameObject.Find("Front Side Button Holder");
                        else tempObj = new GameObject("Front Side Button Holder");
                        break;
                    case 6:
                        if (GameObject.Find("Back Side Button Holder")) tempObj = GameObject.Find("Back Side Button Holder");
                        else tempObj = new GameObject("Back Side Button Holder");

                        break;
                }


                assetSidePopulatorButtons[i, j] = Instantiate(basePopulateButton, basePopulateButton.transform.position, Quaternion.identity, tempObj.transform);
                assetSidePopulatorButtons[i, j].GetComponent<Image>().sprite = list.listOfAssets[j].assetLogo;
                assetSidePopulatorButtons[i, j].GetComponentInChildren<Text>().text = list.listOfAssets[j].name;
                assetSidePopulatorButtons[i, j].name = list.listOfAssets[j].name;
            }
        }

        basePopulateButton.SetActive(false);
        GameObject.Find("Top Buttons").transform.parent.gameObject.SetActive(false);
    }

    void updateUI()
    {
        if (cellBlocks.Length > 0)
        {
            assetNameDisplay.text = "Asset Name: " + cellBlocks[assetIndex].primeCell.dataAssigned.ToString().Split("(assetData)").FirstOrDefault();

        }
    }

    public void addToIndex(int numberToAdd)
    {
        updateToggles();
        foreach (GameObject item in assetSidePopulatorButtons)
        {
            foreach (Transform child in item.transform)
            {
                if (child.name.Contains("(Clone)")) Destroy(child.gameObject);
            }
        }

        assetIndex += numberToAdd;

        

        if (assetIndex > list.listOfAssets.Count - 1) assetIndex = 0;
        if (assetIndex < 0) assetIndex = list.listOfAssets.Count - 1;
        updateUI();
        Camera.main.transform.position = cellBlocks[assetIndex].primeCell.cellObj.transform.position + cameraOffset;
        Camera.main.transform.LookAt(cellBlocks[assetIndex].primeCell.cellObj.transform);

        

        
    }

    void updateToggles()
    {
        foreach (GameObject item in assetSidePopulatorButtons)
        {
            if ((item.name + " Right") == (list.listOfAssets[assetIndex].name)) print(item + " Right" + "    " + list.listOfAssets[assetIndex].name);
            if ((item.name + " Left") == (list.listOfAssets[assetIndex].name)) print(item + " Left" + "    " + list.listOfAssets[assetIndex].name);
            if ((item.name + " Forward") == (list.listOfAssets[assetIndex].name)) print(item + " Forward" + "    " + list.listOfAssets[assetIndex].name);
            if ((item.name + " Backward") == (list.listOfAssets[assetIndex].name)) print(item + " Backward" + "    " + list.listOfAssets[assetIndex].name);
        }
            
    }

    public void populateAssetSide()
    {
        GameObject thingCalling = EventSystem.current.currentSelectedGameObject;


        /*if (thingCalling.GetComponent<Toggle>().isOn) thingCalling.GetComponent<Image>().color = Color.green;
        else thingCalling.GetComponent<Image>().color = Color.red;
        if (thingCalling.transform.parent.gameObject.name.Contains("Top"))
        {
            if(!list.listOfAssets[assetIndex].allowedAssetsAbove.Contains(list.listOfAssets.Where(assetData => assetData.name == thingCalling.name).SingleOrDefault())) 
            {
                list.listOfAssets[assetIndex].allowedAssetsAbove.Add(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault());
                cellBlocks[assetIndex].cellAbove.dataAssigned = list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault();
                switch (list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().facingDir)
                {
                    case possibleFacingDirections.Right:
                        cellBlocks[assetIndex].cellAbove.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset, 
                            cellBlocks[assetIndex].cellAbove.cellPos, Quaternion.LookRotation(Vector3.right), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                    case possibleFacingDirections.Left:
                        cellBlocks[assetIndex].cellAbove.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset, 
                            cellBlocks[assetIndex].cellAbove.cellPos, Quaternion.LookRotation(-Vector3.right), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                    case possibleFacingDirections.Forward:
                        cellBlocks[assetIndex].cellAbove.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset, 
                            cellBlocks[assetIndex].cellAbove.cellPos, Quaternion.LookRotation(Vector3.forward), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                    case possibleFacingDirections.Backward:
                        cellBlocks[assetIndex].cellAbove.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset, 
                            cellBlocks[assetIndex].cellAbove.cellPos, Quaternion.LookRotation(-Vector3.forward), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                }
            }
            else if(list.listOfAssets[assetIndex].allowedAssetsAbove.Contains(list.listOfAssets.Where(assetData => assetData.name == thingCalling.name).SingleOrDefault()))
            {
                list.listOfAssets[assetIndex].allowedAssetsAbove.Remove(list.listOfAssets.Where(assetData => assetData.name == thingCalling.name).SingleOrDefault());
                foreach (Transform child in cellBlocks[assetIndex].primeCell.cellObj.transform)
                {
                    if (child.name == list.listOfAssets.Where(assetData => assetData.name == thingCalling.name).SingleOrDefault().primaryAsset.name + "(Clone)") Destroy(child.gameObject);
                }
            }
        }
        if (thingCalling.transform.parent.gameObject.name.Contains("Bot"))
        {
            if (!list.listOfAssets[assetIndex].allowedAssetsBelow.Contains(list.listOfAssets.Where(assetData => assetData.name == thingCalling.name).SingleOrDefault()))
            {
                list.listOfAssets[assetIndex].allowedAssetsBelow.Add(list.listOfAssets.Where(assetData => assetData.name == thingCalling.name).SingleOrDefault());
                cellBlocks[assetIndex].cellBelow.dataAssigned = list.listOfAssets.Where(assetData => assetData.name == thingCalling.name).SingleOrDefault();

                switch(list.listOfAssets.Where(assetData => assetData.name == thingCalling.name).SingleOrDefault().facingDir)
                {
                    case possibleFacingDirections.Right: 
                cellBlocks[assetIndex].cellBelow.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling.name).SingleOrDefault().primaryAsset, 
                    cellBlocks[assetIndex].cellBelow.cellPos, Quaternion.LookRotation(Vector3.right), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                    case possibleFacingDirections.Left: 
                cellBlocks[assetIndex].cellBelow.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling.name).SingleOrDefault().primaryAsset, 
                    cellBlocks[assetIndex].cellBelow.cellPos, Quaternion.LookRotation(-Vector3.right), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                    case possibleFacingDirections.Forward: 
                cellBlocks[assetIndex].cellBelow.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling.name).SingleOrDefault().primaryAsset, 
                    cellBlocks[assetIndex].cellBelow.cellPos, Quaternion.LookRotation(Vector3.forward), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                    case possibleFacingDirections.Backward: 
                cellBlocks[assetIndex].cellBelow.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling.name).SingleOrDefault().primaryAsset, 
                    cellBlocks[assetIndex].cellBelow.cellPos, Quaternion.LookRotation(-Vector3.forward), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                }

            }
            else if (list.listOfAssets[assetIndex].allowedAssetsBelow.Contains(list.listOfAssets.Where(assetData => assetData.name == thingCalling.name).SingleOrDefault()))
            {
                list.listOfAssets[assetIndex].allowedAssetsBelow.Remove(list.listOfAssets.Where(assetData => assetData.name == thingCalling.name).SingleOrDefault());
                foreach (Transform child in cellBlocks[assetIndex].primeCell.cellObj.transform)
                {
                    if (child.name == list.listOfAssets.Where(assetData => assetData.name == thingCalling.name).SingleOrDefault().primaryAsset.name + "(Clone)") Destroy(child.gameObject);
                }
            }
        }
        if (thingCalling.transform.parent.gameObject.name.Contains("Right"))
        {
            if (!list.listOfAssets[assetIndex].allowedAssetsRight.Contains(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault()))
            {
                list.listOfAssets[assetIndex].allowedAssetsRight.Add(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault());
                cellBlocks[assetIndex].cellRight.dataAssigned = list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault();
                switch (list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().facingDir)
                {
                    case possibleFacingDirections.Right:
                        cellBlocks[assetIndex].cellRight.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset,
                            cellBlocks[assetIndex].cellRight.cellPos, Quaternion.LookRotation(Vector3.right), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                    case possibleFacingDirections.Left:
                        cellBlocks[assetIndex].cellRight.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset,
                            cellBlocks[assetIndex].cellRight.cellPos, Quaternion.LookRotation(-Vector3.right), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                    case possibleFacingDirections.Forward:
                        cellBlocks[assetIndex].cellRight.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset,
                            cellBlocks[assetIndex].cellRight.cellPos, Quaternion.LookRotation(Vector3.forward), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                    case possibleFacingDirections.Backward:
                        cellBlocks[assetIndex].cellRight.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset,
                            cellBlocks[assetIndex].cellRight.cellPos, Quaternion.LookRotation(-Vector3.forward), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                }
            }
            else if (list.listOfAssets[assetIndex].allowedAssetsRight.Contains(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault()))
            {
                list.listOfAssets[assetIndex].allowedAssetsRight.Remove(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault());
                foreach (Transform child in cellBlocks[assetIndex].primeCell.cellObj.transform)
                {
                    if (child.name == list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset.name + "(Clone)") Destroy(child.gameObject);
                }
            }
        }
        if (thingCalling.transform.parent.gameObject.name.Contains("Left"))
        {
            if (!list.listOfAssets[assetIndex].allowedAssetsLeft.Contains(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault()))
            {
                list.listOfAssets[assetIndex].allowedAssetsLeft.Add(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault());
                cellBlocks[assetIndex].cellLeft.dataAssigned = list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault();
                switch (list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().facingDir)
                {
                    case possibleFacingDirections.Right:
                        cellBlocks[assetIndex].cellLeft.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset,
                            cellBlocks[assetIndex].cellLeft.cellPos, Quaternion.LookRotation(Vector3.right), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                    case possibleFacingDirections.Left:
                        cellBlocks[assetIndex].cellLeft.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset,
                            cellBlocks[assetIndex].cellLeft.cellPos, Quaternion.LookRotation(-Vector3.right), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                    case possibleFacingDirections.Forward:
                        cellBlocks[assetIndex].cellLeft.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset,
                            cellBlocks[assetIndex].cellLeft.cellPos, Quaternion.LookRotation(Vector3.forward), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                    case possibleFacingDirections.Backward:
                        cellBlocks[assetIndex].cellLeft.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset,
                            cellBlocks[assetIndex].cellLeft.cellPos, Quaternion.LookRotation(-Vector3.forward), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                }
            }
            else if (list.listOfAssets[assetIndex].allowedAssetsLeft.Contains(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault()))
            {
                list.listOfAssets[assetIndex].allowedAssetsLeft.Remove(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault());
                foreach (Transform child in cellBlocks[assetIndex].primeCell.cellObj.transform)
                {
                    if (child.name == list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset.name + "(Clone)") Destroy(child.gameObject);
                }
            }
        }
        if (thingCalling.transform.parent.gameObject.name.Contains("Front"))
        {
            if (!list.listOfAssets[assetIndex].allowedAssetsForward.Contains(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault()))
            {
                list.listOfAssets[assetIndex].allowedAssetsForward.Add(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault());
                cellBlocks[assetIndex].cellForward.dataAssigned = list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault();
                switch (list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().facingDir)
                {
                    case possibleFacingDirections.Right:
                        cellBlocks[assetIndex].cellForward.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset,
                            cellBlocks[assetIndex].cellForward.cellPos, Quaternion.LookRotation(Vector3.right), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                    case possibleFacingDirections.Left:
                        cellBlocks[assetIndex].cellForward.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset,
                            cellBlocks[assetIndex].cellForward.cellPos, Quaternion.LookRotation(-Vector3.right), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                    case possibleFacingDirections.Forward:
                        cellBlocks[assetIndex].cellForward.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset,
                            cellBlocks[assetIndex].cellForward.cellPos, Quaternion.LookRotation(Vector3.forward), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                    case possibleFacingDirections.Backward:
                        cellBlocks[assetIndex].cellForward.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset,
                            cellBlocks[assetIndex].cellForward.cellPos, Quaternion.LookRotation(-Vector3.forward), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                }
            }
            else if (list.listOfAssets[assetIndex].allowedAssetsForward.Contains(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault()))
            {
                list.listOfAssets[assetIndex].allowedAssetsForward.Remove(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault());
                foreach (Transform child in cellBlocks[assetIndex].primeCell.cellObj.transform)
                {
                    if (child.name == list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset.name + "(Clone)") Destroy(child.gameObject);
                }
            }
        }
        if (thingCalling.transform.parent.gameObject.name.Contains("Back"))
        {
            if (!list.listOfAssets[assetIndex].allowedAssetsBackward.Contains(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault()))
            {
                list.listOfAssets[assetIndex].allowedAssetsBackward.Add(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault());
                cellBlocks[assetIndex].cellBackward.dataAssigned = list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault();
                switch (list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().facingDir)
                {
                    case possibleFacingDirections.Right:
                        cellBlocks[assetIndex].cellBackward.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset,
                            cellBlocks[assetIndex].cellBackward.cellPos, Quaternion.LookRotation(Vector3.right), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                    case possibleFacingDirections.Left:
                        cellBlocks[assetIndex].cellBackward.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset,
                            cellBlocks[assetIndex].cellBackward.cellPos, Quaternion.LookRotation(-Vector3.right), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                    case possibleFacingDirections.Forward:
                        cellBlocks[assetIndex].cellBackward.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset,
                            cellBlocks[assetIndex].cellBackward.cellPos, Quaternion.LookRotation(Vector3.forward), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                    case possibleFacingDirections.Backward:
                        cellBlocks[assetIndex].cellBackward.cellObj = Instantiate(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset,
                            cellBlocks[assetIndex].cellBackward.cellPos, Quaternion.LookRotation(-Vector3.forward), cellBlocks[assetIndex].primeCell.cellObj.transform);
                        break;
                }
            }
            else if (list.listOfAssets[assetIndex].allowedAssetsBackward.Contains(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault()))
            {
                list.listOfAssets[assetIndex].allowedAssetsBackward.Remove(list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault());
                foreach (Transform child in cellBlocks[assetIndex].primeCell.cellObj.transform)
                {
                    if (child.name == list.listOfAssets.Where(assetData => assetData.name == thingCalling).SingleOrDefault().primaryAsset.name + "(Clone)") Destroy(child.gameObject);
                }
            }
        }*/
    }

    public void clearAllAssetData()
    {
        foreach (assetData item in list.listOfAssets)
        {
            item.allowedAssetsAbove.Clear();
            item.allowedAssetsBelow.Clear();
            item.allowedAssetsRight.Clear();
            item.allowedAssetsLeft.Clear();
            item.allowedAssetsForward.Clear();
            item.allowedAssetsBackward.Clear();
        }
    }

    public void fixDiscrepencies()
    {
        for (int i = 0; i < list.listOfAssets.Count-1; i++)
        {
            for (int x = 0; x < list.listOfAssets[i].allowedAssetsAbove.Count-1; x++)
            {
                list.listOfAssets[i].allowedAssetsAbove = list.listOfAssets[i].allowedAssetsAbove.Union(list.listOfAssets[i].allowedAssetsAbove[x].allowedAssetsBelow).ToList();
            }
            for (int x = 0; x < list.listOfAssets[i].allowedAssetsBelow.Count - 1; x++)
            {
                list.listOfAssets[i].allowedAssetsBelow = list.listOfAssets[i].allowedAssetsBelow.Union(list.listOfAssets[i].allowedAssetsBelow[x].allowedAssetsAbove).ToList();
            }
            for (int x = 0; x < list.listOfAssets[i].allowedAssetsRight.Count - 1; x++)
            {
                list.listOfAssets[i].allowedAssetsRight = list.listOfAssets[i].allowedAssetsRight.Union(list.listOfAssets[i].allowedAssetsRight[x].allowedAssetsLeft).ToList();
            }
            for (int x = 0; x < list.listOfAssets[i].allowedAssetsLeft.Count - 1; x++)
            {
                list.listOfAssets[i].allowedAssetsLeft = list.listOfAssets[i].allowedAssetsLeft.Union(list.listOfAssets[i].allowedAssetsLeft[x].allowedAssetsRight).ToList();
            }
            for (int x = 0; x < list.listOfAssets[i].allowedAssetsForward.Count - 1; x++)
            {
                list.listOfAssets[i].allowedAssetsForward = list.listOfAssets[i].allowedAssetsForward.Union(list.listOfAssets[i].allowedAssetsForward[x].allowedAssetsBackward).ToList();
            }
            for (int x = 0; x < list.listOfAssets[i].allowedAssetsBackward.Count - 1; x++)
            {
                list.listOfAssets[i].allowedAssetsBackward = list.listOfAssets[i].allowedAssetsBackward.Union(list.listOfAssets[i].allowedAssetsBackward[x].allowedAssetsForward).ToList();
            }
        }
    }
}

[System.Serializable]
public struct populatorCell
{
    public int cellId;
    public Vector3 cellPos;
    public assetData dataAssigned;
    public GameObject cellObj;
    public populatorCell(int Id, Vector3 Pos, assetData dataToAssign)
    {
        cellId = Id;
        cellPos = Pos;
        dataAssigned = dataToAssign;
        cellObj = null;
    }
}

[System.Serializable]
public struct cellBlock
{
    public populatorCell primeCell, cellAbove, cellBelow, cellRight, cellLeft, cellForward, cellBackward;
    public int cellBlockId;
    public GameObject[] topSidePopulateButtons;
    public GameObject[] botSidePopulateButtons;
    public GameObject[] rightSidePopulateButtons;
    public GameObject[] leftSidePopulateButtons;
    public GameObject[] frontSidePopulateButtons;
    public GameObject[] backSidePopulateButtons;

    public cellBlock(Vector3 primeCellPos, assetData primeCellAsset, int Id, GameObject[,] assetButtons)
    {
        primeCell = new populatorCell((Id * 10), primeCellPos, primeCellAsset);
        cellAbove = new populatorCell((Id * 10) + 1, primeCellPos + Vector3.up, primeCellAsset);
        cellBelow = new populatorCell((Id * 10) + 2, primeCellPos - Vector3.up, primeCellAsset);
        cellRight = new populatorCell((Id * 10) + 3, primeCellPos + Vector3.right, primeCellAsset);
        cellLeft = new populatorCell((Id * 10) + 4, primeCellPos - Vector3.right, primeCellAsset);
        cellForward = new populatorCell((Id * 10) + 5, primeCellPos + Vector3.forward, primeCellAsset);
        cellBackward = new populatorCell((Id * 10) + 6, primeCellPos - Vector3.forward, primeCellAsset);
        cellBlockId = Id;

        topSidePopulateButtons = new GameObject[assetButtons.GetLength(1)];

        botSidePopulateButtons = new GameObject[assetButtons.GetLength(1)];
        rightSidePopulateButtons = new GameObject[assetButtons.GetLength(1)];
        leftSidePopulateButtons = new GameObject[assetButtons.GetLength(1)];
        frontSidePopulateButtons = new GameObject[assetButtons.GetLength(1)];
        backSidePopulateButtons = new GameObject[assetButtons.GetLength(1)];
        for (int i = 0; i < assetButtons.GetLength(0); i++)
        {
            switch (i + 1)
            {
                case 1:

                    for (int j = 0; j < assetButtons.GetLength(1); j++)
                    {
                        topSidePopulateButtons[j] = assetButtons[i, j];
                    }
                    break;
                case 2:
                    for (int j = 0; j < assetButtons.GetLength(1); j++)
                    {
                        botSidePopulateButtons[j] = assetButtons[i, j];
                    }
                    break;
                case 3:
                    for (int j = 0; j < assetButtons.GetLength(1); j++)
                    {
                        rightSidePopulateButtons[j] = assetButtons[i, j];
                    }
                    break;
                case 4:
                    for (int j = 0; j < assetButtons.GetLength(1); j++)
                    {
                        leftSidePopulateButtons[j] = assetButtons[i, j];
                    }
                    break;
                case 5:
                    for (int j = 0; j < assetButtons.GetLength(1); j++)
                    {
                        frontSidePopulateButtons[j] = assetButtons[i, j];
                    }
                    break;
                case 6:
                    for (int j = 0; j < assetButtons.GetLength(1); j++)
                    {
                        backSidePopulateButtons[j] = assetButtons[i, j];
                    }
                    break;
            }

        }
    }
}


