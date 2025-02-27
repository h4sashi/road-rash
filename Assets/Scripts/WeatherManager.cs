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

    public static Action<WeatherType> OnWeatherChange;

    void Start()
    {

    }

    void Update()
    {
        lastweather = weather;
        weather = (int)weatherType;
        if (weather != lastweather) OnWeatherChange?.Invoke(weatherType);
    }

    public static int GetDayNight()
    {
        return weather;
    }
}
