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
                //SoundManager.Instance.Play("MenuVoicing");
            }

        }
        if (scene.buildIndex > 0)
        {
            SoundManager.Instance.Stop("MenuVoicing");
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

            //SoundManager.Instance.Play("MenuVoicing");

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
            // ✅ Name successfully set
            Debug.Log("Display Name updated to: " + result.DisplayName);
            PlayerPrefs.SetString("NAME", result.DisplayName);

            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                uiManager.usernameText.text = result.DisplayName;
                uiManager.usernamePanel.SetActive(false);
                uiManager.characterPanel.SetActive(true);
                // uiManager.messageText.gameObject.SetActive(false); // Hide error message if previously shown
            }

        }, error =>
        {
            UIManager uiManager = FindObjectOfType<UIManager>();
            if (uiManager != null)
            {
                string previousText = uiManager.messageText.text; // Store previous message
                uiManager.messageText.color = Color.red;

                if (error.Error == PlayFabErrorCode.NameNotAvailable)
                {
                    uiManager.messageText.text = "Username already taken. Try another.";
                }
                else
                {
                    uiManager.messageText.text = "Failed to set username. Try again.";
                }

                uiManager.messageText.gameObject.SetActive(true);
                uiManager.StartCoroutine(RevertMessage(uiManager, previousText, 4.5f));
            }

            Debug.LogError("Failed to update Display Name: " + error.GenerateErrorReport());
        });
    }

    // Coroutine to revert messageText after a delay
    private IEnumerator RevertMessage(UIManager uiManager, string previousText, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (uiManager != null)
        {
            uiManager.messageText.color = Color.white;
            uiManager.messageText.text = previousText;
            // uiManager.messageText.gameObject.SetActive(false);
        }
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






