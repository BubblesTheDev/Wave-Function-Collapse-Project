using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Asset List", menuName = "asset Data Containers/Create New Asset List", order = 1)]
public class assetDataList : ScriptableObject
{
    public List<assetData> listOfAssets = new List<assetData>();
}
