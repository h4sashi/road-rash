using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Slider loadingBar;
    [SerializeField] float loadTime;
    public GameObject[] startHolder;

    void Start()
    {
        Time.timeScale = 1f;
        startButton.onClick.AddListener(() =>
        {
            startButton.gameObject.SetActive(false);
            loadingBar.gameObject.SetActive(true);
            foreach (var item in startHolder)
            {
                item.SetActive(false);
            }
            StartCoroutine(Load());
        });

        //OpenKeyboard();
    }

    void Update()
    {
        //UpdateKeyboard();
    }

    IEnumerator Load()
    {
        float elapsed = 0;
        while (elapsed < loadTime)
        {
            elapsed += Time.deltaTime;
            loadingBar.value = elapsed / loadTime;
            yield return null;
        }
        loadingBar.value = 1;
        SceneManager.LoadSceneAsync("Profiler");
    }

#if UNITY_WEBGL //&& !UNITY_EDITOR
    TouchScreenKeyboard keyboard;

    void OpenKeyboard()
    {
        keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    void UpdateKeyboard()
    {
        if (keyboard != null && keyboard.status == TouchScreenKeyboard.Status.Done)
        {
            string inputText = keyboard.text;
            // Process inputText
            keyboard = null;
        }
    }
#endif
}
