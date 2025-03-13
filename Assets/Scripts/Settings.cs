using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] Toggle Music, SFX, Haptic;
    public static bool musicOn, sfxOn, hapticOn;

    void Start()
    {
        Music.onValueChanged.AddListener((isOn) =>
        {
            PlayerPrefs.SetInt("MUSIC", isOn ? 1 : 0);
            PlayerPrefs.Save();
            musicOn = isOn;
            ToggleMusic();
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

        musicOn = Music.isOn = PlayerPrefs.GetInt("MUSIC", 1) > 0;
        sfxOn = SFX.isOn = PlayerPrefs.GetInt("SFX", 1) > 0;
        hapticOn = Haptic.isOn = PlayerPrefs.GetInt("HAPTIC", 1) > 0;
    }

    void Update()
    {

    }

    public void ToggleMusic()
    {
        if (!Music.isOn)
        {
            SoundManager.Instance?.Stop("Theme");
            SoundManager.Instance?.Stop("MenuVoicing");
        }
        else
        {
            SoundManager.Instance?.Play("Theme");
            SoundManager.Instance?.Play("MenuVoicing");
        }
    }
}
