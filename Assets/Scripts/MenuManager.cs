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
        scoreboardAnim.SetBool("isOpen", true);
        LeaderboardManager.Singleton.GetLeaderboard();
    }

    public void Credits()
    {
        creditsAnim.SetBool("isOpen", true);
        mainMenuPanel.SetActive(false);
    }

    public void Settings()
    {
        settingsAnim.SetBool("isOpen", true);
        mainMenuPanel.SetActive(false);
    }

    public void MainMenu()
    {
        scoreboardAnim.SetBool("isOpen", !true);
        creditsAnim.SetBool("isOpen", !true);
        settingsAnim.SetBool("isOpen", !true);

        // settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
