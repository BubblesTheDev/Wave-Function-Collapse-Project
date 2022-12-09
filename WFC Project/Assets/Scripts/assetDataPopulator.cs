using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class assetDataPopulator : MonoBehaviour
{
    public assetDataList list;
    public int currentAssetListIndex = 0;
    public bool clearBefore = false;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.F3))
        {
            if(clearBefore)
            foreach (assetData item in list.listOfAssets)
            {
                item.allowedAssetsAbove.Clear();
                item.allowedAssetsBelow.Clear();
                item.allowedAssetsRight.Clear();
                item.allowedAssetsLeft.Clear();
                item.allowedAssetsForward.Clear();
                item.allowedAssetsBackward.Clear();
            }
            for (currentAssetListIndex = 0; currentAssetListIndex < list.listOfAssets.Count; currentAssetListIndex++)
            {
                populateAsset(list.listOfAssets[currentAssetListIndex]);
            }
        }
    }
    void populateAsset(assetData dataToGive)
    {

        //Sets all the assets to have propper names
        {
            if (list.listOfAssets[currentAssetListIndex].name.Contains("1") && !list.listOfAssets[currentAssetListIndex].name.Contains("(Above)"))
            {
                string assetPath = AssetDatabase.GetAssetPath(list.listOfAssets[currentAssetListIndex]);
                AssetDatabase.RenameAsset(assetPath, list.listOfAssets[currentAssetListIndex].name + " (Above)");

            }
            if (list.listOfAssets[currentAssetListIndex].name.Contains("2") && !list.listOfAssets[currentAssetListIndex].name.Contains("(Below)"))
            {
                string assetPath = AssetDatabase.GetAssetPath(list.listOfAssets[currentAssetListIndex]);
                AssetDatabase.RenameAsset(assetPath, list.listOfAssets[currentAssetListIndex].name + " (Below)");
            }
            if (list.listOfAssets[currentAssetListIndex].name.Contains("3") && !list.listOfAssets[currentAssetListIndex].name.Contains("(Right)"))
            {
                string assetPath = AssetDatabase.GetAssetPath(list.listOfAssets[currentAssetListIndex]);
                AssetDatabase.RenameAsset(assetPath, list.listOfAssets[currentAssetListIndex].name + " (Right)");
            }
            if (list.listOfAssets[currentAssetListIndex].name.Contains("4") && !list.listOfAssets[currentAssetListIndex].name.Contains("(Left)"))
            {
                string assetPath = AssetDatabase.GetAssetPath(list.listOfAssets[currentAssetListIndex]);
                AssetDatabase.RenameAsset(assetPath, list.listOfAssets[currentAssetListIndex].name + " (Left)");
            }
            if (list.listOfAssets[currentAssetListIndex].name.Contains("5") && !list.listOfAssets[currentAssetListIndex].name.Contains("(Forward)"))
            {
                string assetPath = AssetDatabase.GetAssetPath(list.listOfAssets[currentAssetListIndex]);
                AssetDatabase.RenameAsset(assetPath, list.listOfAssets[currentAssetListIndex].name + " (Forward)");
            }
            if (list.listOfAssets[currentAssetListIndex].name.Contains("6") && !list.listOfAssets[currentAssetListIndex].name.Contains("(Backward)"))
            {
                string assetPath = AssetDatabase.GetAssetPath(list.listOfAssets[currentAssetListIndex]);
                AssetDatabase.RenameAsset(assetPath, list.listOfAssets[currentAssetListIndex].name + " (Backward)");
            }

        }





        //Runs through all assets within the asset list
        foreach (assetData dataInList in list.listOfAssets)
        {
            //Populates the dataToGive dataAsset with all the inverse assets
            {
                if (dataToGive.fillAboveSlot)
                    if (!dataToGive.name.Contains("1") && !dataInList.name.Contains("2") && !dataToGive.allowedAssetsAbove.Contains(dataInList))
                    {
                        //print("Adding The Asset of " + dataInList.name + " To the allowed assets below of " + dataToGive.name);
                        dataToGive.allowedAssetsAbove.Add(dataInList);
                    }
                if (dataToGive.fillBelowSlot)
                    if (!dataToGive.name.Contains("2") && !dataInList.name.Contains("1") && !dataToGive.allowedAssetsBelow.Contains(dataInList))
                    {
                        //print("Adding The Asset of " + dataInList.name + " To the allowed assets below of " + dataToGive.name);
                        dataToGive.allowedAssetsBelow.Add(dataInList);
                    }
                if (dataToGive.fillRightSlot)
                    if (!dataToGive.name.Contains("3") && !dataInList.name.Contains("4") && !dataToGive.allowedAssetsRight.Contains(dataInList))
                    {
                        //print("Adding The Asset of " + dataInList.name + " To the allowed assets below of " + dataToGive.name);
                        dataToGive.allowedAssetsRight.Add(dataInList);
                    }
                if (dataToGive.fillLeftSlot)
                    if (!dataToGive.name.Contains("4") && !dataInList.name.Contains("3") && !dataToGive.allowedAssetsLeft.Contains(dataInList))
                    {
                        //print("Adding The Asset of " + dataInList.name + " To the allowed assets below of " + dataToGive.name);
                        dataToGive.allowedAssetsLeft.Add(dataInList);
                    }
                if (dataToGive.fillForwardSlot)
                    if (!dataToGive.name.Contains("5") && !dataInList.name.Contains("6") && !dataToGive.allowedAssetsForward.Contains(dataInList))
                    {
                        //print("Adding The Asset of " + dataInList.name + " To the allowed assets below of " + dataToGive.name);
                        dataToGive.allowedAssetsForward.Add(dataInList);
                    }
                if (dataToGive.fillBackwardSlot)
                    if (!dataToGive.name.Contains("6") && !dataInList.name.Contains("5") && !dataToGive.allowedAssetsBackward.Contains(dataInList))
                    {
                        //print("Adding The Asset of " + dataInList.name + " To the allowed assets below of " + dataToGive.name);
                        dataToGive.allowedAssetsBackward.Add(dataInList);
                    }
            }

            //Populates the dataToGive dataAsset with all allowed assets
            {
                if (dataToGive.fillAboveSlot)
                    if (dataToGive.name.Contains("1") && dataInList.name.Contains("2") && !dataToGive.allowedAssetsAbove.Contains(dataInList))
                    {
                        //print("Adding The Asset of " + dataInList.name + " To the allowed assets below of " + dataToGive.name);
                        dataToGive.allowedAssetsAbove.Add(dataInList);
                    }
                if (dataToGive.fillBelowSlot)
                    if (dataToGive.name.Contains("2") && dataInList.name.Contains("1") && !dataToGive.allowedAssetsBelow.Contains(dataInList))
                    {
                        //print("Adding The Asset of " + dataInList.name + " To the allowed assets above of " + dataToGive.name);
                        dataToGive.allowedAssetsBelow.Add(dataInList);
                    }
                if (dataToGive.fillRightSlot)
                    if (dataToGive.name.Contains("3") && dataInList.name.Contains("4") && !dataToGive.allowedAssetsRight.Contains(dataInList))
                    {
                        //print("Adding The Asset of " + dataInList.name + " To the allowed assets left of " + dataToGive.name);
                        dataToGive.allowedAssetsRight.Add(dataInList);
                    }
                if (dataToGive.fillLeftSlot)
                    if (dataToGive.name.Contains("4") && dataInList.name.Contains("3") && !dataToGive.allowedAssetsLeft.Contains(dataInList))
                    {
                        //print("Adding The Asset of " + dataInList.name + " To the allowed assets right of " + dataToGive.name);
                        dataToGive.allowedAssetsLeft.Add(dataInList);
                    }
                if (dataToGive.fillForwardSlot)
                    if (dataToGive.name.Contains("5") && dataInList.name.Contains("6") && !dataToGive.allowedAssetsForward.Contains(dataInList))
                    {
                        //print("Adding The Asset of " + dataInList.name + " To the allowed assets backward of " + dataToGive.name);
                        dataToGive.allowedAssetsForward.Add(dataInList);
                    }
                if (dataToGive.fillBackwardSlot)
                    if (dataToGive.name.Contains("6") && dataInList.name.Contains("5") && !dataToGive.allowedAssetsBackward.Contains(dataInList))
                    {
                        //print("Adding The Asset of " + dataInList.name + " To the allowed assets forward of " + dataToGive.name);
                        dataToGive.allowedAssetsBackward.Add(dataInList);
                    }
            }

            //Populates the dataToGive dataAssets empty slots with the void asset if needed
            {
                if (dataToGive.fillEmptyWithVoid && dataToGive.allowedAssetsAbove.Count == 0)
                {
                    dataToGive.allowedAssetsAbove.Add(list.listOfAssets[0]);
                }
                if (dataToGive.fillEmptyWithVoid && dataToGive.allowedAssetsBelow.Count == 0)
                {
                    dataToGive.allowedAssetsBelow.Add(list.listOfAssets[0]);
                }
                if (dataToGive.fillEmptyWithVoid && dataToGive.allowedAssetsRight.Count == 0)
                {
                    dataToGive.allowedAssetsRight.Add(list.listOfAssets[0]);
                }
                if (dataToGive.fillEmptyWithVoid && dataToGive.allowedAssetsLeft.Count == 0)
                {
                    dataToGive.allowedAssetsLeft.Add(list.listOfAssets[0]);
                }
                if (dataToGive.fillEmptyWithVoid && dataToGive.allowedAssetsForward.Count == 0)
                {
                    dataToGive.allowedAssetsForward.Add(list.listOfAssets[0]);
                }
                if (dataToGive.fillEmptyWithVoid && dataToGive.allowedAssetsBackward.Count == 0)
                {
                    dataToGive.allowedAssetsBackward.Add(list.listOfAssets[0]);
                }
            }
        }


        print("Finished Populating assets");
    }
    

}
