using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Asset Info", menuName = "asset Data Containers/New Asset Info", order = 0)]
[System.Serializable]

public class assetData : ScriptableObject
{
    public int minVerticalLevel = 0;
    public int maxVerticalLevel = 5;
    public possibleFacingDirections facingDir;
    [Range(.01f,1),]
    public float percentageAllowed = 1;
    public GameObject primaryAsset;
    public List<assetData> allowedAssetsAbove;
    public List<assetData> allowedAssetsBelow;
    public List<assetData> allowedAssetsRight;
    public List<assetData> allowedAssetsLeft;
    public List<assetData> allowedAssetsForward;
    public List<assetData> allowedAssetsBackward;
}

public enum possibleFacingDirections
{
    Forward,
    Right,
    Left,
    Backward
}

