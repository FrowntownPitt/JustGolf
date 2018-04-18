using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// "Destructible" game elements, tile drops when rolled over
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
				pHit = other.gameObject.transform.root.name;
				Debug.Log (pHit);
			}
		}
	}

	void OnTriggerExit(Collider other){
        // When the same initial player leaves this tile, drop it
		if (other.gameObject.transform.root.name.Equals(pHit)) {
			Debug.Log ("Fall!");
			rb.useGravity = true;
			rb.isKinematic = false;
		}
	}
}
