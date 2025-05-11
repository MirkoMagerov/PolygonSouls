using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    private int screenMode = 1; // 0 = Windowed, 1 = Fullscreen, 2 = Borderless
    private int graphicsQuality = 2; // 0 = Low, 1 = Medium, 2 = High
    private int musicVolume = 100;
    private int sfxVolume = 100;

    [SerializeField] private GameObject settingsCanvas;
    [SerializeField] private TextMeshProUGUI screenModeText;
    [SerializeField] private TextMeshProUGUI graphicsQualityText;

    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private TextMeshProUGUI musicVolumeText;

    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;
    [SerializeField] private AudioMixer audioMixer;

    private void Start()
    {
        LoadSettings();

        UpdateSettingsUI();

        settingsCanvas.SetActive(false);
    }

    private void UpdateSettingsUI()
    {
        UpdateScreenModeText();
        UpdateGraphicsQualityText();
        UpdateMusicVolumeText();
        UpdateSFXVolumeText();
    }

    public void SetScreenMode(int mode)
    {
        screenMode = mode;
        switch (screenMode)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
        }
        UpdateScreenModeText();
    }

    public void IncrementScreenMode()
    {
        screenMode = (screenMode + 1) % 3;
        SetScreenMode(screenMode);
    }

    public void DecrementScreenMode()
    {
        screenMode = (screenMode - 1 + 3) % 3;
        SetScreenMode(screenMode);
    }

    private void UpdateScreenModeText()
    {
        if (screenModeText != null)
        {
            string[] modes = { "Windowed", "Fullscreen", "Borderless" };
            screenModeText.text = modes[screenMode];
        }
    }

    public void SetGraphicsQuality(int quality)
    {
        graphicsQuality = quality;
        QualitySettings.SetQualityLevel(graphicsQuality);
        UpdateGraphicsQualityText();
    }

    public void IncrementGraphicsQuality()
    {
        graphicsQuality = (graphicsQuality + 1) % 3;
        SetGraphicsQuality(graphicsQuality);
    }

    public void DecrementGraphicsQuality()
    {
        graphicsQuality = (graphicsQuality - 1 + 3) % 3;
        SetGraphicsQuality(graphicsQuality);
    }

    private void UpdateGraphicsQualityText()
    {
        if (graphicsQualityText != null)
        {
            string[] qualities = { "Low", "Medium", "High" };
            graphicsQualityText.text = qualities[graphicsQuality];
        }
    }

    public void HandleMusicSlider()
    {
        musicVolume = (int)musicVolumeSlider.value;
        SetMusicVolume(musicVolume);
    }

    private void SetMusicVolume(int volume)
    {
        musicVolume = Mathf.Clamp(volume, 0, 100);

        float audioVolume = musicVolume > 0 ? Mathf.Log10(musicVolume / 100f) * 20 : -80f;
        audioMixer.SetFloat("Music", audioVolume);

        UpdateMusicVolumeText();
    }

    private void UpdateMusicVolumeText()
    {
        musicVolumeText.text = musicVolume.ToString();
    }

    public void HandleSFXSlider()
    {
        sfxVolume = (int)sfxVolumeSlider.value;
        SetSFXVolume(sfxVolume);
    }

    private void SetSFXVolume(int volume)
    {
        sfxVolume = Mathf.Clamp(volume, 0, 100);

        float audioVolume = sfxVolume > 0 ? Mathf.Log10(sfxVolume / 100f) * 20 : -80f;
        audioMixer.SetFloat("SFX", audioVolume);

        UpdateSFXVolumeText();
    }

    private void UpdateSFXVolumeText()
    {
        sfxVolumeText.text = sfxVolume.ToString();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("ScreenMode", screenMode);
        PlayerPrefs.SetInt("GraphicsQuality", graphicsQuality);
        PlayerPrefs.SetInt("MusicVolume", musicVolume);
        PlayerPrefs.SetInt("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        screenMode = PlayerPrefs.GetInt("ScreenMode", 0);
        graphicsQuality = PlayerPrefs.GetInt("GraphicsQuality", 2);
        musicVolume = PlayerPrefs.GetInt("MusicVolume", 100);
        sfxVolume = PlayerPrefs.GetInt("SFXVolume", 100);

        SetScreenMode(screenMode);
        SetGraphicsQuality(graphicsQuality);
        SetMusicVolume(musicVolume);
        musicVolumeSlider.value = musicVolume;
        SetSFXVolume(sfxVolume);
        sfxVolumeSlider.value = sfxVolume;
    }

    public void DisableSettingsCanvas()
    {
        settingsCanvas.SetActive(false);
    }
}
