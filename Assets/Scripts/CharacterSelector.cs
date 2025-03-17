using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    [Header("Avatars")]
    public Button[] avatarBTN;
    public TextMeshProUGUI[] avatarNameText;
    private int avatarIndex = -1;
    private Button selectedAvatarButton;

    [Header("Vehicles")]
    public Button[] vehicleBTN;
    public TextMeshProUGUI[] vehicleNameText;
    private int vehicleIndex = -1;
    private Button selectedVehicleButton;
    
    public Color selectedColor, unselectedColor;

    [Header("Submit Button")]
    public Button submitBTN;

    [Header("Sprite Changers")]
    public SpriteChanger avatarSpriteChanger;  // Reference to Avatar SpriteChanger
    public SpriteChanger vehicleSpriteChanger; // Reference to Vehicle SpriteChanger

    void Start()
    {
        // Assign button listeners for avatars
        for (int i = 0; i < avatarBTN.Length; i++)
        {
            int index = i;
            avatarBTN[i].onClick.AddListener(() => OnAvatarSelect(index, avatarBTN[index]));
        }

        // Assign button listeners for vehicles
        for (int i = 0; i < vehicleBTN.Length; i++)
        {
            int index = i;
            vehicleBTN[i].onClick.AddListener(() => OnVehicleSelect(index, vehicleBTN[index]));
        }

        // Assign submit button listener
        submitBTN.onClick.AddListener(SaveSelection);

        // Auto-select the saved character and vehicle
        avatarBTN[PlayerPrefs.GetInt("SELECTED_CHARACTER", 0)].onClick.Invoke();
        vehicleBTN[PlayerPrefs.GetInt("SELECTED_VEHICLE", 0)].onClick.Invoke();
    }

    private void OnAvatarSelect(int _avatarIndex, Button clickedButton)
    {
        if (selectedAvatarButton != null)
        {
            selectedAvatarButton.GetComponent<Outline>().enabled = false;
            avatarNameText[avatarIndex].color = unselectedColor;
        }

        avatarIndex = _avatarIndex;
        selectedAvatarButton = clickedButton;
        selectedAvatarButton.GetComponent<Outline>().enabled = true;
        avatarNameText[avatarIndex].color = selectedColor;

        PlayerPrefs.SetInt("SELECTED_CHARACTER", avatarIndex);
        Debug.Log("Selected Avatar: " + avatarIndex);

        avatarSpriteChanger.UpdateSprite(); // Notify SpriteChanger to update
    }

    private void OnVehicleSelect(int _vehicleIndex, Button clickedButton)
    {
        if (selectedVehicleButton != null)
        {
            selectedVehicleButton.GetComponent<Outline>().enabled = false;
            vehicleNameText[vehicleIndex].color = unselectedColor;
        }

        vehicleIndex = _vehicleIndex;
        selectedVehicleButton = clickedButton;
        selectedVehicleButton.GetComponent<Outline>().enabled = true;
        vehicleNameText[vehicleIndex].color = selectedColor;

        PlayerPrefs.SetInt("SELECTED_VEHICLE", vehicleIndex);
        Debug.Log("Selected Vehicle: " + vehicleIndex);

        vehicleSpriteChanger.UpdateSprite(); // Notify SpriteChanger to update
    }

    private void SaveSelection()
    {
        if (avatarIndex == -1 || vehicleIndex == -1)
        {
            Debug.LogWarning("Please select both an avatar and a vehicle before proceeding.");
            return;
        }

        PlayerPrefs.Save();
        Debug.Log("Selections Saved: Avatar " + avatarIndex + ", Vehicle " + vehicleIndex);

        UnityEngine.SceneManagement.SceneManager.LoadScene("LoadingScreen");
    }
}
