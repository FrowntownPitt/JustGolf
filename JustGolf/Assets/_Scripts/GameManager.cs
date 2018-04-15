using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    private int level = 0;                                  //Current level number, expressed in game as "Day 1".

    public Camera camera;

    private int currentPlayer = 0;
    public List<BallController> Players;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        camera.gameObject.SetActive(false);
        Players[currentPlayer].StartTurn();

    }

    public void EndTurn()
    {
        currentPlayer = (currentPlayer + 1) % Players.Count;

        Players[currentPlayer].StartTurn();
    }

}
