using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowDirector : MonoBehaviour {

	public GameObject door;
	int i = 0;
    
	void Update () {
		i++;
		if (i > 50) {
			i = 0;
            // Check for cows.  When all are collected, open the door
			GameObject[] cows = GameObject.FindGameObjectsWithTag ("Cow");
			if (cows.Length == 0) {
				Destroy (door);
			}
		}
	}
}
