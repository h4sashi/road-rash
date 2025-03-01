using System.Collections;
using UnityEngine;
using UnityEngine.UI;         // Needed for Slider
using UnityEngine.SceneManagement; // Uncomment if you plan to load scenes

public class LoadingScreen : MonoBehaviour
{
    public Slider _loader;
    public float loadingDuration = 3f; // Duration of the simulated load in seconds

    // Start is called before the first frame update
    void Start()
    {
        // Set the slider's maximum value to 1
        _loader.maxValue = 1f;
        // Start the loading coroutine
        StartCoroutine(LoadAsync());
    }

    // Coroutine to simulate loading progress
    IEnumerator LoadAsync()
    {
        float timer = 0f;
        while (timer < loadingDuration)
        {
            timer += Time.deltaTime;
            // Update slider value (normalized between 0 and 1)
            _loader.value = Mathf.Clamp01(timer / loadingDuration);
            yield return null;
        }
        
        // Optional: Once loading is complete, load the next scene
        // SceneManager.LoadScene("YourNextSceneName");
    }
}
