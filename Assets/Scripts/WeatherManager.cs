using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour
{
    public enum WeatherType
    {
        Night, Day
    }
    [SerializeField] private WeatherType weatherType;
    static int weather = 0;
    int lastweather;

    float weatherTimer = 0;

    public static Action<WeatherType> OnWeatherChange;

    void Awake()
    {
        OnWeatherChange = null;
    }

    void Start()
    {

    }

    void Update()
    {
        lastweather = weather;
        weather = (int)weatherType;
        if (weather != lastweather) OnWeatherChange?.Invoke(weatherType);

        if (!GameStateManager.Singleton.isGameOver)
        {
            weatherTimer += Time.deltaTime;
            if (weatherTimer >= UnityEngine.Random.Range(10f, 15f))
            {
                weatherType = (WeatherType)((weather + 1) % 2);
                weatherTimer = 0;
            }
        }
    }

    public static int GetDayNight()
    {
        return weather;
    }
}
