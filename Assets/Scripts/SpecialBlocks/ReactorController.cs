using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactorController : SpecialBlock {
    [SerializeField]
    float thrust = 5.0f;

    [SerializeField]
    LensFlare flare;

    [SerializeField]
    float maxFlareIntensity = 0.3f;

    float flareIntensity;

    void Start () {
        Vector3 vec = Utils.GetClosestCartesianFromVector(-transform.forward);

        keys = new KeyCode[2];
        if (vec == Vector3.down)
        {
            keys = new KeyCode[3];
            keys[0] = KeyCode.DownArrow;
            keys[1] = KeyCode.S;
            Vector3 testPos = transform.parent.position;
            testPos.z = transform.position.z;
            Vector3 test = testPos - transform.position;
            if(Vector3.Dot(transform.parent.right, test) > 0)
            {
                keys[2] = KeyCode.A;
            }
            else if (Vector3.Dot(transform.position, testPos) < 0)
            {
                keys[2] = KeyCode.E;
            }
        }
        if (vec == Vector3.up)
        {
            keys = new KeyCode[3];
            keys[0] = KeyCode.UpArrow;
            keys[1] = KeyCode.Z;
            Vector3 testPos = transform.parent.position;
            testPos.z = transform.position.z;
            Vector3 test = testPos - transform.position;
            if (Vector3.Dot(transform.parent.right, test) > 0)
            {
                keys[2] = KeyCode.E;
            }
            else if (Vector3.Dot(transform.position, testPos) < 0)
            {
                keys[2] = KeyCode.A;
            }
        }
        if (vec == Vector3.right)
        {
            keys[0] = KeyCode.RightArrow;
            keys[1] = KeyCode.D;
        }
        if (vec == Vector3.left)
        {
            keys[0] = KeyCode.LeftArrow;
            keys[1] = KeyCode.Q;
        }
        if (vec == Vector3.forward)
        {
            keys[0] = KeyCode.Space;
        }
        if (vec == Vector3.back)
        {
            keys[0] = KeyCode.LeftShift;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        foreach(KeyCode key in keys)
        {
            if (Input.GetKey(key))
            {
                flareIntensity += Time.deltaTime * 2.0f;
                if (isAttached)
                    transform.parent.GetComponent<Rigidbody>().AddForceAtPosition(-transform.forward * thrust, transform.position, ForceMode.Force);
                else
                    GetComponent<Rigidbody>().AddForceAtPosition(-transform.forward * thrust, transform.position, ForceMode.Force);
            }
        }
        flareIntensity -= Time.deltaTime;
        flareIntensity = Mathf.Clamp(flareIntensity, 0.0f, maxFlareIntensity);
        if (flare)
            flare.brightness = flareIntensity;
    }
}
