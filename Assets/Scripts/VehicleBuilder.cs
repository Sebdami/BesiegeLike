using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VehicleBuilder {
    public static void AddBlock(GameObject vehicleToAttachTo, GameObject block, Vector3 position, Quaternion rotation, bool isPlayer = false)
    {

        GameObject go = GameObject.Instantiate(block, vehicleToAttachTo.transform);
        go.transform.position = position;
        go.transform.rotation = rotation;

        if (isPlayer && go.GetComponent<CoreBlock>() != null)
            go.GetComponent<CoreBlock>().IsPlayer = true;
    }

    public static GameObject LoadVehicleFromString(string data, GameObject vehicle = null)
    {
        StringReader sr = new StringReader(data);
        if (sr == null)
        {
            return null;
        }
        int nbBlocks = int.Parse(sr.ReadLine());

        if(vehicle == null)
        {
            vehicle = new GameObject("Vehicle", typeof(Rigidbody));
            Rigidbody rb = vehicle.GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.drag = 0.5f;
            rb.angularDrag = 2.0f;
        }

        for (int i = 0; i < nbBlocks; i++)
        {
            int id = int.Parse(sr.ReadLine());

            string[] posCoords = sr.ReadLine().Split(',');
            string[] rotCoords = sr.ReadLine().Split(',');
            Vector3 pos = new Vector3(float.Parse(posCoords[0]), float.Parse(posCoords[1]), float.Parse(posCoords[2]));
            Quaternion rot = new Quaternion(float.Parse(rotCoords[0]), float.Parse(rotCoords[1]), float.Parse(rotCoords[2]), float.Parse(rotCoords[3]));
            AddBlock(vehicle, BlockDatabase.instance.GetBlockPrefabById(id), pos, rot, false);
        }
        sr.Close();
        return vehicle;
    }
}
