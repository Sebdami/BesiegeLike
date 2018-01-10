using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class BlockDatabase : MonoBehaviour {
    public static BlockDatabase instance;
    public delegate void Initialisation();

    public static Initialisation OnBlocksInitialised;

    bool blocksInitialised = false;

    [SerializeField]
    public GameObject[] Blocks;


    public bool BlocksInitialised
    {
        get
        {
            return blocksInitialised;
        }

        set
        {
            blocksInitialised = value;
        }
    }

    private void Start()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(this);
            StartCoroutine(LoadBlocks());
        }
        else
            Destroy(gameObject);
    }


    int CompareBlockById(GameObject go1, GameObject go2)
    {
        int id1 = go1.GetComponent<Block>().Id;
        int id2 = go2.GetComponent<Block>().Id;
        if (id1 > id2)
            return 1;
        if (id1 < id2)
            return -1;
        return 0;
    }

    public IEnumerator LoadBlocks()
    {
        string[] blockFileNames = Directory.GetFiles(Directories.BLOCK_BUNDLES_DIRECTORY, "*.manifest");
        List<GameObject> blockList = new List<GameObject>();
        foreach (string fileName in blockFileNames)
        {
            string bundleName = fileName.Replace(".manifest", "");
            string uri = "file:///" + Application.dataPath + "/../" + bundleName;
            //string assetName = fileName.Remove(0, fileName.LastIndexOf('/'));
            UnityWebRequest request = UnityWebRequest.GetAssetBundle(uri, 0);
            yield return request.Send();
            AssetBundle bundle = null;
            try
            {
                bundle = DownloadHandlerAssetBundle.GetContent(request);
            }
            catch(Exception e)
            {
                Debug.LogError("Block loading failed " + e.Message);
            }
            if (bundle == null)
            {
                Debug.LogError("Failed loading blocks");
                yield return null;
            }
            GameObject[] gos = bundle.LoadAllAssets<GameObject>();
            foreach (GameObject go in gos)
            {
                if (go.GetComponent<Block>())
                {
                    if (blockList.Find(x => x.GetComponent<Block>().Id == go.GetComponent<Block>().Id))
                    {
                        continue;
                    }
                    blockList.Add(go);
                }
            }
        }
        blockList.Sort(CompareBlockById);
        Blocks = blockList.ToArray();
        if (OnBlocksInitialised != null)
            OnBlocksInitialised();
        blocksInitialised = true;
    }


    public Block GetBlockById(int id)
    {
        Block b = Array.Find<GameObject>(Blocks, x => x.GetComponent<Block>().Id == id).GetComponent<Block>();
        if (b)
            return b;
        return null;
    }

    public GameObject GetBlockPrefabById(int id)
    {
        GameObject go = Array.Find<GameObject>(Blocks, x => x.GetComponent<Block>().Id == id);
        if (go)
            return go;
        return null;
    }

    public int GetBlockIndexById(int id)
    {
        int i = Array.FindIndex<GameObject>(Blocks, x => x.GetComponent<Block>().Id == id);
        return i;
    }
}
