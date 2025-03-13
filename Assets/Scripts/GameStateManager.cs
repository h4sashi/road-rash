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
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

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
        if(Settings.hapticOn) Vibrate(1000);
#endif
        if (chances == 0)
        {
            SoundManager.Instance?.Play("GameOverSound");
            GameOver();
        }
    }

    public bool isGameOver { get; private set; }
    private void GameOver()
    {
        OnGameOver?.Invoke();
        isGameOver = true;

        TutorialManager.Singleton.TriggerTutorial(3);

        LeaderboardManager.Singleton.SubmitScore(score);
        Invoke(nameof(ShowGameOverScreen), FindObjectOfType<WorldManager>().brakeTime + 0.75f);

        if (Time.timeScale < 1) Time.timeScale = 1;
    }
    void ShowGameOverScreen()
    {
        GameplayUIManager.Singleton.ShowScreen(Screens.GameOver);
    }

    public void AddScore(int _score)
    {
        var addition = Mathf.Min(100, _score * Mathf.Max(1, (int)Time.timeSinceLevelLoad / 5));
        score += addition;
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