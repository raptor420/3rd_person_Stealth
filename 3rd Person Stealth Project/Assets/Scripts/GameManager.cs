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
      

        
    }
    private void OnEnable()
    {
        BaseClassSpotter.OnSpotPlayer += GameIsOver;
    }
    private void OnDisable()
    {
        BaseClassSpotter.OnSpotPlayer -= GameIsOver;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void GameIsOver()
    {
       StartCoroutine ( ProcessGameOver());
    }

    private IEnumerator ProcessGameOver()
    {
        if (!GameOver)
        {
            float delayTime =1.5f;
            Debug.Log("Game's Over");
            GameOver = true;
            yield return new  WaitForSeconds(delayTime);
        //    Invoke("Restartlevel", .5f);
            Restartlevel();

        }
    }

    public void LevelComplete()
    {
        if (!GameOver)
        {

            Debug.Log("WIN");
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
