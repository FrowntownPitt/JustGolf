using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    private int level = 0;                                  //Current level number, expressed in game as "Day 1".

    public Camera camera;

    private int currentActivePlayer = 0;
    private int currentPlayer = 0;
    public List<BallController> Players;
    public List<BallController> RemainingPlayers;


    public List<string> Levels;
    public int currentLevel = 0;
    
    [SerializeField]
    public static Arduino.Communication gameController;
    public string controllerPort = "COM5";

    public GameObject playerNotification;
    public Text playerText;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);

        gameController = GetComponent<Arduino.Communication>();

        InitLevel();
        //gameController = new Arduino.Communication();
        //gameController.TryConnect(controllerPort);
    }

    private void InitLevel()
    {
        RemainingPlayers = new List<BallController>(Players.Count);
        for (int i = 0; i < Players.Count; i++)
        {
            RemainingPlayers.Insert(i, Players[i]);
            DontDestroyOnLoad(Players[i].gameObject.transform.root);
        }
    }

    public void PlacePlayers(List<GameObject> locations)
    {
        for(int i=0; i<Players.Count; i++)
        {
            Transform t = Players[i].gameObject.transform.root;
            Transform s = locations[i].transform;
            t.position = s.position;
            t.rotation = s.rotation;
            t.localScale = s.localScale;
            if (s.root != Players[i].gameObject.transform.root) Destroy(locations[i]);
            else Debug.Log("First level!");
        }

        InitLevel();
    }

    private void StartLevel(int level)
    {
        SceneManager.LoadSceneAsync(Levels[level]);
        //Application.LoadLevelAdditiveAsync(Levels[level]);
    }

    private void Start()
    {
        camera.gameObject.SetActive(false);
        StartNextTurn();
        //Players[currentPlayer].StartTurn();

        Debug.Log(gameController.TryConnect(controllerPort));

        gameController.AddHandler("TRIGGER", (string s) => HandleBallHit(s));

        //System.Action<string> H = (string s) => this.HandleBallHit(s);

        //H("Trigger 20000");

    }

    public void HandleBallHit(string message)
    {
        RemainingPlayers[currentPlayer].HandleBallHit(message);

        //Debug.Log("Triggered!!!!");
    }

    private void EndLevel()
    {
        StartLevel(++currentLevel);
    }

    public void EndTurn()
    {
        if (RemainingPlayers[currentPlayer].isFinished)
        {
            RemainingPlayers.RemoveAt(currentPlayer);
            if (RemainingPlayers.Count == 0)
            {
                EndLevel();
            }
            else
            {
                currentPlayer %= RemainingPlayers.Count;
            }
        }
        else {
            currentPlayer = (currentPlayer + 1) % RemainingPlayers.Count;
        }

        int cur = currentActivePlayer;
        for (int i = (cur+1)%Players.Count; i != cur; i = (i + 1) % Players.Count)
        {
            if (Players[i].isFinished) break;
            currentActivePlayer = (currentActivePlayer + 1) % Players.Count;
        }

        StartNextTurn();
        //Players[currentPlayer].StartTurn();
    }

    private void StartNextTurn()
    {
        playerText.text = "Player " + (currentActivePlayer+1);
        playerNotification.GetComponent<Animator>().Play("Fade Effect", -1, 0);

        RemainingPlayers[currentPlayer].StartTurn();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentLevel = (currentLevel + 1) % Levels.Count;
            Debug.Log("Loading next level (" + currentLevel + ")...");
            StartLevel(currentLevel);
            //Debug.Log("Is connected: " + gameController.TryConnect(controllerPort));
            //gameController.AddHandler("Trigger", (string s) => HandleBallHit(s));
        }
    }

}
