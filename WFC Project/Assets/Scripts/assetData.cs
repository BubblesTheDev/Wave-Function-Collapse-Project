using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Asset Info", menuName = "asset Data Containers/New Asset Info", order = 0)]
public class assetData : ScriptableObject
{
    public string assetName;
    public int assetId;
    public bool isAssetAir;
    public GameObject primaryAsset;
    public GameObject[] allowedAssetsYPos;
    public GameObject[] allowedAssetsYNeg;
    public GameObject[] allowedAssetsXPos;
    public GameObject[] allowedAssetsXNeg;
    public GameObject[] allowedAssetsZPos;
    public GameObject[] allowedAssetsZNeg;
}

