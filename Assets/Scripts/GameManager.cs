using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public static class Directories
{
    public static string VEHICLE_SAVE_DIRECTORY { get { return "SavedVehicles/"; } }
    public static string CACHE_DIRECTORY { get { return "Cache/"; } }
    public static string BLOCK_BUNDLES_DIRECTORY { get { return "AssetBundles/StandaloneWindows/blocks/"; } }
    public static string LEVEL_BUNDLES_DIRECTORY { get { return "AssetBundles/StandaloneWindows/levels/"; } }
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
            SpecialBlock[] spe = preview.GetComponents<SpecialBlock>();
            if (spe != null)
            {
                foreach (SpecialBlock speBlock in spe)
                    speBlock.enabled = false;
            }

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
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
            return;
        }  
        GameState = GameStateEnum.Editor;
        Time.timeScale = 0.0f;
        if (BlockDatabase.instance.BlocksInitialised)
            InitAfterBlockLoad();
        else
            BlockDatabase.OnBlocksInitialised += InitAfterBlockLoad;

    }

    void InitAfterBlockLoad()
    {
        GameManager.instance.SelectedBlock = BlockDatabase.instance.GetBlockPrefabById(1);
        InitEmptyVehicleWithCore();
        BlockDatabase.OnBlocksInitialised -= InitAfterBlockLoad;
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

        VehicleBuilder.AddBlock(vehicle, BlockDatabase.instance.GetBlockPrefabById(0), vehicle.transform.position, vehicle.transform.rotation, true);

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
        
        if (disableBuildingControls || !BlockDatabase.instance.BlocksInitialised)
        {
            if (preview && preview.activeSelf)
                preview.SetActive(false);
            return;
        }
            
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Block")) && hit.transform == vehicle.transform)
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

    void AddBlock(Transform anchor, bool isPlayer = false)
    {
        //For now only instantiate
        GameObject go = Instantiate(SelectedBlock, anchor);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        go.transform.SetParent(vehicle.transform);

        if (isPlayer && go.GetComponent<CoreBlock>() != null)
            go.GetComponent<CoreBlock>().IsPlayer = true;
    }

    void DeleteBlock(Block block)
    {
        //For now only destroy it, add list gestion later
        Destroy(block.gameObject);
    }

    public void SetSelectedBlockFromIndex(int index)
    {
        if (index < 0 || index >= BlockDatabase.instance.Blocks.Length)
            return;
        SelectedBlock = BlockDatabase.instance.Blocks[index];
    }

    public void SetSelectedBlockFromID(int id)
    {
        GameObject newBlock = BlockDatabase.instance.GetBlockPrefabById(id);
        if (newBlock)
            SelectedBlock = newBlock;
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
        GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
        foreach(GameObject go in projectiles)
            Destroy(go);

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
        string serializedVehicle = VehicleBuilder.SerializeVehicle(vehicle);
        if (serializedVehicle == string.Empty)
            return false;
        sw.WriteLine(serializedVehicle);
        sw.WriteLine(vehicle.transform.childCount);
        sw.Close();
        return true;
    }

    public bool LoadVehicle(string filePath)
    {
        Destroy(vehicle);
        vehicle = VehicleBuilder.LoadVehicleFromFile(filePath, null, true);
        if (vehicle == null)
            return false;
        return true;
    }
}
