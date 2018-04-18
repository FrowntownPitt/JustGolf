using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Arduino;

public class BallController : MonoBehaviour {

    [SerializeField]
    Communication controller; // Use the global arduino controller script
    bool useController; // Keep track of whether the game is using the USB controller

	Rigidbody rb;
	Transform tf;

	public Transform cameraman;  // Camera which follows the player
	float tm;   // How long the user has held the fire key (keyboard controls)
	bool held;  // Track when the user holds the fire key (keyboard controls)

    public int score = 0;               // player's level score
    public int cumulativeScore = 0;     // Track player's persistent score

	public GameObject[] indicators;     // Show how long the user has pressed the fire key
	public float MaxSpeed;  // Max speed the player can travel (keyboard controls)

    // Arduino controller's exponential function (time to speed) [m*e^(j*x) + k]
    public float mult = 118.768f;
    public float exp = -0.00879889f;
    public float offset = 10;   // Minimum speed offset

    private bool isTurn = false; // Whether it is this player's turn

    public bool isFinished = false; // Whether this player has reached the hole

    private enum BallState // State machine of the turn
    {
        WAITTOSTART, // Waiting for input
        MOVING,      // In the process of moving
        STOPPED      // Stopped moving
    }

    // Reset at the beginning of the game
    public void ResetPlayer()
    {
        score = 0;
        cumulativeScore = 0;

        isTurn = false;
        isFinished = false;

        ballState = BallState.WAITTOSTART;
    }

    private BallState ballState = BallState.WAITTOSTART; // State machine

	public float hitStart = 0;
    public float currentTime;// = Time.time;

    // Enable this player's turn
    public void StartTurn()
    {
        isTurn = true;
        ballState = BallState.WAITTOSTART; // Init state machine

        controller.TryReset(); // Reset the controller (replace the ball)

        cameraman.gameObject.SetActive(true); // Enable this player's camera
    }

    // Stop this player's turn
    public void EndTurn()
    {
        isTurn = false;
        ballState = BallState.WAITTOSTART;

        // disable this player's camera
        cameraman.gameObject.SetActive(false);
    }
    
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

    // Do the exponential time-to-power function
    // time: "<time in ms>"
    public float CalcPower(long time)
    {
        return mult * (Mathf.Exp(exp * time)) + offset;
    }
    
    // What to do when the controller says the ball has been hit
    // message: "TRIGGER <time in us>"
    public void HandleBallHit(string message)
    {
        // Get the time from the message
        string[] m = message.Split();
        string t = m[1];
        long dur;
        bool valid = long.TryParse(t, out dur);

        // If the message is valid and the ball hasn't started moving
        if (valid && ballState == BallState.WAITTOSTART)
        {
            // Calculate the speed the user hit the ball
            float speed = MaxSpeed * CalcPower(dur / 1000);
            Debug.Log("Triggered: " + dur);
            Debug.Log("Power: " + speed);

            if (useController) {
                // Move at the speed the user hit, in the direction they hit
                rb.AddForce(speed * cameraman.forward);
            }


            ballState = BallState.MOVING; // Say the ball is moving now

			hitStart = Time.time; // Track the time the user has hit the ball
        }
    }
    
	void Update () {
		currentTime = Time.time;
        if (isTurn)
        {
            useController = controller.IsConnected();

            if (useController)
            {
                // In case the controller is not reset, allow the user to press space to reset it
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    controller.TryReset();
                }
            }

            if(ballState == BallState.MOVING)
            {
                // once the ball has stopped moving, track that
                if(rb.velocity == Vector3.zero)
                {
                    // Only allow after the ball has been moving for 3 seconds
                    // (traps a bug)
					if (currentTime - hitStart > 3)
                    {
                        // Track the number of attempts the user has made
                        score++;
                        cumulativeScore++;

                        hitStart = float.MaxValue;
						ballState = BallState.STOPPED;
						cameraman.gameObject.SetActive (false); // Disable the player's camera
						GameManager.instance.EndTurn (); // End the current player's turn
					}
                }
            }

            // If the user wants to use the keyboard, track the "w" key pressed for power
            if (Input.GetKeyDown(KeyCode.W) && ballState == BallState.WAITTOSTART)
            {
                tm = 0;
                held = true;
                indicators[0].GetComponent<MeshRenderer>().enabled = true;
            }

            if (held && tm < MaxSpeed)
            {
                tm += 0.1f; // track the time the user holds 'w'
            }

            // Indicate the current power the player has set
            for (int i = 0; i < indicators.Length; i++)
            {
                if (tm > MaxSpeed / (MaxSpeed / (i + 1)))
                    indicators[i].GetComponent<MeshRenderer>().enabled = true;
            }

            // When the player releases the 'w' key, hit the ball
            if (Input.GetKeyUp(KeyCode.W) && ballState == BallState.WAITTOSTART && held)
            {
				hitStart = currentTime;
                rb.AddForce(500 * tm * cameraman.forward);
                tm = 0;
                held = false;
                // disable the power indicators
                for (int i = 0; i < indicators.Length; i++)
                {
                    indicators[i].GetComponent<MeshRenderer>().enabled = false;
                }

                ballState = BallState.MOVING;
            }

            // On 'f', stop the velocity (for debugging/testing)
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

    // When the ball reaches the end, or interacts with the level components
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
