using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Asset List", menuName = "asset Data Containers/Create Asset Name", order = 1)]
public class assetDataList : ScriptableObject
{
    List<assetData> listOfAssets = new List<assetData>();
}
