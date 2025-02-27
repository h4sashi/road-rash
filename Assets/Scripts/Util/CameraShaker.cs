using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    static CameraShaker instance;
    static Transform mainCam;

    Vector3 startPosition;
    Vector3 startRotation;

    void Start()
    {
        startPosition = Vector3.zero;
        startRotation = Vector3.zero;
    }

    public static CameraShaker Instance
    {
        get
        {
            if (instance == null) instance = new GameObject("Shaker").AddComponent<CameraShaker>();
            return instance;
        }
    }

    public void ShakeCam(float time, float magnitude)
    {
        mainCam = Camera.main.transform;
        StartCoroutine(Shake(time, magnitude));
    }

    IEnumerator Shake(float time, float magnitude)
    {
        for (float i = 0; i < time; i += Time.unscaledDeltaTime)
        {
            mainCam.localPosition = startPosition + new Vector3(Random.Range(-1f, 1f) * magnitude, Random.Range(-1f, 1f) * magnitude, Random.Range(-1f, 1f) * magnitude);
            mainCam.localEulerAngles = startRotation + new Vector3(0, 0, Random.Range(-1f, 1f) * magnitude);
            yield return null;
        }

        mainCam.localPosition = startPosition;
        mainCam.localEulerAngles = startRotation;
    }
}
