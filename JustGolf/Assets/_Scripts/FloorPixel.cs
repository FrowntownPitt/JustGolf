using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPixel : MonoBehaviour {
	public Material red;
	Rigidbody rb;
	Renderer rend;
	string pHit;
	bool hit;


	// Use this for initialization
	void Start () {
		hit = false;
		rb = gameObject.GetComponent<Rigidbody> ();
		rend = gameObject.GetComponent<Renderer> ();
	}

	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("Player")) {
			if (!hit) {
				hit = true;
				rend.material = red;
				pHit = other.gameObject.transform.parent.name;
				Debug.Log (pHit);
			}
		}
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.transform.parent.name.Equals(pHit)) {
			Debug.Log ("Fall!");
			rb.useGravity = true;
			rb.isKinematic = false;
		}
	}
}
