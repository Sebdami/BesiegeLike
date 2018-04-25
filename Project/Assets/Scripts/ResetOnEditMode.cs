using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetOnEditMode : MonoBehaviour {

    Vector3 startPos;
    Quaternion startRot;

    Rigidbody rb;

	// Use this for initialization
	void Start () {
        startPos = transform.position;
        startRot = transform.rotation;
        rb = GetComponent<Rigidbody>();
            
        GameManager.OnGameStateChange += GameStateChanged;
	}
	
	void GameStateChanged(GameManager.GameStateEnum state)
    {
        if (state == GameManager.GameStateEnum.Editor)
            ResetObject();
    }

    private void ResetObject()
    {
        transform.position = startPos;
        transform.rotation = startRot;
        if (rb)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void OnDestroy()
    {
        GameManager.OnGameStateChange -= GameStateChanged;
    }
}
