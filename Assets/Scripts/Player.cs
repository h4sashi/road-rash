using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Player : MonoBehaviour
{
    int freeLane;
    int targetLane;
    int[] lanes = { -4, 0, 4 };
    int[] lanesReverse = { 4, 0, -4 };

    float hitDistance;

    [SerializeField] Transform rayPoint;
    [SerializeField] float rayLength = 10f;

    [SerializeField] Sprite[] sprites;
    SpriteRenderer spriteRenderer;

    Transform HitCar;



    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[WeatherManager.GetDayNight()];

        WeatherManager.OnWeatherChange += (time) =>
        {
            spriteRenderer.sprite = sprites[(int)time];
        };

        targetLane = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(lanes[targetLane], transform.position.y, transform.position.z), 20 * Time.fixedDeltaTime);
        float orientation = (lanes[targetLane] - transform.position.x) / 4 * -30f;

        transform.rotation = Quaternion.Euler(0, 0, orientation);

        if (Blocked(out int _freelane, out hitDistance))
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

    public float GetHitDistanceRatio()
    {
        return hitDistance / (rayLength * WorldManager.Difficulty);
    }

    bool Blocked(out int freelane, out float distance)
    {
        bool blocked = false;
        freelane = 0;
        distance = float.MaxValue;

        RaycastHit2D hit2D;

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
        Gizmos.DrawRay(rayPoint.position, Vector3.up * WorldManager.Difficulty * rayLength);
    }
}
