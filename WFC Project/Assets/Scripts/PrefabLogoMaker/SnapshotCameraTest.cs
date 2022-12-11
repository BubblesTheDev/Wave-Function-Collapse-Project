using UnityEngine;
using System.Collections;

public class SnapshotCameraTest : MonoBehaviour {
    [HideInInspector]
    public GameObject objectToSnapshot;
    [HideInInspector]
    public Color backgroundColor = Color.clear;
    [HideInInspector]
    public Vector3 position = new Vector3(0, 0, 1);
    [HideInInspector]
    public Vector3 rotation = new Vector3(345.8529f, 313.8297f, 14.28433f);
    [HideInInspector]
    public Vector3 scale = new Vector3(1, 1, 1);
    [HideInInspector]
    public assetDataPopulator populator;
    public int assetIndex;

    private SnapshotCamera snapshotCamera;
    private Texture2D texture;
    public bool camActive = false;

    void OnGUI()
    {
        if (camActive)
        {
            //GUI.TextField(new Rect(10, 5, 275, 21), "Press \"Spacebar\" to save the snapshot");

            if (texture != null)
            {
                GUI.backgroundColor = Color.clear;
                GUI.Box(new Rect(300, 300, texture.width, texture.height), texture);
            }
        }
        
    }

    public void UpdatePreview ()
    {
        if (objectToSnapshot != null)
        {
            // Destroy the texture to prevent a memory leak
            // For a bit of fun you can try removing this and watching the memory profiler while for example continuously changing the rotation to trigger UpdatePreview()
            Object.Destroy(texture);

            // Take a new snapshot of the objectToSnapshot
            texture = snapshotCamera.TakeObjectSnapshot(objectToSnapshot, backgroundColor, position, Quaternion.Euler(rotation), scale, width: 512, height: 512);
        }
    }

	void Update ()
    {

        if (camActive)
        {
            if(populator == null) populator = GameObject.Find("Game Manager").GetComponent<assetDataPopulator>();
            if(snapshotCamera == null) snapshotCamera = SnapshotCamera.MakeSnapshotCamera("SnapshotLayer");

            UpdatePreview();


            objectToSnapshot = populator.cellBlocks[assetIndex].primeCell.cellObj;
        }
    }

    public void takePicture()
    {
        UpdatePreview();
        System.IO.FileInfo fi = SnapshotCamera.SavePNG(texture);

        Debug.Log(string.Format("Snapshot {0} saved to {1}", fi.Name, fi.DirectoryName));
    }

    public void setCameraActive()
    {
        camActive = true;
    }

    public void setCameraFalse()
    {
        camActive = false;

    }

    public void nextAsset()
    {
        assetIndex++;
    }

    public void prevAsset()
    {
        assetIndex--;

    }

}
