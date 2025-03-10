using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public GameObject LoginUI, RegisterUI, LoginContainer, loaderUI, smallLoginUI, startHolder;
    public TextMeshProUGUI registerStatus, loginStatus;

    [Header("Login InputField")]
    public InputField loginEmail;

    [Header("Register InputField")]
    public InputField registrationEmail;

    public Toggle rememberMeToggle;

    private void Awake()
    {
        if (PlayfabManager.Instance.isLoggedIn)
        {
            LoginContainer.SetActive(false);
            loaderUI.SetActive(false);
            startHolder.SetActive(true);
        }
    }

    public void Register()
    {
        RegisterUI.SetActive(true);
        LoginUI.SetActive(false);
    }

    public void Login()
    {
        RegisterUI.SetActive(false);
        LoginUI.SetActive(true);
    }

    public void OnLogin()
    {
        bool rememberMe = rememberMeToggle.isOn;
        PlayfabManager.Instance.OnLogin(loginEmail.text, rememberMe);
    }

    public void OnRegistration()
    {
        PlayfabManager.Instance.OnRegistration(registrationEmail.text);
    }
}
