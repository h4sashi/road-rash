using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public GameObject layoutContainer;
    public Image Sprite;
    public TextMeshProUGUI Header;
    public TextMeshProUGUI Note;
    //public AnimalSignsSO animalSigns;
    public AnimalSign[] animalSigns;

    public Image[] gameOverCharacterImage;

    void OnEnable()
    {
        var sign = animalSigns[Random.Range(0, animalSigns.Length)];

        if (sign.Heading.Length > 0)
        {
            Header.text = sign.Heading;
            Header.gameObject.SetActive(true);
        }
        else
        {
            Header.gameObject.SetActive(false);
        }

        if (sign.Sign)
        {
            Sprite.sprite = sign.Sign;
            Sprite.gameObject.SetActive(true);
        }
        else
        {
            Sprite.gameObject.SetActive(false);
        }

        Note.text = sign.Note;

        layoutContainer.SetActive(!layoutContainer.activeSelf);
        layoutContainer.SetActive(!layoutContainer.activeSelf);

        gameOverCharacterImage[(PlayerPrefs.GetInt("SELECTED_CHARACTER", 0))].gameObject.SetActive(true);
    }
}
