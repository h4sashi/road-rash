using System.Collections;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayfabManager : MonoBehaviour
{
    public static PlayfabManager Instance;
    public bool isLoggedIn = false;

    private LoginManager loginManager;
    public GameObject soundManager;


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

        StartCoroutine(AutoLogin(1.3f));
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

            if (isLoggedIn == true)
            {
                SoundManager.instance.Play("MenuVoicing");
            }

        }
        if (scene.buildIndex > 0)
        {
            SoundManager.instance.Stop("MenuVoicing");
        }
    }

    public void OnLogin()
    {
        Debug.Log("Logging In Player..");
        string customId = GetOrCreateCustomId();

        var request = new LoginWithCustomIDRequest
        {
            CustomId = customId,
            CreateAccount = false
        };

        PlayFabClientAPI.LoginWithCustomID(request, result =>
        {
            isLoggedIn = true;
            Debug.Log("Login Successful! User ID: " + result.PlayFabId);

            soundManager.SetActive(true); // Enable sound manager\

            SoundManager.instance.Play("MenuVoicing");

            loginManager.loaderUI.SetActive(false); // Hide loader UI
            loginManager.startHolder.SetActive(true); // Hide small login UI

        }, error =>
        {
            Debug.LogWarning("Login Failed: " + error.GenerateErrorReport());
            if (error.Error == PlayFabErrorCode.AccountNotFound)
            {
                Debug.Log("Account not found, attempting registration...");
                OnRegistration(); // Auto-register if account doesn't exist
            }
        });
    }

    public void OnRegistration()
    {
        Debug.Log("Registering Player..");
        string customId = GetOrCreateCustomId();
        var request = new LoginWithCustomIDRequest
        {
            CustomId = customId,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, result =>
        {

            Debug.Log("Registration & Login Successful! User ID: " + result.PlayFabId);
            OnLogin();

        }, error =>
        {
            Debug.LogError("Registration Failed: " + error.GenerateErrorReport());
        });
    }

    private string GetOrCreateCustomId()
    {
        if (!PlayerPrefs.HasKey("CustomID"))
        {
            string newCustomId = GenerateCustomId();
            PlayerPrefs.SetString("CustomID", newCustomId);
            PlayerPrefs.Save();
        }
        return PlayerPrefs.GetString("CustomID");
    }

    private string GenerateCustomId()
    {
        return SystemInfo.deviceUniqueIdentifier + "_" + System.Guid.NewGuid().ToString();
    }

    IEnumerator AutoLogin(float t)
    {
        yield return new WaitForSeconds(t);
        OnLogin(); // Auto-login or register if necessary
    }

    // ✅ SET USERNAME
    public void SetUsername(string username)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = username
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, result =>
        {
            Debug.Log("Display Name updated to: " + result.DisplayName);
            PlayerPrefs.SetString("NAME", result.DisplayName);

            // Update UIManager elements
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.usernameText.text = result.DisplayName; // Show updated username
                Debug.Log("Username is =" + result.DisplayName);
                uiManager.usernamePanel.SetActive(false); // Hide username input panel
                uiManager.characterPanel.SetActive(true); // Show character selection panel
            }

        }, error => Debug.LogError("Failed to update Display Name: " + error.GenerateErrorReport()));
    }

    // ✅ FETCH USERNAME
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








// using System;
// using System.Collections;
// using PlayFab;
// using PlayFab.ClientModels;
// using TMPro;
// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class PlayfabManager : MonoBehaviour
// {
//     public static PlayfabManager Instance;
//     public LoginManager loginManager;
//     public bool isLoggedIn = false;

//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }

//     private void OnEnable()
//     {
//         // PlayerPrefs.DeleteAll();
//         Debug.Log("PlayfabManager Started!");
//         SceneManager.sceneLoaded += OnSceneLoaded;

//         StartCoroutine(AutoLogin(1.3f)); // Attempt auto-login on startup
//     }

//     private void OnDestroy()
//     {
//         SceneManager.sceneLoaded -= OnSceneLoaded;
//     }

//     private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//     {
//         if (scene.buildIndex == 0)
//         {
//             loginManager = GameObject.Find("LoginManager").GetComponent<LoginManager>();


//         }
//     }

//     public void OnLogin(string username, string password, bool rememberMe)
//     {
//         var request = new LoginWithPlayFabRequest
//         {
//             Username = username,
//             Password = password
//         };

//         PlayFabClientAPI.LoginWithPlayFab(request, result => OnLoginSuccess(result, rememberMe), OnLoginFailure);
//     }

//     private void OnLoginSuccess(LoginResult result, bool rememberMe)
//     {
//         isLoggedIn = true;

//         loginManager.loaderUI.SetActive(false); // Hide loader UI
//         loginManager.LoginContainer.SetActive(false); // Hide login UI
//         loginManager.startHolder.SetActive(true); // Hide small login UI
//         loginManager.loginStatus.text = "Login Successful!";
//         Debug.Log("Login Successful! User ID: " + result.PlayFabId);

//         if (rememberMe)
//         {
//             PlayerPrefs.SetString("SavedUsername", loginManager.loginUsername.text);
//             PlayerPrefs.SetString("SavedPassword", loginManager.loginPassword.text);
//             PlayerPrefs.SetInt("RememberMe", 1);
//         }
//         else
//         {
//             PlayerPrefs.DeleteKey("SavedUsername");
//             PlayerPrefs.DeleteKey("SavedPassword");
//             PlayerPrefs.SetInt("RememberMe", 0);
//         }
//     }

//     private void OnLoginFailure(PlayFabError error)
//     {
//         loginManager.loginStatus.text = "Login Failed: " + error.ErrorMessage;
//         Debug.LogError("Login Failed: " + error.GenerateErrorReport());
//     }

//     public void OnRegistration(string username, string email, string password)
//     {
//         var request = new RegisterPlayFabUserRequest
//         {
//             Email = email,
//             Password = password,
//             Username = username
//         };

//         PlayFabClientAPI.RegisterPlayFabUser(request, OnRegistrationSuccess, OnRegistrationFailure);
//     }

//     private void OnRegistrationSuccess(RegisterPlayFabUserResult result)
//     {
//         loginManager.registerStatus.text = "Registration Successful!";
//         Debug.Log("Registration Successful! User ID: " + result.PlayFabId + " " + result.Username);
//         loginManager.smallLoginUI.SetActive(false);
//         OnLogin(result.Username, loginManager.registrationPassword.text, false);

//     }

//     private void OnRegistrationFailure(PlayFabError error)
//     {
//         loginManager.registerStatus.text = "Registration Failed: " + error.ErrorMessage;
//         Debug.LogError("Registration Failed: " + error.GenerateErrorReport());
//     }

//     IEnumerator AutoLogin(float t)
//     {
//         yield return new WaitForSeconds(t);

//         if (PlayerPrefs.GetInt("RememberMe", 0) == 1)
//         {
//             loginManager.LoginContainer.SetActive(false); // Hide login UI
//             loginManager.loaderUI.SetActive(true); // Show loader UI

//             string savedUsername = PlayerPrefs.GetString("SavedUsername", "");
//             string savedPassword = PlayerPrefs.GetString("SavedPassword", "");

//             if (!string.IsNullOrEmpty(savedUsername) && !string.IsNullOrEmpty(savedPassword))
//             {
//                 OnLogin(savedUsername, savedPassword, true);
//                 yield break; // Stop execution if login proceeds
//             }
//         }

//         // If auto-login is disabled or fails, show the login UI
//         loginManager.loaderUI.SetActive(false); // Hide loader UI
//         loginManager.smallLoginUI.SetActive(true); // Show login UI
//     }


//     public void SetUsername(string username)
//     {
//         var request = new UpdateUserTitleDisplayNameRequest
//         {
//             DisplayName = username
//         };

//         PlayFabClientAPI.UpdateUserTitleDisplayName(request, result =>
//         {
//             Debug.Log("Display Name updated to: " + result.DisplayName);

//             // Update UIManager elements
//             UIManager uiManager = FindObjectOfType<UIManager>();
//             if (uiManager != null)
//             {
//                 uiManager.usernameText.text = result.DisplayName; // Show updated username
//                 uiManager.usernamePanel.SetActive(false); // Hide username input panel
//                 uiManager.characterPanel.SetActive(true); // Show character selection panel
//             }

//         }, error => Debug.LogError("Failed to update Display Name: " + error.GenerateErrorReport()));
//     }

//     public void FetchUsername()
//     {
//         PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), result =>
//         {
//             string savedUsername = result.AccountInfo.TitleInfo.DisplayName;

//             if (!string.IsNullOrEmpty(savedUsername))
//             {
//                 // Found a saved username, update the UI
//                 UIManager uiManager = FindObjectOfType<UIManager>();
//                 if (uiManager != null)
//                 {
//                     uiManager.usernameText.text = savedUsername; // Show saved username
//                     uiManager.usernamePanel.SetActive(false); // Hide input panel
//                     uiManager.characterPanel.SetActive(true); // Show character panel
//                 }
//             }
//         },
//         error => Debug.LogError("Failed to fetch username: " + error.GenerateErrorReport()));
//     }



// }
