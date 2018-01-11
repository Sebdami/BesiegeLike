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

    public static GameObject LoadVehicleFromString(string data, GameObject vehicle = null, bool isPlayer = false)
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
            AddBlock(vehicle, BlockDatabase.instance.GetBlockPrefabById(id), pos, rot, isPlayer);
        }
        sr.Close();
        return vehicle;
    }

    public static GameObject LoadVehicleFromFile(string filePath, GameObject vehicle = null, bool isPlayer = false)
    {
        StreamReader sr = new StreamReader(filePath);
        if (sr == null)
        {
            return null;
        }
        string data = sr.ReadToEnd();
        sr.Close();

        return LoadVehicleFromString(data, vehicle, isPlayer); ;
    }

    public static string SerializeVehicle(GameObject vehicle)
    {
        if (vehicle == null)
            return string.Empty;

        StringWriter sw = new StringWriter();
        if (sw == null)
        {
            return string.Empty;
        }
        sw.WriteLine(vehicle.transform.childCount);
        for (int i = 0; i < vehicle.transform.childCount; i++)
        {
            Transform child = vehicle.transform.GetChild(i);
            sw.WriteLine(child.GetComponent<Block>().Id);
            sw.WriteLine(child.position.x + "," + child.position.y + "," + child.position.z);
            sw.WriteLine(child.rotation.x + "," + child.rotation.y + "," + child.rotation.z + "," + child.rotation.w);
        }
        string toReturn = sw.ToString();
        sw.Close();
        return toReturn;
    }
}
