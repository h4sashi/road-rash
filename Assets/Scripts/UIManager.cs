using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    public GameObject usernamePanel, characterPanel;
    public InputField usernameInput;

     public TextMeshProUGUI messageText;

    // void Start()
    // {
    //     PlayfabManager.Instance.FetchUsername();
    // }

     void OnEnable()
    {
        PlayfabManager.Instance.FetchUsername();
        Debug.Log("Fetching Username on OnEnable()");
    }

    public void Submit()
    {
        if (!string.IsNullOrEmpty(usernameInput.text))
        {
            PlayfabManager.Instance.SetUsername(usernameInput.text);
        }
        else
        {
            Debug.LogWarning("Username cannot be empty!");
        }
    }

    public void BackStart()
    {
        usernamePanel.SetActive(true);
        characterPanel.SetActive(false);
    }

    public void BackSubmit()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void SoundToggle()
    {
        // Implement sound toggle logic here
    }
}
