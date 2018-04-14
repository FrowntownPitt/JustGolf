using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {

	public Transform following;
	
	// Update is called once per frame
	void Update () {
		gameObject.transform.SetPositionAndRotation (following.position, gameObject.transform.rotation);
		if (Input.GetKey (KeyCode.A)) {
			gameObject.transform.Rotate (0, -2, 0);
		}
		if (Input.GetKey (KeyCode.D)) {
			gameObject.transform.Rotate (0, 2, 0);
		}
	}
}
