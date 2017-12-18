using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorCameraController : MonoBehaviour {
    Vector3 startPos;

    [SerializeField]
    Transform CameraPoint;

    [SerializeField]
    float rotateSpeedY = 60.0f;

    [SerializeField]
    float rotateSpeedX = 40.0f;

    [SerializeField]
    float scrollSpeed = 70.0f;

    [SerializeField]
    Vector3 cameraFollowOffset = new Vector3(0.0f,  3.0f, -10.0f);

    private void Start()
    {
        GameManager.OnGameStateChange += ChangeControlsWithGameState;
        startPos = transform.position;
    }

    void Update () {
        if(GameManager.instance.GameState == GameManager.GameStateEnum.Editor && !GameManager.instance.DisableBuildingControls)
        {
            transform.LookAt(CameraPoint);
            transform.RotateAround(CameraPoint.position, Vector3.up, -Input.GetAxisRaw("Horizontal") * Time.unscaledDeltaTime * rotateSpeedY);

            transform.position += transform.forward * Input.mouseScrollDelta.y * Time.unscaledDeltaTime * scrollSpeed;
            transform.RotateAround(CameraPoint.position, transform.right, Input.GetAxisRaw("Vertical") * Time.unscaledDeltaTime * rotateSpeedX);
        }
	}

    private void FixedUpdate()
    {
        //if(GameManager.instance.GameState == GameManager.GameStateEnum.Play)
        //{
        //    transform.LookAt(GameManager.instance.Vehicle.transform.position);
        //    transform.position = GameManager.instance.Vehicle.transform.position - cameraFollowOffset;
        //}
    }

    void ChangeControlsWithGameState()
    {
        if(GameManager.instance.GameState == GameManager.GameStateEnum.Editor)
        {
            //temp
            transform.SetParent(null);
            transform.position = startPos;
        }
        else if(GameManager.instance.GameState == GameManager.GameStateEnum.Play)
        {
            startPos = transform.position;
            //temp
            transform.SetParent(GameManager.instance.Vehicle.transform);
            transform.localPosition = cameraFollowOffset;
            transform.LookAt(GameManager.instance.Vehicle.transform.position);
        }
    }

    private void OnDestroy()
    {
        if (GameManager.OnGameStateChange != null)
            GameManager.OnGameStateChange -= ChangeControlsWithGameState;
    }
}
