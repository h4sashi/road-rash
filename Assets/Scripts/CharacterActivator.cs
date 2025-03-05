using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActivator : MonoBehaviour
{
    [Header("Player Avatars Parent")]
    public Transform avatarParent; // Parent containing all avatars

    [Header("Vehicles Parent")]
    public Transform vehicleParent; // Parent containing all vehicles

    void Start()
    {
        // Retrieve saved selections
        int selectedAvatarIndex = PlayerPrefs.GetInt("SELECTED_CHARACTER", -1);
        int selectedVehicleIndex = PlayerPrefs.GetInt("SELECTED_VEHICLE", -1);

        // Ensure valid selections exist
        if (selectedAvatarIndex == -1 || selectedVehicleIndex == -1)
        {
            Debug.LogError("No valid selections found! Ensure selection is made before loading the scene.");
            return;
        }

        // Activate selected avatar and deactivate others
        for (int i = 0; i < avatarParent.childCount; i++)
        {
            avatarParent.GetChild(i).gameObject.SetActive(i == selectedAvatarIndex);
        }

        // Activate selected vehicle and deactivate others
        for (int i = 0; i < vehicleParent.childCount; i++)
        {
            vehicleParent.GetChild(i).gameObject.SetActive(i == selectedVehicleIndex);
        }

        Debug.Log("Activated Avatar: " + selectedAvatarIndex);
        Debug.Log("Activated Vehicle: " + selectedVehicleIndex);
    }
}
