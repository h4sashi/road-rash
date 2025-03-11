using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TutSection
{
    public string[] textPrompts;
    public bool actionRequired;
}

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Singleton;
    public TutSection[] tutSections;
    public string[] textPrompts;
    public int currentStep;
    public int currentSection;
    public TextMeshProUGUI tutorialPrompt;
    public Button nextButton;

    public Sprite[] Avatars;
    public Image Avatar;

    void Awake()
    {
        Singleton = this;
        Avatar.sprite = Avatars[PlayerPrefs.GetInt("SELECTED_CHARACTER", 0)];
        Avatar.SetNativeSize();
    }

    IEnumerator Start()
    {
        nextButton.onClick.AddListener(NextPrompt);
        textPrompts = tutSections[currentSection].textPrompts;
        yield return null;
        currentSection = PlayerPrefs.GetInt("CurrentSection", -1);
        if (currentSection > 0) { gameObject.SetActive(false); }
        TriggerTutorial(0);
    }

    void Update()
    {

    }

    public void ShowTutorial()
    {
        if (currentStep >= textPrompts.Length)
        {
            Time.timeScale = 1;
            gameObject.SetActive(false);
            currentStep = 0;
            PlayerPrefs.SetInt("CurrentSection", currentSection);
            PlayerPrefs.Save();
            return;
        }
        Time.timeScale = 0;
        var text = textPrompts[currentStep++];
        StartCoroutine(Display(text));
    }

    IEnumerator Display(string text)
    {
        tutorialPrompt.text = "";

        if (text.Contains("{name}"))
        {
            text = text.Replace("{name}", PlayerPrefs.GetString("NAME", "Bro"));
        }
        if (text.Contains("<br>"))
        {
            text = text.Replace("<br>", "|");
        }
        foreach (char letter in text)
        {
            tutorialPrompt.text += letter == '|' ? "<br>" : letter;
            yield return new WaitForSecondsRealtime(0.0333f);
        }
        nextButton.gameObject.SetActive(!tutSections[currentSection].actionRequired);
    }

    public void NextPrompt()
    {
        nextButton.gameObject.SetActive(false);
        ShowTutorial();
    }

    public void TriggerTutorial(int section)
    {
        Debug.Log("trigerring " + section);
        if (currentSection >= section)
        {
            return;
        }
        gameObject.SetActive(true);
        currentSection++;
        StartCoroutine(TriggerTut());
    }

    IEnumerator TriggerTut()
    {
        Time.timeScale = 0;
        tutorialPrompt.text = "";
        GetComponent<Animator>().Play("SlideIn", 0, 0);
        yield return new WaitForSecondsRealtime(0.5f);
        textPrompts = tutSections[currentSection].textPrompts;
        NextPrompt();
    }

    public void ClosePage()
    {
        if (!tutSections[currentSection].actionRequired || !gameObject.activeInHierarchy)
            return;
        nextButton.onClick.Invoke();
    }
}
