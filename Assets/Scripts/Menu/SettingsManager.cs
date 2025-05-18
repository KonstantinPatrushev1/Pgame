using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown languageDropdown;
    public Slider volumeSlider;

    private Resolution[] resolutions;

    public CursorManager cursorManager;

    void Start()
    {
        InitializeFullscreen();
        InitializeResolutions();
        InitializeQuality();
        InitializeLanguage();
        InitializeVolume();
    }

    private void InitializeFullscreen()
    {
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        Screen.fullScreen = isFullscreen;
        fullscreenToggle.isOn = isFullscreen;
        fullscreenToggle.onValueChanged.AddListener(delegate { SetFullscreen(fullscreenToggle.isOn); });
    }

    private void InitializeResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int savedResolutionIndex = PlayerPrefs.GetInt("Resolution", -1);
        int currentResolutionIndex = 0;
        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        if (savedResolutionIndex != -1 && savedResolutionIndex < resolutions.Length)
        {
            currentResolutionIndex = savedResolutionIndex;
            // Установить разрешение на сохранённое
            Resolution resolution = resolutions[currentResolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        resolutionDropdown.onValueChanged.AddListener(delegate { SetResolution(resolutionDropdown.value); });
    }

    private void InitializeQuality()
    {
        int qualityLevel = PlayerPrefs.GetInt("Quality", QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(qualityLevel);
        qualityDropdown.value = qualityLevel;
        qualityDropdown.RefreshShownValue();
        qualityDropdown.onValueChanged.AddListener(delegate { SetQuality(qualityDropdown.value); });
    }

    private void InitializeLanguage()
    {
        int languageIndex = PlayerPrefs.GetInt("Language", 0); // По умолчанию английский
        languageDropdown.value = languageIndex;
        languageDropdown.RefreshShownValue();

        // Применяем язык при старте
        StartCoroutine(SetLanguageAtStart(languageIndex));

        // Обработчик для смены языка в меню
        languageDropdown.onValueChanged.AddListener(delegate { SetLanguage(languageDropdown.value); });
    }

    private IEnumerator SetLanguageAtStart(int languageIndex)
    {
        yield return LocalizationSettings.InitializationOperation; // Ждём инициализации локализации

        // Применяем язык на основе индекса
        string languageCode = languageIndex == 0 ? "en" : "ru"; // 0 — английский, 1 — русский
        Locale locale = LocalizationSettings.AvailableLocales.Locales.Find(l => l.Identifier.Code == languageCode);

        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
            Debug.Log($"Language set to: {locale.Identifier.Code}");
        }
        else
        {
            Debug.LogWarning($"Locale with code '{languageCode}' not found!");
        }
    }

    private void InitializeVolume()
    {
        float volume = PlayerPrefs.GetFloat("Volume", 1f);
        AudioListener.volume = volume;
        volumeSlider.value = volume;
        volumeSlider.onValueChanged.AddListener(delegate { SetVolume(volumeSlider.value); });
        AudioListener.volume = volumeSlider.value;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("Resolution", resolutionIndex);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("Quality", qualityIndex);
    }

    public void SetLanguage(int languageIndex)
    {
        string languageCode = languageIndex == 0 ? "en" : "ru"; // Предполагается, что 0 — английский, 1 — русский
        Locale locale = LocalizationSettings.AvailableLocales.Locales.Find(l => l.Identifier.Code == languageCode);

        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
            PlayerPrefs.SetInt("Language", languageIndex); // Сохраняем индекс выбранного языка
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning($"Locale with code '{languageCode}' not found!");
        }
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
    }
    
    public void LoadScene(string sceneName)
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene(sceneName);
    }

    public void ExitGame()
    {
        PlayerPrefs.Save();
        Application.Quit();
    }
}