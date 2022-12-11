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


    private void Awake()
    {
        
    }

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

            
            if (objectToSnapshot.name.Contains("Right") && rotation != new Vector3(220, 250, 130)) rotation = new Vector3(220, 250, 130);
            if (objectToSnapshot.name.Contains("Left") && rotation != new Vector3(400, 250, 55)) rotation = new Vector3(400, 250, 55);
            if (objectToSnapshot.name.Contains("Forward") && rotation != new Vector3(325, 300, 50)) rotation = new Vector3(325, 300, 50);
            if (objectToSnapshot.name.Contains("Backward") && rotation != new Vector3(145, 295, 130)) rotation = new Vector3(145, 295, 130);

            // Take a new snapshot of the objectToSnapshot
            texture = snapshotCamera.TakeObjectSnapshot(objectToSnapshot, backgroundColor, position, Quaternion.Euler(rotation) , scale, width: 512, height: 512);
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


            if(Input.GetKeyUp(KeyCode.A)) print(rotation + objectToSnapshot.transform.rotation.eulerAngles);
        }
    }

    public void takePicture()
    {
        UpdatePreview();
        System.IO.FileInfo fi = SnapshotCamera.SavePNG(texture, objectToSnapshot.name + " Logo");

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
        if (assetIndex > GameObject.Find("Game Manager").GetComponent<assetDataPopulator>().list.listOfAssets.Count - 1) assetIndex = 0;
    }

    public void prevAsset()
    {
        assetIndex--;
        if (assetIndex < 0 ) assetIndex = GameObject.Find("Game Manager").GetComponent<assetDataPopulator>().list.listOfAssets.Count - 1;
    }

}
