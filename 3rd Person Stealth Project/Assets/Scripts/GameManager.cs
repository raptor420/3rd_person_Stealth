using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    bool GameOver;
    public static GameManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
           // DontDestroyOnLoad(this);
        }
        GameOver = false;
        Surveilance.PlayerFound += GameIsOver;

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void GameIsOver()
    {

        if (!GameOver)
        {
            Debug.Log("Game's Over");
            GameOver = true;
            Invoke("Restartlevel", .5f);
            Surveilance.PlayerFound -= GameIsOver;

        }
    } 
    public void LevelComplete()
    {
        if (!GameOver)
        {

            Debug.Log("WIN");
            Surveilance.PlayerFound -= GameIsOver;
            LoadNextLevel();
        }
    }

    void Restartlevel()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    void LoadNextLevel()
    {

        if(SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings-1)
        {
            Debug.Log("gg");
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);

        }

    }
}
