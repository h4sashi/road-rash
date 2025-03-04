using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    [Header("Avatars")]
    public Button[] avatarBTN;
    private int avatarIndex = -1;
    private Button selectedAvatarButton;

    [Header("Vehicles")]
    public Button[] vehicleBTN;
    private int vehicleIndex = -1;
    private Button selectedVehicleButton;

    [Header("Submit Button")]
    public Button submitBTN;

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
    }

    private void OnAvatarSelect(int _avatarIndex, Button clickedButton)
    {
        // Disable outline on previously selected avatar
        if (selectedAvatarButton != null)
        {
            selectedAvatarButton.GetComponent<Outline>().enabled = false;
        }

        // Update selection
        avatarIndex = _avatarIndex;
        selectedAvatarButton = clickedButton;
        selectedAvatarButton.GetComponent<Outline>().enabled = true; // Enable outline

        PlayerPrefs.SetInt("SELECTED_CHARACTER", avatarIndex);
        Debug.Log("Selected Avatar: " + avatarIndex);
    }

    private void OnVehicleSelect(int _vehicleIndex, Button clickedButton)
    {
        // Disable outline on previously selected vehicle
        if (selectedVehicleButton != null)
        {
            selectedVehicleButton.GetComponent<Outline>().enabled = false;
        }

        // Update selection
        vehicleIndex = _vehicleIndex;
        selectedVehicleButton = clickedButton;
        selectedVehicleButton.GetComponent<Outline>().enabled = true; // Enable outline

        PlayerPrefs.SetInt("SELECTED_VEHICLE", vehicleIndex);
        Debug.Log("Selected Vehicle: " + vehicleIndex);
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

        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}
