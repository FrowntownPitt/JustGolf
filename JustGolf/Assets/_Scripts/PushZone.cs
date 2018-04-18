using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushZone : MonoBehaviour {
	public Vector3 direction;

	void OnTriggerStay(Collider other){
		if (other.CompareTag ("Player")) {
			other.GetComponent<Rigidbody> ().AddForce (direction);
		}
	}

}
