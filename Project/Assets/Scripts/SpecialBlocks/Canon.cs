using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canon : SpecialBlock {
    [SerializeField]
    GameObject CanonBallPrefab;
    [SerializeField]
    Transform CanonEnd;
    [SerializeField]
    float shootForce = 100.0f;
    [SerializeField]
    float shootSpeed = 0.1f;

    float shootTimer = 0.0f;

    bool canShoot = true;

    // Update is called once per frame
    void Update () {
        shootTimer += Time.deltaTime;

        if(shootTimer > shootSpeed)
        {
            canShoot = true;
            shootTimer = 0.0f;
        }
		if(Input.GetKey(keys[0]) && canShoot && GameManager.instance.GameState == GameManager.GameStateEnum.Play)
        {
            GameObject go = Instantiate(CanonBallPrefab);
            go.transform.position = CanonEnd.position;
            go.transform.rotation = CanonEnd.rotation;
            go.GetComponent<Rigidbody>().AddForce(transform.root.GetComponent<Rigidbody>().velocity + transform.forward * shootForce, ForceMode.Impulse);

            GetComponentInParent<Rigidbody>().AddForceAtPosition(-transform.forward*2.0f, transform.position, ForceMode.Impulse);

            canShoot = false;
        }
	}
}
