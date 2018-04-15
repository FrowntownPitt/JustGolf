using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    private int level = 0;                                  //Current level number, expressed in game as "Day 1".

    public Camera camera;

    private int currentPlayer = 0;
    public List<BallController> Players;
    
    [SerializeField]
    public static Arduino.Communication gameController;
    public string controllerPort = "COM5";

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);

        gameController = GetComponent<Arduino.Communication>();
        //gameController = new Arduino.Communication();
        //gameController.TryConnect(controllerPort);
    }

    private void Start()
    {
        camera.gameObject.SetActive(false);
        Players[currentPlayer].StartTurn();

        Debug.Log(gameController.TryConnect(controllerPort));

        gameController.AddHandler("TRIGGER", (string s) => HandleBallHit(s));

        //System.Action<string> H = (string s) => this.HandleBallHit(s);

        //H("Trigger 20000");

    }

    public void HandleBallHit(string message)
    {
        Players[currentPlayer].HandleBallHit(message);

        //Debug.Log("Triggered!!!!");
    }

    public void EndTurn()
    {
        currentPlayer = (currentPlayer + 1) % Players.Count;

        Players[currentPlayer].StartTurn();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Is connected: " + gameController.TryConnect(controllerPort));
            gameController.AddHandler("Trigger", (string s) => HandleBallHit(s));
        }
    }

}
