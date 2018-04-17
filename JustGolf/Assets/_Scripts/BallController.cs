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

    public int score = 0;
    public int cumulativeScore = 0;

	public GameObject[] indicators;
	public float MaxSpeed;

    public float mult = 118.768f;
    public float exp = -0.00879889f;
    public float offset = 10;

    private bool isTurn = false;

    public bool isFinished = false;

    private enum BallState
    {
        WAITTOSTART,
        MOVING,
        STOPPED
    }

    public void ResetPlayer()
    {
        score = 0;
        cumulativeScore = 0;

        isTurn = false;
        isFinished = false;

        ballState = BallState.WAITTOSTART;
    }

    private BallState ballState = BallState.WAITTOSTART;

	public float hitStart = 0;
    public float currentTime;// = Time.time;

    public void StartTurn()
    {
        isTurn = true;
        ballState = BallState.WAITTOSTART;

        controller.TryReset();

        cameraman.gameObject.SetActive(true);
    }

    public void EndTurn()
    {
        isTurn = false;
        ballState = BallState.WAITTOSTART;

        cameraman.gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start () {
		rb = gameObject.GetComponent<Rigidbody> ();
		tf = gameObject.transform;
		held = false;

		for (int i = 0; i < indicators.Length; i++) {
			indicators [i].GetComponent<MeshRenderer> ().enabled = false;
		}

        controller = GameManager.gameController;

        currentTime = Time.time;

        //controller.AddHandler("TRIGGER", (string s) => HandleBallHit(s));
    }

    public float CalcPower(long time)
    {
        return mult * (Mathf.Exp(exp * time)) + offset;
    }
    
    public void HandleBallHit(string message)
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
			hitStart = Time.time;
            //cameraman.gameObject.SetActive(false);

            //controller.TryReset();
        }
    }

	// Update is called once per frame
	void Update () {
		currentTime = Time.time;
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
					//Debug.Log ("Checking");
					//Debug.Log ("Start Time: " + hitStart);
					//Debug.Log("Current Time: " + currentTime);
					if (currentTime - hitStart > 3) {
						//Debug.Log ("Got it");
						hitStart = float.MaxValue;
						ballState = BallState.STOPPED;
						cameraman.gameObject.SetActive (false);
						GameManager.instance.EndTurn ();
					}
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
				hitStart = currentTime;
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


    private void OnTriggerEnter(Collider other)
    {
		if (other.CompareTag ("Finish")) {
			isFinished = true;
            score++;
            cumulativeScore++;
		} else if (other.CompareTag ("Spider")) {
			gameObject.transform.SetPositionAndRotation (GameObject.FindGameObjectWithTag ("Start").transform.position, gameObject.transform.rotation);
			rb.velocity = new Vector3 (0, 0, 0);
			hitStart = float.MaxValue;
			ballState = BallState.STOPPED;
			cameraman.gameObject.SetActive (false);
			GameManager.instance.EndTurn ();
		} else if (other.CompareTag ("Cow")) {
			Destroy (other.gameObject);
		}
    }
}
