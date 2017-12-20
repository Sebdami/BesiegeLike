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
    float shootForce = 100.0f;
    [SerializeField]
    float shootSpeed = 0.1f;

    float shootTimer = 0.0f;

    bool canShoot = true;

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
        shootTimer += Time.deltaTime;

        if(shootTimer > shootSpeed)
        {
            canShoot = true;
            shootTimer = 0.0f;
        }
		if(Input.GetKey(key) && canShoot)
        {
            GameObject go = Instantiate(CanonBallPrefab);
            go.transform.position = CanonEnd.position;
            go.transform.rotation = CanonEnd.rotation;
            go.GetComponent<Rigidbody>().AddForce(transform.root.GetComponent<Rigidbody>().velocity + transform.forward * shootForce, ForceMode.Impulse);
            canShoot = false;
        }
	}
}
