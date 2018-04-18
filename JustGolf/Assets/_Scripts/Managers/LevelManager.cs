using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class LevelManager : MonoBehaviour {

    public Camera sceneCamera; // Camera of the scene

    public List<GameObject> PlayerStarts; // Track the starting locations for each player

	// Use this for initialization
	void Start () {
        GameManager.instance.PlacePlayers(PlayerStarts); // Place the players at the given locations
        GameManager.instance.InitLevel(); // Start up the level
        DisableSceneCamera(); // disable the scene camera so Unity stops complaining
    }

    public void DisableSceneCamera()
    {
        sceneCamera.gameObject.SetActive(false);
    }
	
}
