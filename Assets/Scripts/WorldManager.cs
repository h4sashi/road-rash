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

    void Start()
    {

    }

    void FixedUpdate()
    {
        if (GameStateManager.Singleton.isGameOver)
        {
            worldSpeed = Mathf.Lerp(worldSpeed, 0, 1.5f * Time.fixedDeltaTime);
            carsSpeed = Mathf.Lerp(carsSpeed, 0, 1.5f * Time.fixedDeltaTime);
        }
        else
        {
            multiplier = Mathf.Clamp(GameStateManager.Singleton.getScore / 1000f, 1, 3.5f);
            Difficulty = multiplier;
            worldSpeed = scrollSpeed * multiplier;
            carsSpeed = scrollSpeed * carScrollSpeedRatio * multiplier;
        }

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
}
