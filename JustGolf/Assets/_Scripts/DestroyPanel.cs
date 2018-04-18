using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyPanel : MonoBehaviour {

    // "Game over" panel (reset player on level). Destroy if not player
	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("Player")) {
			Debug.Log ("Lose");
		} else {
			Destroy(other.gameObject);
		}
	}

}
