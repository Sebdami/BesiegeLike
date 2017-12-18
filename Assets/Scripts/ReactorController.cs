using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactorController : MonoBehaviour {
    float thrust = 5.0f;
    [SerializeField]
    KeyCode key = KeyCode.Space;

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

    void Start () {
        Vector3 vec = Utils.GetClosestCartesianFromVector(-transform.forward);
        if (vec == Vector3.down)
            key = KeyCode.DownArrow;
        if (vec == Vector3.up)
            key = KeyCode.UpArrow;
        if (vec == Vector3.right)
            key = KeyCode.RightArrow;
        if (vec == Vector3.left)
            key = KeyCode.LeftArrow;
        if (vec == Vector3.forward)
            key = KeyCode.Space;
        if (vec == Vector3.back)
            key = KeyCode.LeftShift;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
		if(Input.GetKey(key))
        {
            if(transform.parent)
                transform.parent.GetComponent<Rigidbody>().AddForceAtPosition(-transform.forward * thrust, transform.position, ForceMode.Acceleration);
        }
	}
}
