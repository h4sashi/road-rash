using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] Toggle Music, SFX, Haptic;
    public static bool musicOn, sfxOn, hapticOn;

    void Awake()
    {
        Music.onValueChanged.AddListener((isOn) =>
        {
            PlayerPrefs.SetInt("MUSIC", isOn ? 1 : 0);
            PlayerPrefs.Save();
            musicOn = isOn;
            if (!musicOn) SoundManager.Instance.Stop("Theme");
        });

        SFX.onValueChanged.AddListener((isOn) =>
        {
            PlayerPrefs.SetInt("SFX", isOn ? 1 : 0);
            PlayerPrefs.Save();
            sfxOn = isOn;
        });

        Haptic.onValueChanged.AddListener((isOn) =>
        {
            PlayerPrefs.SetInt("HAPTIC", isOn ? 1 : 0);
            PlayerPrefs.Save();
            hapticOn = isOn;
        });

        Music.isOn = PlayerPrefs.GetInt("MUSIC", 1) > 0;
        SFX.isOn = PlayerPrefs.GetInt("SFX", 1) > 0;
        Haptic.isOn = PlayerPrefs.GetInt("HAPTIC", 1) > 0;
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
