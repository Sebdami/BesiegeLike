using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;

public static class Directories
{
    public static string VEHICLE_SAVE_DIRECTORY { get { return "SavedVehicles/"; } }
    public static string CACHE_DIRECTORY { get { return "Cache/"; } }
}

public class GameManager : MonoBehaviour {
    public enum GameStateEnum
    {
        Editor,
        Play
    }
    public static GameManager instance;
    public delegate void GameStateChange(GameStateEnum state);

    public static GameStateChange OnGameStateChange;
    [SerializeField]
    GameObject[] Blocks;

    [SerializeField]
    GameObject selectedBlock;

    GameObject vehicle;

    GameObject preview;

    bool disableBuildingControls = false;
    bool disableCameraControls = false;

    GameStateEnum gameState = GameStateEnum.Editor;

    public GameObject Vehicle
    {
        get
        {
            return vehicle;
        }

        set
        {
            vehicle = value;
        }
    }

    public GameStateEnum GameState
    {
        get
        {
            return gameState;
        }

        set
        {
            gameState = value;
            if (OnGameStateChange != null)
                OnGameStateChange(value);
        }
    }

    public bool DisableBuildingControls
    {
        get
        {
            return disableBuildingControls;
        }

        set
        {
            disableBuildingControls = value;
        }
    }

    public bool DisableCameraControls
    {
        get
        {
            return disableCameraControls;
        }

        set
        {
            disableCameraControls = value;
        }
    }

    public GameObject SelectedBlock
    {
        get
        {
            return selectedBlock;
        }

        set
        {
            selectedBlock = value;
            if (preview)
                DestroyImmediate(preview);
            preview = Instantiate(selectedBlock);
            if(preview.GetComponent<Collider>())
                DestroyImmediate(preview.GetComponent<Collider>());
            MeshRenderer[] renderers = preview.GetComponentsInChildren<MeshRenderer>();
            for(int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.SetColor("_EmissionColor", Color.white);
            }
            preview.SetActive(false);
        }
    }

    private void Start()
    {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);
        GameState = GameStateEnum.Editor;
        InitEmptyVehicleWithCore();
        Time.timeScale = 0.0f;
        SelectedBlock = Blocks[1];
    }

    public void InitEmptyVehicleWithCore()
    {
        if (vehicle)
            DestroyImmediate(vehicle);

        vehicle = new GameObject("Vehicle", typeof(Rigidbody));
        Rigidbody rb = vehicle.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.drag = 0.5f;
        rb.angularDrag = 2.0f;

        AddBlock(Blocks[0], vehicle.transform.position, vehicle.transform.rotation);

    }

    public void InitEmptyVehicle()
    {
        if (vehicle)
            Destroy(vehicle);

        vehicle = new GameObject("Vehicle", typeof(Rigidbody));
        Rigidbody rb = vehicle.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.drag = 0.5f;
        rb.angularDrag = 2.0f;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectedBlock = Blocks[1];
        else if(Input.GetKeyDown(KeyCode.Alpha2))
            SelectedBlock = Blocks[2];
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SelectedBlock = Blocks[3];
        if (disableBuildingControls)
        {
            if (preview.activeSelf)
                preview.SetActive(false);
            return;
        }
            
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Block")))
        {
            
            Transform anchor = hit.collider.GetComponent<Block>().GetAnchorFromPositionAndNormal(hit.point, hit.normal);
            if (anchor != null)
            {
                // Check for collisions
                Vector3 pos = anchor.localToWorldMatrix * SelectedBlock.GetComponent<Block>().Extent; // using localToWorldMatrix * instead of localToWorldMatrix.MultiplyPoint allows to only match world rotation
                pos += anchor.position;
                if (Physics.OverlapCapsule(anchor.position, pos, .25f).Length > 0)
                {
                    return;
                }
                preview.SetActive(true);
                preview.transform.position = anchor.position;
                preview.transform.rotation = anchor.rotation;
                if (Input.GetMouseButtonUp(0))
                {
                    AddBlock(anchor);
                }
            }
            else
            {
                if (preview.activeSelf)
                    preview.SetActive(false);
            }
            if (Input.GetMouseButtonUp(1) && hit.collider.GetComponent<CoreBlock>() == null) //Delete blocks but not the core
            {
                DeleteBlock(hit.collider.GetComponent<Block>());
            }
        }
        else
        {
            if (preview.activeSelf)
                preview.SetActive(false);
        }
    }

    void AddBlock(Transform anchor)
    {
        //For now only instantiate
        GameObject go = Instantiate(SelectedBlock, anchor);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.SetParent(vehicle.transform);
        //Rigidbody rb = go.AddComponent<Rigidbody>();
        //rb.useGravity = false;
        //FixedJoint fj = go.AddComponent<FixedJoint>();
        //fj.connectedBody = vehicle.GetComponent<Rigidbody>();
        //fj.autoConfigureConnectedAnchor = false;

    }

    void AddBlock(GameObject block, Vector3 position, Quaternion rotation)
    {
        GameObject go = Instantiate(block, vehicle.transform);
        go.transform.position = position;
        go.transform.rotation = rotation;
        //Rigidbody rb = go.AddComponent<Rigidbody>();
        //rb.useGravity = false;
        //FixedJoint fj = go.AddComponent<FixedJoint>();
        //fj.connectedBody = vehicle.GetComponent<Rigidbody>();
        //fj.autoConfigureConnectedAnchor = false;
    }

    void DeleteBlock(Block block)
    {
        //For now only destroy it, add list gestion later
        Destroy(block.gameObject);
    }

    public void SetSelectedBlockFromIndex(int index)
    {
        if (index < 0 || index >= Blocks.Length)
            return;
        SelectedBlock = Blocks[index];
    }

    public void EnterPlayMode()
    {
        if (preview.activeSelf)
            preview.SetActive(false);
        if (vehicle.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = vehicle.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.drag = 0.5f;
            rb.angularDrag = 2.0f;
            rb.mass = vehicle.transform.childCount;
        }
        else
        {
            vehicle.GetComponent<Rigidbody>().mass = vehicle.transform.childCount;
        }

        SaveVehicle(Directories.CACHE_DIRECTORY + "CachedVehicle.tmp");
        
        Time.timeScale = 1.0f;
        GameState = GameStateEnum.Play;
        this.enabled = false;
    }

    public void ExitPlayMode()
    {
        //Destroy(vehicle.GetComponent<Rigidbody>());
        //Rigidbody rb = vehicle.GetComponent<Rigidbody>();
        //rb.velocity = Vector3.zero;
        //rb.angularVelocity = Vector3.zero;
        //vehicle.transform.position = Vector3.zero;
        //vehicle.transform.rotation = Quaternion.identity;
        this.enabled = true;
        Time.timeScale = 0.0f;
        GameState = GameStateEnum.Editor;
        LoadVehicle(Directories.CACHE_DIRECTORY + "CachedVehicle.tmp");
        File.Delete(Directories.CACHE_DIRECTORY + "CachedVehicle.tmp");
    }

    public bool SaveVehicle(string filepath)
    {
        int removeIndex = filepath.LastIndexOf('/');
        string directory = filepath.Remove(removeIndex);

        if(!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        StreamWriter sw = new StreamWriter(filepath);
        if(sw == null)
        {
            return false;
        }
        sw.WriteLine(vehicle.transform.childCount);
        for(int i = 0; i < vehicle.transform.childCount; i++)
        {
            Transform child = vehicle.transform.GetChild(i);
            sw.WriteLine(child.GetComponent<Block>().Id);
            sw.WriteLine(child.position.x + "," + child.position.y + "," + child.position.z);
            sw.WriteLine(child.rotation.x + "," + child.rotation.y + "," + child.rotation.z + "," + child.rotation.w);
        }
        sw.Close();
        return true;
    }

    public bool LoadVehicle(string filePath)
    {
        StreamReader sr = new StreamReader(filePath);
        if(sr == null)
        {
            return false;
        }

        InitEmptyVehicle();

        int nbBlocks = int.Parse(sr.ReadLine());

        for(int i = 0; i < nbBlocks; i++)
        {
            int id = int.Parse(sr.ReadLine());

            string[] posCoords = sr.ReadLine().Split(',');
            string[] rotCoords = sr.ReadLine().Split(',');
            Vector3 pos = new Vector3(float.Parse(posCoords[0]), float.Parse(posCoords[1]), float.Parse(posCoords[2]));
            Quaternion rot = new Quaternion(float.Parse(rotCoords[0]), float.Parse(rotCoords[1]), float.Parse(rotCoords[2]), float.Parse(rotCoords[3]));
            AddBlock(Blocks[id], pos, rot); // Do better by adding a GetBlockTypeById function somewhere
        }
        sr.Close();
        return true;
    }

}
