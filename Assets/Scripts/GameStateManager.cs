using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Singleton { get; private set; }
    public static Action OnGameOver;

    int chances = 0;

    void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Singleton != this)
        {
            DestroyImmediate(gameObject);
        }
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
        chances = 3;
        OnGameOver = null;
    }

    public void Warn()
    {
        chances--;
        if (chances == 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        OnGameOver.Invoke();
    }
}
