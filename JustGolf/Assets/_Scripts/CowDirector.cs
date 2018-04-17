using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowDirector : MonoBehaviour {

	public GameObject door;
	int i = 0;

	// Update is called once per frame
	void Update () {
		i++;
		if (i > 50) {
			i = 0;
			GameObject[] cows = GameObject.FindGameObjectsWithTag ("Cow");
			if (cows.Length == 0) {
				Destroy (door);
			}
		}
	}
}
