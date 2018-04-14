using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour {
	public Transform goal;

	void OnTriggerEnter(Collider coll){
		coll.gameObject.transform.SetPositionAndRotation (goal.position, coll.gameObject.transform.rotation);
		coll.gameObject.GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, 0);
	}
}
