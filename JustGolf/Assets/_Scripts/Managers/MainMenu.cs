using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public Camera sceneCamera;

    public GameManager gm;

    public string firstSceneName;

	// Use this for initialization
	void Start ()
    {
        
        //GameManager.instance.DisablePlaying();
        sceneCamera.gameObject.SetActive(true);

        //StartCoroutine(Wait());
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.P))
        {
            StartGame();
        }
	}

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(5f);
        StartGame();
    }

    //
    void StartGame()
    {
        //gm.enabled = true;
        //gm.gameObject.SetActive(true);
        sceneCamera.gameObject.SetActive(false);
        SceneManager.LoadSceneAsync(firstSceneName);
        //GameManager.instance.EnablePlaying();
        //StartCoroutine(WaitForGameManager());

        DontDestroyOnLoad(gameObject);
        WaitForGameManager();
    }

    IEnumerator WaitForGameManager()
    {
        while (GameManager.instance == null)
        {
            yield return null;
        }
        GameManager.instance.StartGame();
        //GameManager.instance.StartGame();
    }
}
