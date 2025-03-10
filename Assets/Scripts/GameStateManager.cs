using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Singleton { get; private set; }
    public static Action OnGameOver;
    Player player;

    int chances = 3;//Health
    int score;

    [DllImport("__Internal")]
    private static extern void Vibrate(int ms);

    void Awake()
    {
        Singleton = this;
    }

    void Start()
    {
        player = FindObjectOfType<Player>();
        ResetGame();
    }

    void Update()
    {

    }

    public void ResetGame()
    {
        isGameOver = false;
        ResumeGame();
        score = 0;
        chances = 3;
        OnGameOver = null;
    }

    public void Warn()
    {
        chances--;
        player.UpdatePlayerIcon();
#if UNITY_EDITOR
#else
        Vibrate(1000);
#endif
        if (chances == 0)
        {
            GameOver();
        }
    }

    public bool isGameOver { get; private set; }
    private void GameOver()
    {
        OnGameOver?.Invoke();
        isGameOver = true;
        LeaderboardManager.Singleton.SubmitScore(score);
        Invoke(nameof(ShowGameOverScreen), FindObjectOfType<WorldManager>().brakeTime + 0.75f);
    }
    void ShowGameOverScreen()
    {
        GameplayUIManager.Singleton.ShowScreen(Screens.GameOver);
    }

    public void AddScore(int _score)
    {
        score += _score;
    }

    public int getScore => score;
    public int getChancesLeft => chances;

    public void Reload()
    {
        ResetGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Home()
    {
        SceneManager.LoadScene("Profiler");
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
}