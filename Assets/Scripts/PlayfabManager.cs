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
    public bool isLoggedIn = false;

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

    public void OnLogin(string email, bool rememberMe)
    {
        string customId = PlayerPrefs.GetString("CustomID", ""); // Get stored CustomID

        if (string.IsNullOrEmpty(customId))
        {
            Debug.LogError("No CustomID found! User may need to register first.");
            return;
        }

        var request = new LoginWithCustomIDRequest
        {
            CustomId = customId,
            CreateAccount = false
        };

        PlayFabClientAPI.LoginWithCustomID(request, result =>
        {
            isLoggedIn = true;
            Debug.Log("Login Successful! User ID: " + result.PlayFabId);
            loginManager.loaderUI.SetActive(false); // Hide loader UI
        loginManager.LoginContainer.SetActive(false); // Hide login UI
        loginManager.startHolder.SetActive(true); // Hide small login UI
        loginManager.loginStatus.text = "Login Successful!";


            if (rememberMe)
            {
                PlayerPrefs.SetString("SavedEmail", email);
                PlayerPrefs.SetInt("RememberMe", 1);
            }
            else
            {
                PlayerPrefs.DeleteKey("SavedEmail");
                PlayerPrefs.SetInt("RememberMe", 0);
            }
        }, OnLoginFailure);
    }



    private void OnLoginSuccess(LoginResult result, string email, bool rememberMe)
    {
        isLoggedIn = true;
        loginManager.loaderUI.SetActive(false);
        loginManager.LoginContainer.SetActive(false);
        loginManager.startHolder.SetActive(true);
        loginManager.loginStatus.text = "Login Successful!";
        Debug.Log("Login Successful! User ID: " + result.PlayFabId);

        if (rememberMe)
        {
            PlayerPrefs.SetString("SavedEmail", email);
            PlayerPrefs.SetInt("RememberMe", 1);
        }
        else
        {
            PlayerPrefs.DeleteKey("SavedEmail");
            PlayerPrefs.SetInt("RememberMe", 0);
        }
    }

    private void OnLoginFailure(PlayFabError error)
    {
        loginManager.loginStatus.text = "Login Failed: " + error.ErrorMessage;
        Debug.LogError("Login Failed: " + error.GenerateErrorReport());
    }

    public void OnRegistration(string email)
    {
        PlayerPrefs.DeleteAll();
        loginManager.registerStatus.text = "Registering...";

        string customId = GetOrCreateCustomId();

        var request = new LoginWithCustomIDRequest
        {
            CustomId = customId,
            CreateAccount = true
        };
        

        PlayFabClientAPI.LoginWithCustomID(request, result =>
        {

            Debug.Log("Registration Successful with Device ID: " + result.PlayFabId);
            loginManager.registerStatus.text = "Registration Successful!.. Logging In";
            LinkEmailToAccount(email); // Link email after successful registration
        }, OnRegistrationFailure);
    }

    private void CheckAndLinkEmail(string email)
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), result =>
        {
            // Check if an email is already linked
            if (!string.IsNullOrEmpty(result.AccountInfo.PrivateInfo.Email))
            {
                Debug.Log("Email already linked: " + result.AccountInfo.PrivateInfo.Email);
                loginManager.registerStatus.text = "Registration Successful!";
                loginManager.smallLoginUI.SetActive(false);
                OnLogin(email, false); // Proceed with login
            }
            else
            {
                LinkEmailToAccount(email); // If not linked, link the email
            }
        }, error =>
        {
            Debug.LogError("Failed to check account info: " + error.GenerateErrorReport());
            loginManager.registerStatus.text = "Error checking email link status!";
        });
    }


    // Links the provided email to the PlayFab account
    private void LinkEmailToAccount(string email)
    {
        var request = new AddUsernamePasswordRequest
        {
            Email = email,
            Username = email.Split('@')[0], // Create a username from email prefix
            Password = System.Guid.NewGuid().ToString() // Generate a random password (not needed for login)
        };

        PlayFabClientAPI.AddUsernamePassword(request, result =>
        {
            loginManager.registerStatus.text = "Registration Successful!";
            Debug.Log("Email Linked Successfully!");
            loginManager.smallLoginUI.SetActive(false);
            OnLogin(email, false); // Auto-login after linking email
        }, error =>
        {
            Debug.LogError("Failed to link email: " + error.GenerateErrorReport());
            loginManager.registerStatus.text = "Email linking failed: " + error.ErrorMessage;
        });
    }


    IEnumerator StartingLogin(string _email, float t)
    {
        yield return new WaitForSeconds(t);
        OnLogin(_email, false);
    }

    public string GetOrCreateCustomId()
    {
        if (!PlayerPrefs.HasKey("CustomID"))
        {
            string newCustomId = GenerateCustomId();
            PlayerPrefs.SetString("CustomID", newCustomId);
            PlayerPrefs.Save();
        }
        return PlayerPrefs.GetString("CustomID");
    }

    public string GenerateCustomId()
    {
        return SystemInfo.deviceUniqueIdentifier + "_" + System.Guid.NewGuid().ToString();
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

            string savedEmail = PlayerPrefs.GetString("SavedEmail", "");

            if (!string.IsNullOrEmpty(savedEmail))
            {
                OnLogin(savedEmail, true);
                yield break; // Stop execution if login proceeds
            }
        }

        // If auto-login fails, show login UI
        loginManager.loaderUI.SetActive(false);
        loginManager.smallLoginUI.SetActive(true);
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
