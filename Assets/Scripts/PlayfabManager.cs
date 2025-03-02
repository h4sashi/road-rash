using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayfabManager : MonoBehaviour
{
    private string customID;
    public UIManager uiManager;

    public EventSystem eventSystem;
    public Image inactivePanel;

    public static PlayfabManager Instance;


    private void Awake()
    {
        uiManager = GameObject.FindObjectOfType<UIManager>();

        // SceneManager.sceneLoaded += OnSceneLoaded;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        customID = "SabiDriver" + Guid.NewGuid().ToString();
    }

    void Start()
    {
        PlayerPrefs.DeleteAll();
        // Generate a unique CustomID for the player (You may want to store this in PlayerPrefs for persistence)
        if (!PlayerPrefs.HasKey("PlayFabCustomID"))
        {
            customID = "SabiDriver"+Guid.NewGuid().ToString(); // Generate a unique GUID
            PlayerPrefs.SetString("PlayFabCustomID", customID);
            PlayerPrefs.Save();
        }
        else
        {
            customID = PlayerPrefs.GetString("PlayFabCustomID");
        }


        Login();
    }

    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = customID,
            CreateAccount = true // Create account if it doesn't exist
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login Successful! PlayFab ID: " + result.PlayFabId);
        eventSystem.enabled = true;
        inactivePanel.gameObject.SetActive(false);

        // Check if username is set
        if (string.IsNullOrEmpty(result.InfoResultPayload?.PlayerProfile?.DisplayName))
        {
            Debug.Log("No username set. Prompting player to set one.");
            // Example: Call UI to ask for username input
        }
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError("Login Failed: " + error.GenerateErrorReport());
    }

    public void SetUsername(string username)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = username
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnUsernameSetSuccess, OnUsernameSetFailure);
    }

    void OnUsernameSetSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Username set successfully: " + result.DisplayName);
        uiManager.usernameText.text = result.DisplayName;
        uiManager.characterPanel.SetActive(true);
        uiManager.usernamePanel.SetActive(false);
    }

    void OnUsernameSetFailure(PlayFabError error)
    {
        Debug.LogError("Failed to set username: " + error.GenerateErrorReport());
    }
}


