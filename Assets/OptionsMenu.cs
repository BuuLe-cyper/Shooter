using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    private const string MUSIC_AUDIO_KEY = "MusicVolume";
    private const string SFX_AUDIO_KEY = "SFXVolume";

    public AudioMixer audioMixer;

    public Dropdown resolutionDropdown;
    Resolution[] resolutions;

    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;


    void Start()
    {
        InitializeResolutionOptions();
        InitializeVolume();
    }

    private void InitializeVolume()
    {
        if (!PlayerPrefs.HasKey(MUSIC_AUDIO_KEY))
        {
            PlayerPrefs.SetFloat(MUSIC_AUDIO_KEY, 1);
        }
        if (!PlayerPrefs.HasKey(SFX_AUDIO_KEY))
        {
            PlayerPrefs.SetFloat(SFX_AUDIO_KEY, 1);
        }

        // Set sliders to stored values
        float storedMusicVolume = PlayerPrefs.GetFloat(MUSIC_AUDIO_KEY);
        float storedSFXVolume = PlayerPrefs.GetFloat(SFX_AUDIO_KEY);

        musicVolumeSlider.value = storedMusicVolume;
        sfxVolumeSlider.value = storedSFXVolume;

        // Set the audio mixer volumes to match the slider values on load
        audioMixer.SetFloat(MUSIC_AUDIO_KEY, storedMusicVolume);
        audioMixer.SetFloat(SFX_AUDIO_KEY, storedSFXVolume);
    }

    public void InitializeResolutionOptions()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            Debug.Log(option);
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }


    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat(MUSIC_AUDIO_KEY, volume);

        PlayerPrefs.SetFloat(MUSIC_AUDIO_KEY, volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat(SFX_AUDIO_KEY, volume);

        PlayerPrefs.SetFloat(SFX_AUDIO_KEY, volume);
    }

    public void SetQuality(int qualityIndex)
    {
        Debug.Log(qualityIndex);
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
