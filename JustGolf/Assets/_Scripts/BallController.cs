using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arduino;

public class BallController : MonoBehaviour {

    [SerializeField]
    Communication controller;
    bool useController;

	Rigidbody rb;
	Transform tf;

	public Transform cameraman;
	float tm;
	bool held;

	public GameObject[] indicators;
	public float MaxSpeed;

    public float mult = 118.768f;
    public float exp = -0.00879889f;
    public float offset = 10;

    // Use this for initialization
    void Start () {
		rb = gameObject.GetComponent<Rigidbody> ();
		tf = gameObject.transform;
		held = false;

		for (int i = 0; i < indicators.Length; i++) {
			indicators [i].GetComponent<MeshRenderer> ().enabled = false;
		}

        controller.AddHandler("TRIGGER", (string s) => HandleBallHit(s));
    }

    public float CalcPower(long time)
    {
        return mult * (Mathf.Exp(exp * time)) + offset;
    }


    private void HandleBallHit(string message)
    {
        string[] m = message.Split();
        string t = m[1];
        long dur;
        bool valid = long.TryParse(t, out dur);
        if (valid)
        {
            float speed = MaxSpeed * CalcPower(dur / 1000);
            Debug.Log("Triggered: " + dur);
            Debug.Log("Power: " + speed);

            if (useController) {
                rb.AddForce(speed * cameraman.forward);
            }
        }
    }

	// Update is called once per frame
	void Update () {

        useController = controller.IsConnected();

        if (useController)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                controller.TryReset();
            }
        }

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
