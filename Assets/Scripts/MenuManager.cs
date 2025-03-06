using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject leaderboardPanel;
    public GameObject creditsPanel;
    public GameObject settingsPanel;
    public GameObject mainMenuPanel;

public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Leaderboard()
    {
        leaderboardPanel.SetActive(true);
        mainMenuPanel.SetActive(false);

        LeaderboardManager.Singleton.GetLeaderboard();
    }

    public void Credits()
    {
        creditsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void Settings()
    {
        settingsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void MainMenu()
    {
        leaderboardPanel.SetActive(false);
        creditsPanel.SetActive(false);
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
