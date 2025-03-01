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

    private int[] lastOnlane = { 0, 0, 0 };
    struct CarStruct
    {
        public Transform transform;
        public float height;
        public int spriteIndex;
        public SpriteRenderer renderer;
    }
    CarStruct lastSpawnedCar;
    List<CarStruct> Cars;
    List<CarStruct> LastmaCars;

    Player player;

    void Start()
    {
        player = GameObject.FindObjectOfType<Player>();

        currentSpawnLane = 1;

        GameStateManager.OnGameOver += SpawnLASTMA;
        WeatherManager.OnWeatherChange += UpdateCarSpritesOnWeatherChange;

        vehicles = NightVehicles.Concat(DayVehicles).ToArray();
        Cars = new List<CarStruct>();
        LastmaCars = new List<CarStruct>();
        Spawn();
    }

    void Update()
    {
        if (25 - lastSpawnedCar.transform.position.y >= (CarSpacing + lastHeight))
        {
            Spawn();
        }
    }

    void Spawn()
    {
        SpawnCar(0);
        SpawnCar(1);
        UpdateSpawnLane();
    }

    int lastLane = 0;
    float lastHeight;
    private void SpawnCar(int turn)
    {
        int day_night = WeatherManager.GetDayNight();
        int index = Random.Range(0, vehicles.Length / 2);

        var carSprite = vehicles[day_night * vehicles.Length / 2 + index];

        GameObject car = new GameObject(carSprite.name);

        var spriteRenderer = car.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = carSprite;

        int lane = currentSpawnLane;
        if (turn > 0)
        {
            while (lane == lastLane)
            {
                lane = Random.Range(0, 3);
            }
            lastLane += lane;
        }
        else
        {
            lastLane = lane;
        }

        car.transform.position = new Vector3(lanes[lane], 25 + lastHeight);

        var collider = car.AddComponent<BoxCollider2D>();
        collider.size = spriteRenderer.bounds.size;

        if (turn > 0)
        {
            float sidebysideoffset = lastSpawnedCar.height - collider.size.y;
            car.transform.position += Vector3.up * 0.5f * -sidebysideoffset;
        }

        if (turn > 0)
        {
            lastHeight = Mathf.Max(lastSpawnedCar.height, collider.size.y);
        }

        lastSpawnedCar.renderer = spriteRenderer;
        lastSpawnedCar.transform = car.transform;
        lastSpawnedCar.height = spriteRenderer.size.y;
        lastSpawnedCar.spriteIndex = index;

        lastOnlane[lane] = (int)spriteRenderer.size.y;

        Cars.Add(lastSpawnedCar);
    }

    private void SpawnLASTMA()
    {
        Vector3 SpawnPoint = player.transform.position + Vector3.up * (WorldManager.worldSpeed * 0.75f);

        GameObject car = new GameObject("LASTMA");
        car.transform.position = SpawnPoint;
        var spriteRenderer = car.AddComponent<SpriteRenderer>();

        int index = WeatherManager.GetDayNight();
        spriteRenderer.sprite = LASTMAVehicles[index];

        var overlapped = Cars.Where(x => Mathf.Abs(x.transform.position.x - car.transform.position.x) < 1).ToList();
        overlapped.ForEach(x => x.transform.gameObject.SetActive(false));

        CarStruct carS = new CarStruct
        {
            renderer = spriteRenderer,
            transform = car.transform,
            height = spriteRenderer.size.y,
            spriteIndex = index,
        };
        LastmaCars.Add(carS);
    }

    private void UpdateSpawnLane()
    {
        // int lane = currentSpawnLane;
        // while (lane == currentSpawnLane)
        // {
        //     lane = Random.Range(0, 3);
        // }
        currentSpawnLane = 3 - lastLane;
    }

    public void ScrollCars(float speed)
    {
        foreach (var car in Cars)
        {
            car.transform.position += Vector3.down * speed * Time.fixedDeltaTime;
        }
        foreach (var car in LastmaCars)
        {
            car.transform.position += Vector3.down * WorldManager.worldSpeed * Time.fixedDeltaTime;
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
