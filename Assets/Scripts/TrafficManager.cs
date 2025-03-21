using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    [SerializeField] private Sprite[] DayVehicles;
    [SerializeField] private Sprite[] NightVehicles;
    [SerializeField] private Sprite[] LASTMAVehicles;
    [SerializeField] private int CarSpacing = 5;
    [SerializeField] private Sprite[] Obstacles;

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
    List<CarStruct> Obs;
    List<CarStruct> LastmaCars;

    int canSpawnObs;

    Player player;

    void Start()
    {
        player = GameObject.FindObjectOfType<Player>();

        currentSpawnLane = 1;
        canSpawnObs = 3;

        GameStateManager.OnGameOver += SpawnLASTMA;
        WeatherManager.OnWeatherChange += UpdateCarSpritesOnWeatherChange;

        vehicles = NightVehicles.Concat(DayVehicles).ToArray();
        Cars = new List<CarStruct>();
        Obs = new List<CarStruct>();
        LastmaCars = new List<CarStruct>();

        Spawn();
    }

    void Update()
    {
        if (25 - lastSpawnedCar.transform.position.y >= (CarSpacing + lastHeight))
        {
            Spawn();
        }

        if (Cars.Count > 10)
        {
            for (int i = 0; i < 6; i++)
            {
                Destroy(Cars[i].transform.gameObject);
            }
            Cars.RemoveRange(0, 6);
        }
        if (Obs.Count > 10)
        {
            for (int i = 0; i < 6; i++)
            {
                Destroy(Obs[i].transform.gameObject);
            }
            Obs.RemoveRange(0, 6);
        }
    }

    void Spawn()
    {
        SpawnCar(0);
        SpawnCar(1);
        UpdateSpawnLane();
        canSpawnObs--;
    }

    int lastLane = 0;
    float lastHeight;
    bool wasStatic;
    Vector3 staticSupposedPosition;

    private void SpawnCar(int turn)
    {
        if (GameStateManager.Singleton.isGameOver) return;

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

        bool isStatic = ((Random.Range(0, 1000) % 3) == 0) && (lane != 1) && canSpawnObs <= 0;

        int day_night = WeatherManager.GetDayNight();
        int index = Random.Range(0, vehicles.Length / 2);

        Sprite carSprite;// = vehicles[day_night * vehicles.Length / 2 + index];
        if (isStatic)
        {
            carSprite = Obstacles[Random.Range(0, Obstacles.Length)];
        }
        else
        {
            carSprite = vehicles[day_night * vehicles.Length / 2 + index];
        }

        GameObject car = new GameObject(carSprite.name);
        var spriteRenderer = car.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = carSprite;

        car.transform.position = new Vector3(lanes[lane], 25 + (lastHeight + spriteRenderer.bounds.size.y) / 2);
        car.transform.localScale = new Vector3(Mathf.Sign(lanes[lane]), 1, 1);

        var collider = car.AddComponent<BoxCollider2D>();
        collider.size = spriteRenderer.bounds.size;

        if (turn > 0)
        {
            if (wasStatic)
            {
                car.transform.position += Vector3.up * (staticSupposedPosition.y - car.transform.position.y);
            }
            else
            {
                car.transform.position += Vector3.up * (lastSpawnedCar.transform.position.y - car.transform.position.y);
            }

            float sidebysideoffset = lastSpawnedCar.height - collider.size.y;
            car.transform.position += Vector3.up * (0.5f * -sidebysideoffset);
        }

        if (turn > 0)
        {
            lastHeight = Mathf.Max(lastSpawnedCar.height, collider.size.y);
        }

        if (isStatic)
        {
            staticSupposedPosition = car.transform.position;
            var distanceToPlayer = Mathf.Abs(car.transform.position.y - (spriteRenderer.bounds.size.y * 0.5f) - player.getRayPos.y);
            var time = distanceToPlayer / WorldManager.carsSpeed;

            var preferredDist = WorldManager.worldSpeed * time;
            car.transform.position += Vector3.up * (preferredDist - distanceToPlayer);

            car.tag = "static";
        }

        lastSpawnedCar.renderer = spriteRenderer;
        lastSpawnedCar.transform = car.transform;
        lastSpawnedCar.height = spriteRenderer.size.y;
        lastSpawnedCar.spriteIndex = index;

        lastOnlane[lane] = (int)spriteRenderer.size.y;

        if (isStatic) Obs.Add(lastSpawnedCar);
        else Cars.Add(lastSpawnedCar);

        wasStatic = isStatic;
    }

    private void SpawnLASTMA()
    {
        var worldM = FindObjectOfType<WorldManager>();

        var sides = new int[2] { -4, 4 };
        int side = sides.Where(x => Mathf.Abs(x - player.transform.position.x) <= 4)
        .OrderBy(x => Random.value)
        .First();

        float stopDeccel = WorldManager.worldSpeed / worldM.brakeTime;
        float dist = Mathf.Pow(WorldManager.worldSpeed, 2) / (2 * stopDeccel) + 7.5f;

        Vector3 SpawnPoint = player.transform.position + Vector3.up * dist;
        SpawnPoint.x = side;

        GameObject car = new GameObject("LASTMA");
        car.transform.position = SpawnPoint;
        var spriteRenderer = car.AddComponent<SpriteRenderer>();

        int index = WeatherManager.GetDayNight();
        spriteRenderer.sprite = LASTMAVehicles[index];

        var overlapped = Cars.Where(x => Mathf.Abs(x.transform.position.x - side) < 1).ToList();
        overlapped.ForEach(x => x.transform.gameObject.SetActive(false));

        CarStruct carS = new CarStruct
        {
            renderer = spriteRenderer,
            transform = car.transform,
            height = spriteRenderer.size.y,
            spriteIndex = index,
        };
        LastmaCars.Add(carS);

        player.MoveToLane(side);
    }

    public void SpawnObstacle()
    {

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
            //car.transform.position += Vector3.down * WorldManager.worldSpeed * Time.fixedDeltaTime;
        }

        foreach (var car in LastmaCars)
        {
            car.transform.position += Vector3.down * WorldManager.worldSpeed * Time.fixedDeltaTime;
        }

        foreach (var obs in Obs)
        {
            obs.transform.position += Vector3.down * WorldManager.worldSpeed * Time.fixedDeltaTime;
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
