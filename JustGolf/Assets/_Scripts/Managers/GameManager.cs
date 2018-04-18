using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    private int level = 1;                                  //Current level number, expressed in game as "Day 1".

    public Camera camera;       // Camera of the scene, disable on switch to a player's camera

    private int currentActivePlayer = 0;            // Round robin, track the current global player
    private int currentPlayer = 0;                  // Round robin, track the current remaining player
    public List<BallController> Players;            // Keep track of all of the players
    public List<BallController> RemainingPlayers;   // Track players which have not reached the end 

    [SerializeField]
    private bool returnToMain; // Was this level played standalone, or part of a series?

    public List<string> Levels; // Define what the level names are, in order
    
    [SerializeField]
    public static Arduino.Communication gameController;     // Arduino communication handler
    public string controllerPort = "COM5";                  // Which COM should we ask for?

    public GameObject playerNotification;   // A "toast" to whichever player's turn it is
    public Text playerText;     // Text to display on the toast

    void Awake()
    {
        // Begin singleton pattern
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
        // end singleton pattern

        // Make the players persistent
        for (int i = 0; i < Players.Count; i++)
        {
            DontDestroyOnLoad(Players[i].gameObject.transform.root);
            Players[i].score = 0;
        }

        // Initialize our handler
        gameController = GetComponent<Arduino.Communication>();

        // Do level initialization
        InitLevel();
    }

    // UNUSED: Disable players from moving
    public void DisablePlaying()
    {
        for(int i=0; i<Players.Count; i++)
        {
            Players[i].EndTurn();
        }
    }

    // UNUSED: Allow the players to move
    public void EnablePlaying()
    {

    }

    // Prime the system for the first level
    public void StartGame()
    {
        level = 1;
        //StartLevel(level);
    }

    // UNUSED: Reset the game
    public void ResetGame()
    {
        // Reset player data
        for(int i=0; i<Players.Count; i++)
        {
            Players[i].ResetPlayer();
        }
    }

    // Level initialization
    public void InitLevel()
    {
        // Copy Players to RemainingPlayers
        RemainingPlayers = new List<BallController>(Players.Count);
        for (int i = 0; i < Players.Count; i++)
        {
            RemainingPlayers.Insert(i, Players[i]);
            Players[i].isFinished = false;  // Each player hasn't finished the new level yet
            Players[i].score = 0;           // Each player's level score is reset
            //DontDestroyOnLoad(Players[i].gameObject.transform.root);
        }

        // Start on the first player
        currentActivePlayer = 0;
        currentPlayer = 0;

        // Let the first player start
        StartNextTurn();
    }

    // Go through each player and place them on that player's location in the next level
    public void PlacePlayers(List<GameObject> locations)
    {
        for(int i=0; i<Players.Count; i++)
        {
            Transform t = Players[i].gameObject.transform.root;
            Transform s = locations[i].transform;
            t.position = s.position;
            t.rotation = s.rotation;
            t.localScale = s.localScale;

            t = t.GetChild(0);
            s = s.GetChild(0);
            t.position = s.position;
            t.rotation = s.rotation;
            t.localScale = s.localScale;

            t = t.root.GetChild(1);
            s = s.root.GetChild(1);
            t.position = s.position;
            t.rotation = s.rotation;
            t.localScale = s.localScale;

            // Destroy the placeholder player if there is already a persistent player
            if (s.root != Players[i].gameObject.transform.root) Destroy(locations[i]);
            else Debug.Log("First level!" + level);
        }

        //InitLevel();

        //StartNextTurn();
    }
    
    // Start the given level. Run in "trial mode" if single=true (play that level then return to main menu)
    public void StartLevel(int level, bool single=false)
    {
        returnToMain = single;

        // Just load the scene. Persistent objects will survive.
        SceneManager.LoadSceneAsync(Levels[level]); 
        //Application.LoadLevelAdditiveAsync(Levels[level]);
    }

	public void RestartLevel(){
		StartLevel (level);
	}

    private void Start()
    {
        camera.gameObject.SetActive(false);
        StartNextTurn(); // Start the first player
        //Players[currentPlayer].StartTurn();

        // Connect to the controller
        Debug.Log(gameController.TryConnect(controllerPort));

        // Add the global controller handler as a callback for the TRIGGER message
        gameController.AddHandler("TRIGGER", (string s) => HandleBallHit(s));

        //System.Action<string> H = (string s) => this.HandleBallHit(s);

        //H("Trigger 20000");

    }

    // The controller handler for when the ball is hit. Tell the current player the ball has been hit
    public void HandleBallHit(string message)
    {
        RemainingPlayers[currentPlayer].HandleBallHit(message);
        //Debug.Log("Triggered!!!!");
    }

    // Called when all players have finished the level.
    private void EndLevel()
    {
        if (returnToMain) // If we played this level in "Trial mode", return to the main menu
        {
            level = 0;
            StartLevel(level);
        }
        else
        {
            StartLevel(++level); // Otherwise, start the next level
        }
    }

    // End the current player's turn and begin the next
    // Called by the player once it has stopped moving
    public void EndTurn()
    {
        // When it's reached the hole, remove it from the active players
        if (RemainingPlayers[currentPlayer].isFinished)
        {
            RemainingPlayers.RemoveAt(currentPlayer);
            if (RemainingPlayers.Count == 0)
            {
                EndLevel(); // End the current level if all players have reached the hole
                return;
            }
            else
            {
                currentPlayer %= RemainingPlayers.Count;
            }
        }
        else {
            currentPlayer = (currentPlayer + 1) % RemainingPlayers.Count; // Go to the next remaining player
        }

        // Figure out what the next player is
        int cur = currentActivePlayer;
        for (int i = (cur+1)%Players.Count; i != cur; i = (i + 1) % Players.Count)
        {
            if (Players[i].isFinished) break;
            currentActivePlayer = (currentActivePlayer + 1) % Players.Count;
        }

        // Begin the next player's turn
        StartNextTurn();
        //Players[currentPlayer].StartTurn();
    }

    // Begin the turn of the next player
    private void StartNextTurn()
    {
        // Display the "Player x" toast
        playerText.text = "Player " + (currentActivePlayer+1);
        playerNotification.GetComponent<Animator>().Play("Fade Effect", -1, 0);

        // Start the turn of the current player (incremented before calling this method)
        RemainingPlayers[currentPlayer].StartTurn();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            level = (level + 1) % Levels.Count;
            Debug.Log("Loading next level (" + level + ")...");
            StartLevel(level);
            //Debug.Log("Is connected: " + gameController.TryConnect(controllerPort));
            //gameController.AddHandler("Trigger", (string s) => HandleBallHit(s));
        }
    }

}
