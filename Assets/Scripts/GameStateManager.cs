// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.UI;  // Import UI namespace
// using System.Runtime.InteropServices;
// using Unity.VisualScripting;

// public class GameStateManager : MonoBehaviour
// {
//     public static GameStateManager Singleton { get; private set; }
//     public static Action OnGameOver;

//     [Header("Health System")]
//     public int maxHealth = 100;
//     private int currentHealth;

//     [Header("Score & Lives")]
//     public int chances = 3;
//     private int score;

//     [Header("UI Elements")]
//     public Image healthImage;

//     [Header("Player Avatar Parent")]
//     public Transform avatarParent; // Direct reference to AvatarParent
//     private Transform activeAvatar;


//     [DllImport("__Internal")]
//     private static extern void Vibrate(int ms);

//     void Awake()
//     {
//         Singleton = this;
//     }

//     void Start()
//     {
//         ResetGame();
//         FindActiveAvatar();
//     }

//     private void FindActiveAvatar()
//     {
//         if (avatarParent == null)
//         {
//             Debug.LogError("AvatarParent not assigned in GameStateManager!");
//             return;
//         }

//         int selectedAvatarIndex = PlayerPrefs.GetInt("SELECTED_CHARACTER", -1);
//         if (selectedAvatarIndex == -1 || selectedAvatarIndex >= avatarParent.childCount)
//         {
//             Debug.LogError("Invalid selected avatar index!");
//             return;
//         }

//         activeAvatar = avatarParent.GetChild(selectedAvatarIndex);
//     }


//     public void ResetGame()
//     {
//         isGameOver = false;
//         ResumeGame();
//         score = 0;
//         chances = 3;
//         currentHealth = maxHealth;
//         UpdateHealthUI();
//         OnGameOver = null;
//     }

//     public void TakeDamage(int damage)
//     {
//         if (isGameOver) return;

//         currentHealth -= damage;
//         currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

// #if UNITY_EDITOR
// #else
//         Vibrate(500);
// #endif
//         UpdateHealthUI();

//         if (currentHealth <= 0)
//         {
//             LoseChance();
//         }
//     }

//     private void LoseChance()
//     {
//         chances--;
//         UpdateHealthUI();
//         UpdateAvatarImages();

//         if (chances > 0)
//         {
//             currentHealth = maxHealth;
//         }
//         else
//         {
//             GameOver();
//         }
//     }

//     private void UpdateAvatarImages()
//     {
//         if (activeAvatar == null)
//         {
//             FindActiveAvatar();
//             if (activeAvatar == null) return;
//         }

//         Image parentImage = activeAvatar.GetComponent<Image>();
//         if (parentImage != null)
//         {
//             parentImage.enabled = chances > 0;
//         }

//         for (int i = 0; i < activeAvatar.childCount; i++)
//         {

//             Transform child = activeAvatar.GetChild(i);
//             Image childImage = child.GetComponent<Image>();
//             if (childImage != null)
//             {

//                 childImage.enabled = (3 - chances) == i;
//                 //  GameObject go = childImage.transform.parent.gameObject;

//                 // if (go.GetComponent<Image>().enabled == true)
//                 // {
//                 //     go.GetComponent<Image>().enabled = false;
//                 // }
//                 // else
//                 // {
//                 //     go.GetComponent<Image>().enabled = false;
//                 // }
//             }
//         }
//     }

//     public void Heal(int amount)
//     {
//         if (isGameOver) return;

//         currentHealth += amount;
//         currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Prevent overhealing
//         UpdateHealthUI();
//     }

//     private IEnumerator AnimateHealthUI(float targetFill)
//     {
//         float duration = 0.5f; // Animation duration (adjust as needed)
//         float startFill = healthImage.fillAmount;
//         float elapsed = 0f;

//         while (elapsed < duration)
//         {
//             elapsed += Time.deltaTime;
//             healthImage.fillAmount = Mathf.Lerp(startFill, targetFill, elapsed / duration);
//             yield return null;
//         }
//         healthImage.fillAmount = targetFill;
//     }

//     private void UpdateHealthUI()
//     {
//         if (healthImage != null)
//         {
//             // Set fill amount based on chances left
//             float fillAmount = chances == 3 ? 1.0f :
//                                chances == 2 ? 0.683f :
//                                chances == 1 ? 0.318f : 0.0f;

//             healthImage.fillAmount = fillAmount;

//             // Update color based on remaining chances
//             if (chances == 3)
//                 healthImage.color = Color.green;
//             else if (chances == 2)
//                 healthImage.color = new Color(1f, 0.5f, 0f); // Orange
//             else if (chances == 1)
//                 healthImage.color = Color.red;
//         }
//     }



//     public bool isGameOver { get; private set; }

//     private void GameOver()
//     {
//         OnGameOver?.Invoke();
//         isGameOver = true;
//         Invoke(nameof(ShowGameOverScreen), 3.1f);
//     }

//     public void Warn()
//     {
//         LoseChance();
// #if UNITY_EDITOR
// #else
//         Vibrate(1000);
// #endif
//     }

//     void ShowGameOverScreen()
//     {
//         GameplayUIManager.Singleton.ShowScreen(Screens.GameOver);
//     }

//     public void AddScore(int _score)
//     {
//         score += _score;
//     }

//     public int getScore => score;

//     public void Reload()
//     {
//         ResetGame();
//         SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//     }

//     public void Home()
//     {
//         SceneManager.LoadScene(0);
//         Time.timeScale = 1;
//     }

//     public void PauseGame()
//     {
//         Time.timeScale = 0;
//     }

//     public void ResumeGame()
//     {
//         Time.timeScale = 1;
//     }
// }
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using Unity.VisualScripting;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Singleton { get; private set; }
    public static Action OnGameOver;

    [Header("Health System")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Score & Lives")]
    public int chances = 3;
    private int score;

    [Header("UI Elements")]
    public Image healthImage;

    [Header("Player Avatar Parent")]
    public Transform avatarParent;
    private Transform activeAvatar;

    [DllImport("__Internal")]
    private static extern void Vibrate(int ms);

    void Awake()
    {
        Singleton = this;
    }

    void Start()
    {
        ResetGame();
        FindActiveAvatar();
    }

    private void FindActiveAvatar()
    {
        if (avatarParent == null)
        {
            Debug.LogError("AvatarParent not assigned in GameStateManager!");
            return;
        }

        int selectedAvatarIndex = PlayerPrefs.GetInt("SELECTED_CHARACTER", -1);
        if (selectedAvatarIndex == -1 || selectedAvatarIndex >= avatarParent.childCount)
        {
            Debug.LogError("Invalid selected avatar index!");
            return;
        }

        activeAvatar = avatarParent.GetChild(selectedAvatarIndex);
    }

    public void ResetGame()
    {
        isGameOver = false;
        ResumeGame();
        score = 0;
        chances = 3;
        currentHealth = maxHealth;
        UpdateHealthUI();
        OnGameOver = null;
    }

    public void TakeDamage(int damage)
    {
        if (isGameOver) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

#if UNITY_EDITOR
#else
        Vibrate(500);
#endif
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            LoseChance();
        }
    }

    private void LoseChance()
    {
        chances--;
        UpdateHealthUI();
        UpdateAvatarImages();

        if (chances > 0)
        {
            currentHealth = maxHealth;
        }
        else
        {
            GameOver();
        }
    }

    private void UpdateAvatarImages()
    {
        if (activeAvatar == null)
        {
            FindActiveAvatar();
            if (activeAvatar == null) return;
        }

        // Get only children with tag "Animal"
        List<Transform> animalChildren = new List<Transform>();
        for (int i = 0; i < activeAvatar.childCount; i++)
        {
            Transform child = activeAvatar.GetChild(i);
            if (child.CompareTag("Animal"))
            {
                animalChildren.Add(child);
            }
        }

        // If no valid "Animal" images found, exit
        if (animalChildren.Count == 0) return;

        // Ensure we have at least 3 images
        if (animalChildren.Count < 3)
        {
            Debug.LogError("Not enough 'Animal' images. Expected at least 3.");
            return;
        }

        // Disable all images first
        foreach (Transform animal in animalChildren)
        {
            Image img = animal.GetComponent<Image>();
            if (img != null) img.enabled = false;
        }

        // Map chances to the correct index
        int indexToActivate = (chances == 2) ? 0 : (chances == 1) ? 1 : (chances <= 0) ? 2 : -1;

        if (indexToActivate != -1 && indexToActivate < animalChildren.Count)
        {
            Image activeImage = animalChildren[indexToActivate].GetComponent<Image>();



            if (activeImage != null)
            {
                GameObject go = activeImage.transform.parent.gameObject;
                Debug.Log(go.name +" is the parent");
                go.GetComponent<Image>().enabled = false;

                activeImage.enabled = true;
                Debug.Log("Chances remaining: " + chances + " | Activated Image: " + activeImage.gameObject.name);
            }
        }
    }



    public void Heal(int amount)
    {
        if (isGameOver) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
    }

    private IEnumerator AnimateHealthUI(float targetFill)
    {
        float duration = 0.5f;
        float startFill = healthImage.fillAmount;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            healthImage.fillAmount = Mathf.Lerp(startFill, targetFill, elapsed / duration);
            yield return null;
        }
        healthImage.fillAmount = targetFill;
    }

    private void UpdateHealthUI()
    {
        if (healthImage != null)
        {
            float fillAmount = chances == 3 ? 1.0f :
                               chances == 2 ? 0.683f :
                               chances == 1 ? 0.318f : 0.0f;

            healthImage.fillAmount = fillAmount;

            if (chances == 3)
                healthImage.color = Color.green;
            else if (chances == 2)
                healthImage.color = new Color(1f, 0.5f, 0f);
            else if (chances == 1)
                healthImage.color = Color.red;
        }
    }

    public bool isGameOver { get; private set; }

    private void GameOver()
    {
        OnGameOver?.Invoke();
        isGameOver = true;
        Invoke(nameof(ShowGameOverScreen), 3.1f);
    }

    public void Warn()
    {
        LoseChance();
#if UNITY_EDITOR
#else
        Vibrate(1000);
#endif
    }

    void ShowGameOverScreen()
    {
        GameplayUIManager.Singleton.ShowScreen(Screens.GameOver);
    }

    public void AddScore(int _score)
    {
        score += _score;
    }

    public int getScore => score;

    public void Reload()
    {
        ResetGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Home()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
}
