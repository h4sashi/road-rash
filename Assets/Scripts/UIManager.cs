using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI usernameText;
    public GameObject usernamePanel, characterPanel;
    public TMP_InputField usernameInput;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Submit()
    {
        PlayfabManager.Instance.SetUsername(usernameInput.text);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void SoundToggle()
    {

    }
}
