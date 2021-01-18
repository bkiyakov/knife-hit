using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    #region Звуки общие для всей игры
    public AudioClip menuMusic;
    public AudioClip stagesCommonMusic;
    public AudioClip start;
    public AudioClip win;
    public AudioClip lose;
    public AudioClip throwingKnife;
    public AudioClip knifeHitted; // При добавлении разных ножей можно также перенести в уровни
    public AudioClip appleHitted;
    public AudioClip buttonSound;
    #endregion
    #region Звуки меняющиеся с уровнем
    private AudioClip logHitted;
    private AudioClip logBreaking;
    #endregion

    public bool IsSoundOn { get; private set; }
    public bool IsMusicOn { get; private set; }
    public bool IsVibrationOn { get; private set; }

    [SerializeField]
    private AudioSource musicPlayer;
    [SerializeField]
    private AudioSource effectsPlayer;
    private Animator animator; // для плавного перехода музыки
    private SettingsData settingsData;
    private bool isVibrationOn;
    private bool isSubscribedOnGameManager; // флаг для проверки подписки на события менеджера, чтобы не подписываться и воспроизводить звук несколько раз

    protected AudioManager() { }

    private static AudioManager instance;

    public static AudioManager Instance
    {
        get
        {
            return AudioManager.instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);

        animator = GetComponent<Animator>();
        settingsData = DataManager.LoadSettingsData();
        isSubscribedOnGameManager = false;

    }

    void Start()
    {
        SetStageClips(0);
    }

    private void Update()
    {
        effectsPlayer.mute = !settingsData.IsSoundOn;
        musicPlayer.mute = !settingsData.IsMusicOn;
        isVibrationOn = settingsData.IsVibrationOn;
    }

    public void PlayButtonSound()
    {
        effectsPlayer.PlayOneShot(buttonSound);
    }

    public void ToogleMusic(bool mute)
    {
        settingsData.IsMusicOn = !mute;
        SaveChanges();
    }
    public void ToogleEffects(bool mute)
    {
        settingsData.IsSoundOn = !mute;
        SaveChanges();
    }

    public void ToogleVibration(bool off)
    {
        settingsData.IsVibrationOn = !off;
        SaveChanges();
    }

    public void SetStageClips(int sceneIndex)
    {
        var stageData = StageManager.Instance?.currentStage;

        if(stageData != null)
        {
            logHitted = stageData.LogHittedSound;
            logBreaking = stageData.LogBreakingSound;
        }
        SetMusicForScene(sceneIndex);

        // Подписываемя на события
        if (!isSubscribedOnGameManager && GameManager.Instance != null)
        {
            GameManager.Instance.onWinTrigger += OnWinPlay;
            GameManager.Instance.onLoseTrigger += OnLosePlay;
            GameManager.Instance.onKnifeWasThrownTrigger += OnKnifeThrowingPlay;
            GameManager.Instance.onLogWasHitTrigger += OnLogHitPlay;
            GameManager.Instance.onKnifeWasHitTrigger += OnKnifeHittedPlay;
            GameManager.Instance.onAppleWasHitTrigger += OnAppleHitPlay;
            GameManager.Instance.onStartingNextStageTrigger += OnStartingNewStage;
            GameManager.Instance.onGoingMainMenu += OnMainMenu;

            isSubscribedOnGameManager = true;
        }
    }

    private void SetMusicForScene(int sceneIndex)
    {
        if (sceneIndex != 0) // Если мы не находимся на главном меню
        {
            var currentStage = GameManager.Instance.currentStage;

            effectsPlayer.PlayOneShot(start, 0.3f);

            if (!currentStage.IsBoss) // Если текущий уровень не босс
            {
                if (musicPlayer.clip != stagesCommonMusic) // Если уже играет общая музыка для уровня (предыдыущая музыка не меню и не босса)
                {
                    animator.SetTrigger("FadeIn");
                    musicPlayer.clip = stagesCommonMusic;
                    musicPlayer.Play();
                }
            }
            else // Если уровень с боссом
            {
                animator.SetTrigger("FadeIn");
                musicPlayer.clip = currentStage.StageMusic;
                musicPlayer.Play();
            }

        } else // Мы находимся в главном меню
        {
            animator.SetTrigger("FadeIn");
            musicPlayer.clip = menuMusic;
            musicPlayer.Play();
        }
    }

    private void SaveChanges()
    {
        DataManager.SaveSettingsData(settingsData);
    }

    private void OnWinPlay()
    {
        effectsPlayer.PlayOneShot(logBreaking);
        effectsPlayer.PlayOneShot(win, 0.7f);
        if (isVibrationOn)
        {
            Vibration.Init();
            Vibration.Vibrate(500);
        }

        //animator.SetTrigger("FadeOut");
    }
    private void OnLosePlay()
    {
        effectsPlayer.PlayOneShot(lose, 0.4f);
        if (isVibrationOn)
        {
            Vibration.Init();
            Vibration.Vibrate(500);
        }
    }

    private void OnKnifeThrowingPlay()
    {
        effectsPlayer.PlayOneShot(throwingKnife, 0.4f);
    }

    private void OnLogHitPlay()
    {
        effectsPlayer.PlayOneShot(logHitted, 0.4f);
        if (isVibrationOn)
        {
            Vibration.Init();
            Vibration.VibratePop();
        }
    }

    private void OnKnifeHittedPlay()
    {
        effectsPlayer.PlayOneShot(knifeHitted);
        if (isVibrationOn)
        {
            Vibration.Init();
            Vibration.Vibrate(200);
        }
    }

    private void OnAppleHitPlay()
    {
        effectsPlayer.PlayOneShot(appleHitted);
    }

    private void OnStartingNewStage()
    {
        SetStageClips(1);
    }

    private void OnMainMenu()
    {
        SetStageClips(0);
    }
}
