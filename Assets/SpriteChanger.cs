using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteChanger : MonoBehaviour
{
    public GameObject[] images; // Collection of GameObjects (each with an Image component)
    public string selectorKey; // e.g., "SELECTED_CHARACTER" or "SELECTED_VEHICLE"

    void Start()
    {
        UpdateSprite(); // Update on start
    }

    public void UpdateSprite()
    {
        int index = PlayerPrefs.GetInt(selectorKey, 0);

        // Disable all images first
        foreach (GameObject img in images)
        {
            img.SetActive(false);
        }

        // Enable the selected image
        if (index >= 0 && index < images.Length)
        {
            images[index].SetActive(true);
        }
    }
}
