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

    private bool isTurn = false;

    private enum BallState
    {
        WAITTOSTART,
        MOVING,
        STOPPED
    }

    private BallState ballState = BallState.WAITTOSTART;

    public void StartTurn()
    {
        isTurn = true;
        ballState = BallState.WAITTOSTART;

        cameraman.gameObject.SetActive(true);
    }

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
        if (valid && ballState == BallState.WAITTOSTART)
        {
            float speed = MaxSpeed * CalcPower(dur / 1000);
            Debug.Log("Triggered: " + dur);
            Debug.Log("Power: " + speed);

            if (useController) {
                rb.AddForce(speed * cameraman.forward);
            }

            ballState = BallState.MOVING;
            cameraman.gameObject.SetActive(false);

            controller.TryReset();
        }
    }

	// Update is called once per frame
	void Update () {

        if (isTurn)
        {
            useController = controller.IsConnected();

            if (useController)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    controller.TryReset();
                }
            }

            if(ballState == BallState.MOVING)
            {
                if(rb.velocity == Vector3.zero)
                {
                    ballState = BallState.STOPPED;
                    cameraman.gameObject.SetActive(false);
                    GameManager.instance.EndTurn();
                }
            }

            if (Input.GetKeyDown(KeyCode.W) && ballState == BallState.WAITTOSTART)
            {
                tm = 0;
                held = true;
                indicators[0].GetComponent<MeshRenderer>().enabled = true;
            }

            if (held && tm < MaxSpeed)
            {
                tm += 0.1f;
            }

            for (int i = 0; i < indicators.Length; i++)
            {
                if (tm > MaxSpeed / (MaxSpeed / (i + 1)))
                    indicators[i].GetComponent<MeshRenderer>().enabled = true;
            }

            if (Input.GetKeyUp(KeyCode.W) && ballState == BallState.WAITTOSTART && held)
            {
                rb.AddForce(500 * tm * cameraman.forward);
                tm = 0;
                held = false;
                for (int i = 0; i < indicators.Length; i++)
                {
                    indicators[i].GetComponent<MeshRenderer>().enabled = false;
                }

                ballState = BallState.MOVING;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                rb.velocity = Vector3.zero;
                tm = 0;
                held = false;
                for (int i = 0; i < indicators.Length; i++)
                {
                    indicators[i].GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }
	}

}
