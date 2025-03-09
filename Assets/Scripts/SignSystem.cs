using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SignSystem : MonoBehaviour
{
    [SerializeField] Sprite[] signs;
    int displayingSign = 0;
    [SerializeField] Button signButton;
    Button[] signButtons;
    [SerializeField] Image signage;
    [SerializeField] Transform buttonIndicator;
    Transform buttonIndicatorHolder;

    SignChoiceIndicator[] signChoiceIndicators;
    [SerializeField] SignChoiceIndicator crashIndicator;

    Player player;
    float lastSignTimer;
    float signTimer = 0;

    void Start()
    {
        player = FindObjectOfType<Player>();

        signChoiceIndicators = GameObject.FindObjectsOfType<SignChoiceIndicator>();

        signButtons = new Button[signs.Length];
        for (int i = 0; i < signButtons.Length; i++)
        {
            int _i = i;
            signButtons[_i] = Instantiate(signButton, signButton.transform.parent);
            signButtons[_i].image.sprite = signs[_i];
            signButtons[_i].onClick.AddListener(() =>
            {
                ToggleButtonsInteractability(false);
                CheckClickedSign(_i);
                buttonIndicatorHolder = signButtons[_i].transform;
            });
        }
        signButton.gameObject.SetActive(false);

        ToggleButtonsOff(immediate: true);

        UpdateSign();
    }

    void Update()
    {
        lastSignTimer = signTimer;
        signTimer = player.GetHitDistanceRatio();
        if (signTimer < 1f && lastSignTimer >= 1f)
        {
            ToggleOnInteractible();
        }
        else if (signTimer <= 0 && lastSignTimer > 0)
        {
            ToggleButtonsOff();
            crashIndicator.Pop(false);
            GameStateManager.Singleton.Warn();
            player.RemoveHitCar();
            UpdateSign();
        }

        if (buttonIndicatorHolder)
            buttonIndicator.position = new Vector3(buttonIndicatorHolder.position.x, buttonIndicator.position.y);
    }

    private void CheckClickedSign(int clicked)
    {
        ToggleButtonsOff();

        foreach (var item in signChoiceIndicators)
        {
            item.Pop(clicked == displayingSign);
        }

        if (clicked == displayingSign)
        {
            player.SwitchLane();
            GameStateManager.Singleton.AddScore(100);
        }
        else
        {
            GameStateManager.Singleton.Warn();
            player.RemoveHitCar();
        }

        UpdateSign();
    }

    private void UpdateSign()
    {
        displayingSign = (displayingSign + Random.Range(0, signs.Length)) % signs.Length;
        signage.sprite = signs[displayingSign];
    }

    public void ToggleOnInteractible()
    {
        ToggleButtonsOn();
        StartCoroutine(AutoClick());
    }

    private void ToggleButtonsOff(bool immediate = false)
    {
        StartCoroutine(toggleOffRoutine(immediate));
    }
    IEnumerator toggleOffRoutine(bool immediate)
    {
        if (!immediate) yield return new WaitForSeconds(0.4f);
        signButtons[displayingSign].gameObject.SetActive(false);
        foreach (var item in signButtons)
        {
            item.gameObject.SetActive(false);
        }
        //signage.gameObject.SetActive(false);
    }

    private void ToggleButtonsOn()
    {
        var array = signButtons.OrderBy(x => Random.value)
        .Where(x => x != signButtons[displayingSign])
        .Take(4)
        .Append(signButtons[displayingSign]);

        foreach (var item in array)
        {
            item.gameObject.SetActive(true);
            item.interactable = true;
        }

        //signage.gameObject.SetActive(true);
    }

    void ToggleButtonsInteractability(bool flag)
    {
        foreach (var item in signButtons)
        {
            item.interactable = flag;
        }
    }

    int clicks = 0;
    IEnumerator AutoClick()
    {
        //if (clicks < 30)
        if (clicks < 0)
        {
            yield return new WaitForSeconds(Time.deltaTime * 3);
            signButtons[displayingSign].onClick.Invoke();
        }
        clicks++;
    }
}
