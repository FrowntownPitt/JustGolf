using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyPanel : MonoBehaviour {

	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("Player")) {
			Debug.Log ("Lose");
		} else {
			Destroy(other.gameObject);
		}
	}

}
