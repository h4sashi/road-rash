using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    int freeLane;
    int targetLane;
    int[] lanes = { -4, 0, 4 };
    int[] lanesReverse = { 4, 0, -4 };

    float hitDistance;

    [SerializeField] Transform rayPoint;
    [SerializeField] float rayLength = 10f;

    [SerializeField] Sprite[] carSprites;
    [SerializeField] Sprite[] playerIconSprites;
    SpriteRenderer spriteRenderer;
    [SerializeField] Image playerIcon;

    Sprite[,] spriteArray;

    Transform HitCar, lastHitCar;

    // Start is called before the first frame update
    void Start()
    {
        spriteArray = new Sprite[carSprites.Length / 2, 2];
        for (int i = 0; i < carSprites.Length; i++)
        {
            int y = i % 2;
            int x = i / 2;
            spriteArray[x, y] = carSprites[i];
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdatePlayerSprite(WeatherManager.GetDayNight());

        WeatherManager.OnWeatherChange += (time) =>
        {
            UpdatePlayerSprite((int)time);
        };

        targetLane = 1;

        UpdatePlayerIcon();

        GameStateManager.OnGameOver += () =>
        {
            transform.rotation = Quaternion.identity;
        };
    }


    void Update()
    {
        if (!GameStateManager.Singleton.isGameOver && Time.timeScale > 0)
        {
            Time.timeScale = 1f;// Reset time scale
            if (HitCar && HitCar.CompareTag("static") && isBlocking)
            {
                Time.timeScale = WorldManager.speedRatio;// slow down the game
                Debug.Log("slowingDown");
            }
        }
    }
    bool isBlocking;

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (GameStateManager.Singleton.isGameOver) return;

        transform.position = Vector3.MoveTowards(transform.position, new Vector3(lanes[targetLane], transform.position.y, transform.position.z), 20 * Time.fixedDeltaTime);
        float orientation = (lanes[targetLane] - transform.position.x) / 4 * -30f;

        transform.rotation = Quaternion.Euler(0, 0, orientation);

        if (isBlocking = Blocked(out int _freelane, out hitDistance))
        {
            freeLane = _freelane;
        }
    }

    // void FixedUpdate()
    // {

    // }

    void LateUpdate()
    {

    }

    public void SwitchLane()
    {
        targetLane = freeLane;
    }

    public void MoveToLane(int xPos)
    {
        targetLane = lanes.ToList().IndexOf(xPos);
    }

    public float GetHitDistanceRatio()
    {
        //return hitDistance / (rayLength * WorldManager.Difficulty);
        return hitDistance / rayLength;
    }

    bool Blocked(out int freelane, out float distance)
    {
        bool blocked = false;
        freelane = 0;
        distance = float.MaxValue;

        RaycastHit2D hit2D;
        lastHitCar = HitCar;

        if ((hit2D = Physics2D.Raycast(new Vector2(lanes[targetLane], rayPoint.position.y), Vector2.up, rayLength)).transform)
        {
            var _lanes = lanes;

            if (targetLane > 1) _lanes = lanesReverse;
            else if (targetLane == 1) { _lanes = (Random.Range(0, 10) % 2 == 0) ? lanesReverse : _lanes; }

            foreach (var lane in _lanes)
            {
                if (lane == lanes[targetLane]) continue;// skip current lane
                if (!Physics2D.Raycast(new Vector2(lane, rayPoint.position.y), Vector2.up, 1.5f * rayLength).transform)
                {
                    freelane = lane < 0 ? 0 : (lane == 0 ? 1 : 2);
                    break;
                }
            }

            HitCar = hit2D.transform;
            distance = hit2D.distance;
            blocked = true;
        }

        return blocked;
    }

    public void RemoveHitCar()
    {
        if (HitCar) HitCar.gameObject.SetActive(false);
        CameraShaker.Instance.ShakeCam(0.2f, 0.2f);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawRay(rayPoint.position, Vector3.up * rayLength);
    }

    private void UpdatePlayerSprite(int dayornight)
    {
        int selected = PlayerPrefs.GetInt("SELECTED_VEHICLE", 0);
        spriteRenderer.sprite = spriteArray[selected, dayornight];

        float sub = new float[] { 1f, 1.5f, 2.2f }[selected];
        rayPoint.localPosition = new Vector3(0, dayornight == 0 ? sub : (spriteRenderer.bounds.size.y * 0.5f), 0);
    }

    public void UpdatePlayerIcon()
    {
        try
        {
            int leftChances = GameStateManager.Singleton.getChancesLeft;
            playerIcon.sprite = playerIconSprites[PlayerPrefs.GetInt("SELECTED_CHARACTER", 0) * 4 + (3 - leftChances)];
            playerIcon.SetNativeSize();
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    public Vector3 getRayPos => rayPoint.position;
}
