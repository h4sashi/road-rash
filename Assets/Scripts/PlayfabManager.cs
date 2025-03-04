using System;
using System.Collections;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayfabManager : MonoBehaviour
{
    public static PlayfabManager Instance;
    public LoginManager loginManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        // PlayerPrefs.DeleteAll();
        Debug.Log("PlayfabManager Started!");
        SceneManager.sceneLoaded += OnSceneLoaded;

        StartCoroutine(AutoLogin(1.3f)); // Attempt auto-login on startup
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            loginManager = GameObject.Find("LoginManager").GetComponent<LoginManager>();
        }
    }

    public void OnLogin(string username, string password, bool rememberMe)
    {
        var request = new LoginWithPlayFabRequest
        {
            Username = username,
            Password = password
        };

        PlayFabClientAPI.LoginWithPlayFab(request, result => OnLoginSuccess(result, rememberMe), OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result, bool rememberMe)
    {
        loginManager.loaderUI.SetActive(false); // Hide loader UI
        loginManager.LoginContainer.SetActive(false); // Hide login UI

        loginManager.loginStatus.text = "Login Successful!";
        Debug.Log("Login Successful! User ID: " + result.PlayFabId);

        if (rememberMe)
        {
            PlayerPrefs.SetString("SavedUsername", loginManager.loginUsername.text);
            PlayerPrefs.SetString("SavedPassword", loginManager.loginPassword.text);
            PlayerPrefs.SetInt("RememberMe", 1);
        }
        else
        {
            PlayerPrefs.DeleteKey("SavedUsername");
            PlayerPrefs.DeleteKey("SavedPassword");
            PlayerPrefs.SetInt("RememberMe", 0);
        }
    }

    private void OnLoginFailure(PlayFabError error)
    {
        loginManager.loginStatus.text = "Login Failed: " + error.ErrorMessage;
        Debug.LogError("Login Failed: " + error.GenerateErrorReport());
    }

    public void OnRegistration(string username, string email, string password)
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = email,
            Password = password,
            Username = username
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegistrationSuccess, OnRegistrationFailure);
    }

    private void OnRegistrationSuccess(RegisterPlayFabUserResult result)
    {
        loginManager.registerStatus.text = "Registration Successful!";
        Debug.Log("Registration Successful! User ID: " + result.PlayFabId + " " + result.Username);
        loginManager.smallLoginUI.SetActive(false);
        OnLogin(result.Username, loginManager.registrationPassword.text, false);

    }

    private void OnRegistrationFailure(PlayFabError error)
    {
        loginManager.registerStatus.text = "Registration Failed: " + error.ErrorMessage;
        Debug.LogError("Registration Failed: " + error.GenerateErrorReport());
    }

   IEnumerator AutoLogin(float t)
{
    yield return new WaitForSeconds(t);

    if (PlayerPrefs.GetInt("RememberMe", 0) == 1)
    {
        loginManager.LoginContainer.SetActive(false); // Hide login UI
        loginManager.loaderUI.SetActive(true); // Show loader UI

        string savedUsername = PlayerPrefs.GetString("SavedUsername", "");
        string savedPassword = PlayerPrefs.GetString("SavedPassword", "");

        if (!string.IsNullOrEmpty(savedUsername) && !string.IsNullOrEmpty(savedPassword))
        {
            OnLogin(savedUsername, savedPassword, true);
            yield break; // Stop execution if login proceeds
        }
    }

    // If auto-login is disabled or fails, show the login UI
    loginManager.loaderUI.SetActive(false); // Hide loader UI
    loginManager.smallLoginUI.SetActive(true); // Show login UI
}


    public void SetUsername(string username)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = username
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, result =>
        {
            Debug.Log("Display Name updated to: " + result.DisplayName);

            // Update UIManager elements
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.usernameText.text = result.DisplayName; // Show updated username
                uiManager.usernamePanel.SetActive(false); // Hide username input panel
                uiManager.characterPanel.SetActive(true); // Show character selection panel
            }

        }, error => Debug.LogError("Failed to update Display Name: " + error.GenerateErrorReport()));
    }

    public void FetchUsername()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), result =>
        {
            string savedUsername = result.AccountInfo.TitleInfo.DisplayName;

            if (!string.IsNullOrEmpty(savedUsername))
            {
                // Found a saved username, update the UI
                UIManager uiManager = FindObjectOfType<UIManager>();
                if (uiManager != null)
                {
                    uiManager.usernameText.text = savedUsername; // Show saved username
                    uiManager.usernamePanel.SetActive(false); // Hide input panel
                    uiManager.characterPanel.SetActive(true); // Show character panel
                }
            }
        },
        error => Debug.LogError("Failed to fetch username: " + error.GenerateErrorReport()));
    }



}
