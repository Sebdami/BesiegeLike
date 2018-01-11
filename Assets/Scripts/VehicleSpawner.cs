using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour {
    public static int vehiclesRemaining = 0;

    //[SerializeField]
    //GameObject vehicleToSpawn;
    [SerializeField]
    TextAsset vehicleToSpawnFile;
    [SerializeField]
    Transform[] spawnPositions;

    List<GameObject> spawnedVehicles = new List<GameObject>();

    public static int VehiclesRemaining
    {
        get
        {
            return vehiclesRemaining;
        }

        set
        {
            vehiclesRemaining = value;
            if(vehiclesRemaining <= 0 && GameManager.instance.GameState == GameManager.GameStateEnum.Play)
            {
                GameManager.instance.WinLevel();
            }
        }
    }

    void Start () {
        if (BlockDatabase.instance.BlocksInitialised)
            SpawnVehicles();
        else
            BlockDatabase.OnBlocksInitialised += SpawnVehicles;
        GameManager.OnGameStateChange += GameStateChange;
	}

    void GameStateChange(GameManager.GameStateEnum state)
    {
        if(state == GameManager.GameStateEnum.Editor)
            SpawnVehicles();
    }

    void SpawnVehicles()
    {
        vehiclesRemaining += spawnPositions.Length;
        if (spawnedVehicles.Count > 0)
            foreach (GameObject go in spawnedVehicles)
                Destroy(go);
        foreach (Transform tr in spawnPositions)
        {
            spawnedVehicles.Add(VehicleBuilder.LoadVehicleFromString(vehicleToSpawnFile.text));
            GameObject veh = spawnedVehicles[spawnedVehicles.Count - 1];
            veh.transform.parent = tr;
            veh.transform.localPosition = Vector3.zero;
            veh.transform.localRotation = Quaternion.identity;
            SpecialBlock[] speBlocks = veh.GetComponentsInChildren<SpecialBlock>();
            foreach (SpecialBlock b in speBlocks)
            {
                b.enabled = false;
            }
        }
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= GameStateChange;
        BlockDatabase.OnBlocksInitialised -= SpawnVehicles;
    }
}
