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
    public AnimalSignsSO animalSigns;
    void OnEnable()
    {
        var sign = animalSigns.getSign();

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
    }
}
