using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {
	Rigidbody rb;
	Transform tf;

	public Transform cameraman;
	float tm;
	bool held;

	public GameObject[] indicators;
	public float MaxSpeed;

	// Use this for initialization
	void Start () {
		rb = gameObject.GetComponent<Rigidbody> ();
		tf = gameObject.transform;
		held = false;

		for (int i = 0; i < indicators.Length; i++) {
			indicators [i].GetComponent<MeshRenderer> ().enabled = false;
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.W) && rb.velocity == Vector3.zero) {
			tm = 0;
			held = true;
			indicators [0].GetComponent<MeshRenderer> ().enabled = true;
		}

		if (held && tm < MaxSpeed) {
			tm += 0.1f;
		}

		for (int i = 0; i < indicators.Length; i++) {
			if(tm > MaxSpeed/(MaxSpeed/(i+1)))
				indicators [i].GetComponent<MeshRenderer> ().enabled = true;
		}

		if (Input.GetKeyUp(KeyCode.W)) {
			rb.AddForce (500*tm*cameraman.forward);
			tm = 0;
			held = false;
			for (int i = 0; i < indicators.Length; i++) {
				indicators [i].GetComponent<MeshRenderer> ().enabled = false;
			}
		}

		if (Input.GetKeyDown (KeyCode.F)) {
			rb.velocity = Vector3.zero;
			tm = 0;
			held = false;
			for (int i = 0; i < indicators.Length; i++) {
				indicators [i].GetComponent<MeshRenderer> ().enabled = false;
			}
		}
	}

}
