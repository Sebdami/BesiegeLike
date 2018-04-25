using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnEditMode : MonoBehaviour {

	void Start () {
        GameManager.OnGameStateChange += GameStateChanged;
	}

    void GameStateChanged(GameManager.GameStateEnum state)
    {
        if (state == GameManager.GameStateEnum.Editor)
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= GameStateChanged;
    }
}
