using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    public GameObject usernamePanel, characterPanel;
    public TMP_InputField usernameInput;

    void Start()
    {
        PlayfabManager.Instance.FetchUsername();
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

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void SoundToggle()
    {
        // Implement sound toggle logic here
    }
}
