using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    private int level = 1;                                  //Current level number, expressed in game as "Day 1".

    public Camera camera;

    private int currentActivePlayer = 0;
    private int currentPlayer = 0;
    public List<BallController> Players;
    public List<BallController> RemainingPlayers;


    public List<string> Levels;
    
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
        for (int i = 0; i < Players.Count; i++)
        {
            DontDestroyOnLoad(Players[i].gameObject.transform.root);
            Players[i].score = 0;
        }

        gameController = GetComponent<Arduino.Communication>();

        InitLevel();
        //gameController = new Arduino.Communication();
        //gameController.TryConnect(controllerPort);
    }

    public void DisablePlaying()
    {
        for(int i=0; i<Players.Count; i++)
        {
            Players[i].EndTurn();
        }
    }

    public void EnablePlaying()
    {

    }

    public void StartGame()
    {
        level = 1;
        //StartLevel(level);
    }

    public void ResetGame()
    {
        for(int i=0; i<Players.Count; i++)
        {
            Players[i].ResetPlayer();
        }
    }

    public void InitLevel()
    {
        RemainingPlayers = new List<BallController>(Players.Count);
        for (int i = 0; i < Players.Count; i++)
        {
            RemainingPlayers.Insert(i, Players[i]);
            Players[i].isFinished = false;
            Players[i].score = 0;
            //DontDestroyOnLoad(Players[i].gameObject.transform.root);
        }
        currentActivePlayer = 0;
        currentPlayer = 0;

        StartNextTurn();
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

            if (s.root != Players[i].gameObject.transform.root) Destroy(locations[i]);
            else Debug.Log("First level!" + level);
        }

        //InitLevel();

        //StartNextTurn();
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
        StartLevel(++level);
    }

    public void EndTurn()
    {
        if (RemainingPlayers[currentPlayer].isFinished)
        {
            RemainingPlayers.RemoveAt(currentPlayer);
            if (RemainingPlayers.Count == 0)
            {
                EndLevel();
                return;
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
            level = (level + 1) % Levels.Count;
            Debug.Log("Loading next level (" + level + ")...");
            StartLevel(level);
            //Debug.Log("Is connected: " + gameController.TryConnect(controllerPort));
            //gameController.AddHandler("Trigger", (string s) => HandleBallHit(s));
        }
    }

}
