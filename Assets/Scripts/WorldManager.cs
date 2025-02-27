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

    void Start()
    {

    }

    void Update()
    {
        AssignRoadsByWeather();
        ScrollRoad();
        trafficManager.ScrollCars(scrollSpeed * carScrollSpeedRatio);
    }

    void ScrollRoad()
    {
        foreach (var road in roads)
        {
            road.transform.position += Vector3.down * scrollSpeed * Time.deltaTime;
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
