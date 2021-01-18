using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject settings;
    public TextMeshProUGUI appleCounter;
    public TextMeshProUGUI scoreCounter;
    public TextMeshProUGUI stageCounter;
    public Button startGameButton;
    public Button settingsButton;
    public Toggle musicToggle;
    public Toggle soundEffectsToggle;
    public Toggle vibrationToogle;

    private GameData gameData;
    private SettingsData settingsData;

    private void Awake()
    {
        gameData = DataManager.LoadGameData();
        settingsData = DataManager.LoadSettingsData();
        
        if (appleCounter != null)
            appleCounter.text += gameData.AppleQuantity.ToString();
        if (scoreCounter != null)
            scoreCounter.text += gameData.Score.ToString();
        if (stageCounter != null)
            stageCounter.text += gameData.StageRecord.ToString();
    }

    private void Start()
    {
        settings.SetActive(false);

        // Отключаем переключатели перед изменением, чтобы не было звука при старте (по OnValueChanged) и включаем после

        if(soundEffectsToggle != null)
        {
            soundEffectsToggle.isOn = !settingsData.IsSoundOn;
        }

        if (musicToggle != null)
        {
            musicToggle.isOn = !settingsData.IsMusicOn;
        }

        if (vibrationToogle != null)
        {
            vibrationToogle.isOn = !settingsData.IsVibrationOn;
        }
    }
    public void GoToMainMenu()
    {
        GameManager.Instance.GoToMainMenu();
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void ToogleMusic(bool mute)
    {
        AudioManager.Instance.ToogleMusic(mute);
    }
    public void ToogleEffects(bool mute)
    {
        AudioManager.Instance.ToogleEffects(mute);
    }

    public void ToogleVibration(bool off)
    {
        AudioManager.Instance.ToogleVibration(off);
    }
}
