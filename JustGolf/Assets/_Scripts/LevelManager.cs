using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class LevelManager : MonoBehaviour {

    public List<GameObject> PlayerStarts;

	// Use this for initialization
	void Start () {
        Debug.Log("Baking");
        Debug.Log(UnityEditor.Lightmapping.Bake());
        Debug.Log("Baking finished");

        GameManager.instance.PlacePlayers(PlayerStarts);
        //LightMapping.Bake();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
