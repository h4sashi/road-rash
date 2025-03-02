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

    int chances = 0;
    int score;

    [DllImport("__Internal")]
    private static extern void Vibrate(int ms);

    void Awake()
    {
        Singleton = this;
    }

    void Start()
    {
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
        Invoke(nameof(ShowGameOverScreen), 3.1f);
    }
    void ShowGameOverScreen()
    {
        GameplayUIManager.Singleton.ShowScreen(Screens.GameOver);

        if (score > LeaderboardManager.Singleton.previousHighScore)
        {
            LeaderboardManager.Singleton.SubmitScore(score);
        }

    }

    public void AddScore(int _score)
    {
        score += _score;
    }

    public int getScore => score;

    public void Reload()
    {
        ResetGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Home()
    {
        SceneManager.LoadScene(0);
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
