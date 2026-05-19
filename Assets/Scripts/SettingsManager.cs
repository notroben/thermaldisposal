using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio")]
    public Slider masterVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("Controls")]
    public Slider sensitivitySlider;
    public Toggle invertYToggle;

    [Header("Display")]
    public Toggle fullscreenToggle;

    void OnEnable()
    {
        LoadSettings();
    }

    void LoadSettings()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
            masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        if (sensitivitySlider != null)
        {
            sensitivitySlider.minValue = 0.5f;
            sensitivitySlider.maxValue = 5f;
            sensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 2f);
            sensitivitySlider.onValueChanged.AddListener(SetMouseSensitivity);
        }

        if (invertYToggle != null)
        {
            invertYToggle.isOn = PlayerPrefs.GetInt("InvertY", 0) == 1;
            invertYToggle.onValueChanged.AddListener(SetInvertY);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }
    }

    public void SetMasterVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void SetMouseSensitivity(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);
    }

    public void SetInvertY(bool value)
    {
        PlayerPrefs.SetInt("InvertY", value ? 1 : 0);
    }

    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
        PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
    }

    void OnDisable()
    {
        PlayerPrefs.Save();

        if (masterVolumeSlider != null) masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
        if (sfxVolumeSlider != null) sfxVolumeSlider.onValueChanged.RemoveListener(SetSFXVolume);
        if (sensitivitySlider != null) sensitivitySlider.onValueChanged.RemoveListener(SetMouseSensitivity);
        if (invertYToggle != null) invertYToggle.onValueChanged.RemoveListener(SetInvertY);
        if (fullscreenToggle != null) fullscreenToggle.onValueChanged.RemoveListener(SetFullscreen);
    }
}
