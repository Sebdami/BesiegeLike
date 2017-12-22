using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour {
    [SerializeField]
    GameObject vehicleToSpawn;

    GameObject spawnedVehicle;

	void Start () {
        SpawnVehicle();
        GameManager.OnGameStateChange += GameStateChange;
	}

    void GameStateChange(GameManager.GameStateEnum state)
    {
        if(state == GameManager.GameStateEnum.Editor)
            SpawnVehicle();
    }

    void SpawnVehicle()
    {
        if (spawnedVehicle != null)
            Destroy(spawnedVehicle);
        spawnedVehicle = Instantiate(vehicleToSpawn, transform);
        spawnedVehicle.transform.localPosition = Vector3.zero;
        spawnedVehicle.transform.localRotation = Quaternion.identity;
        SpecialBlock[] speBlocks = spawnedVehicle.GetComponentsInChildren<SpecialBlock>();
        foreach (SpecialBlock b in speBlocks)
        {
            b.enabled = false;
        }
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= GameStateChange;
    }
}
