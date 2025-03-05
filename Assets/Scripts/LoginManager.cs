using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public GameObject LoginUI, RegisterUI, LoginContainer, loaderUI, smallLoginUI;

    public TextMeshProUGUI registerStatus, loginStatus;

    [Header("Login InputField")]
    public InputField loginUsername;
    public InputField loginPassword;

    [Header("Register InputField")]
    public InputField registrationEmail;
    public InputField registrationUsername; // Add this
    public InputField registrationPassword;

    public Toggle rememberMeToggle;

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
        PlayfabManager.Instance.OnLogin(loginUsername.text, loginPassword.text, rememberMe);
    }

    public void OnRegistration()
    {
        if (registrationPassword.text.Length < 6)
        {
            registerStatus.text = "Password must be at least 6 characters long!";
            return;
        }

        PlayfabManager.Instance.OnRegistration(registrationUsername.text, registrationEmail.text, registrationPassword.text);
    }


}
