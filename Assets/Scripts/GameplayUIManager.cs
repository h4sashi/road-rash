using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using TMPro;

public enum Screens
{
    GameOver,
    ScoreBoard,
    Pause,
    Main,
    Tutorial,
    Help,
}

public class GameplayUIManager : MonoBehaviour
{
    public static GameplayUIManager Singleton { get; private set; }
    UIScreen[] uIScreens;

    [SerializeField] TextMeshProUGUI[] scoreText;

    void Awake()
    {
        Singleton = this;
    }

    void Start()
    {
        uIScreens = FindObjectsOfType<UIScreen>(true);
        foreach (var screen in uIScreens) screen.Init();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var x in scoreText) x.text = $"{GameStateManager.Singleton.getScore:n0}";
    }

    public void ShowScreen(Screens screenType)
    {
        foreach (var screen in uIScreens) { screen.gameObject.SetActive(false); }

        var array = uIScreens.Where(s => s.screenType == screenType).ToList();
        array.ForEach(x => x.gameObject.SetActive(true));
    }

    public void ShowScreen(string screenName)
    {
        foreach (var screen in uIScreens) { screen.gameObject.SetActive(false); }

        var array = uIScreens.Where(s => screenName.Equals(s.screenName)).ToList();
        array.ForEach(x => x.gameObject.SetActive(true));
    }
}
