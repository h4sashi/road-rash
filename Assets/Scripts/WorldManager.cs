using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private TrafficManager trafficManager;
    [SerializeField] private WeatherManager weatherManager;

    [SerializeField] private Sprite[] RoadSprites;

    [SerializeField] private SpriteRenderer[] roads;

    [SerializeField] float scrollSpeed = 9f;
    [SerializeField] float carScrollSpeedRatio = 0.1f;
    float multiplier = 1;

    public static float Difficulty;

    public static float worldSpeed;
    public static float carsSpeed;

    float stopTimer = 0;
    public float brakeTime;
    public float deccelRate = 7.5f;

    float endWorldSpeed;
    float endCarSpeed;

    void Start()
    {
        stopTimer = 0;
    }

    void FixedUpdate()
    {
        if (GameStateManager.Singleton.isGameOver)
        {
            worldSpeed -= deccelRate * Time.fixedDeltaTime;
            worldSpeed = Mathf.Clamp(worldSpeed, 0, worldSpeed);
        }
        else
        {
            multiplier = Mathf.Clamp(GameStateManager.Singleton.getScore / 1000f, 1, 3.5f);
            Difficulty = multiplier;
            worldSpeed = scrollSpeed * multiplier;
        }
        carsSpeed = worldSpeed * carScrollSpeedRatio;

        GameStateManager.OnGameOver += () =>
        {
            deccelRate = worldSpeed / brakeTime;
            endCarSpeed = carsSpeed;
            endWorldSpeed = worldSpeed;
        };

        AssignRoadsByWeather();
        ScrollRoad(worldSpeed);
        trafficManager.ScrollCars(worldSpeed * carScrollSpeedRatio);
    }

    void ScrollRoad(float speed)
    {
        foreach (var road in roads)
        {
            road.transform.position += Vector3.down * worldSpeed * Time.fixedDeltaTime;
            if (road.transform.position.y < -30)
            {
                road.transform.position += Vector3.up * roads.Length * 26.3f;
            }
        }
    }

    void AssignRoadsByWeather()
    {
        var dayNight = WeatherManager.GetDayNight();
        foreach (var road in roads)
        {
            road.sprite = RoadSprites[dayNight];
        }
    }

    public float getStopTime
    {
        get { return worldSpeed / deccelRate; }
    }
}
