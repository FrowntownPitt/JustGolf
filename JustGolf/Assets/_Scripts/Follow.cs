using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Camera follower
public class Follow : MonoBehaviour {

	public Transform following; // What are we following?
	
	// Update is called once per frame
	void Update () {
        // On A/D, turn side to side
		gameObject.transform.SetPositionAndRotation (following.position, gameObject.transform.rotation);
		if (Input.GetKey (KeyCode.A)) {
			gameObject.transform.Rotate (0, -2, 0);
		}
		if (Input.GetKey (KeyCode.D)) {
			gameObject.transform.Rotate (0, 2, 0);
		}
	}
}
