using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    [SerializeField] private Sprite[] DayVehicles;
    [SerializeField] private Sprite[] NightVehicles;
    [SerializeField] private Sprite[] LASTMAVehicles;
    [SerializeField] private int CarSpacing = 5;

    private int[] lanes = { -4, 0, 4 };
    private int currentSpawnLane;
    Sprite[] vehicles;

    struct CarStruct
    {
        public Transform transform;
        public float height;
        public int spriteIndex;
        public SpriteRenderer renderer;
    }
    CarStruct lastSpawnedCar;
    List<CarStruct> Cars;
    void Start()
    {
        WeatherManager.OnWeatherChange += UpdateCarSpritesOnWeatherChange;
        vehicles = NightVehicles.Concat(DayVehicles).ToArray();
        Cars = new List<CarStruct>();
        SpawnCar();
    }

    void Update()
    {
        if (25 - lastSpawnedCar.transform.position.y >= (CarSpacing + lastSpawnedCar.height))
        {
            SpawnCar();
            SpawnCar();
        }
    }


    private void SpawnCar()
    {
        int day_night = WeatherManager.GetDayNight();
        int index = Random.Range(0, vehicles.Length / 2);

        var carSprite = vehicles[day_night * vehicles.Length / 2 + index];

        GameObject car = new GameObject(carSprite.name);

        var spriteRenderer = car.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = carSprite;

        car.transform.position = new Vector3(lanes[currentSpawnLane], 25);

        var collider = car.AddComponent<BoxCollider2D>();
        collider.size = spriteRenderer.bounds.size;

        lastSpawnedCar.renderer = spriteRenderer;
        lastSpawnedCar.transform = car.transform;
        lastSpawnedCar.height = spriteRenderer.size.y;
        lastSpawnedCar.spriteIndex = index;

        Cars.Add(lastSpawnedCar);
        UpdateSpawnLane();
    }

    private void SpawnLASTMA()
    {

    }

    private void UpdateSpawnLane()
    {
        int lane = currentSpawnLane;
        while (lane == currentSpawnLane)
        {
            lane = Random.Range(0, 3);
        }
        currentSpawnLane = lane;
    }

    public void ScrollCars(float speed)
    {
        foreach (var car in Cars)
        {
            car.transform.position += Vector3.down * speed * Time.deltaTime;
        }
    }

    private void UpdateCarSpritesOnWeatherChange(WeatherManager.WeatherType weather)
    {
        foreach (var car in Cars)
        {
            car.renderer.sprite = vehicles[car.spriteIndex + (int)weather * vehicles.Length / 2];
        }
    }
}
