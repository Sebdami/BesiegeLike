using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : MonoBehaviour {
    [SerializeField]
    GameObject CanonBallPrefab;
    [SerializeField]
    Transform CanonEnd;
    [SerializeField]
    KeyCode key = KeyCode.C;
    [SerializeField]
    float shootForce = 10.0f;

    public KeyCode Key
    {
        get
        {
            return key;
        }

        set
        {
            key = value;
        }
    }

    // Update is called once per frame
    void Update () {
		if(Input.GetKey(key))
        {
            GameObject go = Instantiate(CanonBallPrefab);
            go.transform.position = CanonEnd.position;
            go.transform.rotation = CanonEnd.rotation;
            go.GetComponent<Rigidbody>().AddForce(transform.root.GetComponent<Rigidbody>().velocity + transform.forward * shootForce, ForceMode.Impulse);
        }
	}
}
