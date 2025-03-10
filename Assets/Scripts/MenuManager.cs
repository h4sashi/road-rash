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

    public Animator scoreboardAnim, creditsAnim, settingsAnim;

public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Leaderboard()
    {
        mainMenuPanel.SetActive(false);
        scoreboardAnim.SetTrigger("Open");
        LeaderboardManager.Singleton.GetLeaderboard();
    }

    public void Credits()
    {
        creditsAnim.SetTrigger("Open");
        mainMenuPanel.SetActive(false);
    }

    public void Settings()
    {
        settingsAnim.SetTrigger("Open");
        mainMenuPanel.SetActive(false);
    }

    public void MainMenu()
    {
        scoreboardAnim.SetTrigger("Close");
        creditsAnim.SetTrigger("Close");
        settingsAnim.SetTrigger("Close");
      
        // settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
