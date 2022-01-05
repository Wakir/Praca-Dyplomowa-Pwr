using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Default,
    MainMenu,
    Preparing,
    Playing,
    Gameover
}

public delegate void OnStateChangeHandler();
public class GameManager : MonoBehaviour
{
    public event OnStateChangeHandler OnStateChange;

    private GameState gameState;

    public GameState GameState
    {
        get
        {
            return gameState;
        }

        set
        {
            gameState = value;
            if (OnStateChange != null) OnStateChange();
        }
    }

    public static GameManager MainManager{get; private set;}

    private void Awake() {
        if (MainManager == null)
        {
            MainManager = this;
            DontDestroyOnLoad(gameObject);
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit() {
        MainManager = null;
    }

    public void LoadLevel(string levelName, GameState newState)
    {
        StartCoroutine(LoadLevelAsync(levelName, newState));
    }

    private IEnumerator LoadLevelAsync(string levelName, GameState newState)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelName);
        while(operation.isDone == false)
        {
            yield return null;
        }

        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);

        GameState = newState;
    }
}
