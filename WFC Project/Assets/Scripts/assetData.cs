using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Asset Info", menuName = "asset Data Containers/New Asset Info", order = 0)]
[System.Serializable]

public class assetData : ScriptableObject
{
    public Sprite assetLogo;
    [Space]
    public possibleFacingDirections facingDir;
    public Vector3Int minimumAxisLimit;
    public Vector3Int maximumAxisLimit;
    public float percentageChanceOfAsset;
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

