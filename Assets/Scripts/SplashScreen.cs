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

    void Start()
    {
        Time.timeScale = 1f;
        startButton.onClick.AddListener(() =>
        {
            startButton.gameObject.SetActive(false);
            loadingBar.gameObject.SetActive(true);
            StartCoroutine(Load());
        });
    }

    void Update()
    {

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
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
