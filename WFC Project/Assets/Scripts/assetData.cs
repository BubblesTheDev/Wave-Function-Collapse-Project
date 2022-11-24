using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Asset Info", menuName = "asset Data Containers/New Asset Info", order = 0)]
[System.Serializable]

public class assetData : ScriptableObject
{
    public string assetName;
    public int assetId;
    public GameObject primaryAsset;
    public assetData[] allowedAssetsAbove;
    public assetData[] allowedAssetsBelow;
    public assetData[] allowedAssetsRight;
    public assetData[] allowedAssetsLeft;
    public assetData[] allowedAssetsForward;
    public assetData[] allowedAssetsBackward;
}

