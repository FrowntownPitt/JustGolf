using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class LevelManager : MonoBehaviour {

    public Camera sceneCamera;

    public List<GameObject> PlayerStarts;

	// Use this for initialization
	void Start () {
        GameManager.instance.PlacePlayers(PlayerStarts);
        GameManager.instance.InitLevel();
        DisableSceneCamera();
    }

    public void DisableSceneCamera()
    {
        sceneCamera.gameObject.SetActive(false);
    }
	
}
