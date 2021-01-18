using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Основной класс для управления игровым процессов. Оповещает слушателей событиями игры.
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Ссылки на префабы и прочие элементы
    public GameObject knifeSpawnPoint;
    public GameObject logSpawnPoint;
    public GameObject logPrefab;
    public GameObject knifePrefab;
    public GameObject applePrefab;
    public GameObject background;
    public GameObject pauseMenu;
    public TextMeshProUGUI stageNameText;
    public TextMeshProUGUI scoreCounterText;
    public TextMeshProUGUI appleCounterText;
    public TextMeshProUGUI pauseMenuScoreText;
    [Tooltip("Время задержки респавна ножа (в секундах)")]
    public float spawnKnifeTimeDelay;
    #endregion
    #region Значения данных уровня и текущих данных пользователя
    public int knifeStartSpawnQuantityMin { get; private set; }
    public int knifeStartSpawnQuantityMax { get; private set; }
    public int appleCount { get; private set; }
    public int stageCount { get; private set; }
    public int scoreCount { get; private set; }

    public int appleSpawnChance { get; private set; }
    public int knifeQuantity { get; private set; }
    public Stage currentStage { get; private set; }
    public bool onPlay { get; private set; }
    #endregion

    private GameData gameData;
    private GameObject currentKnife;
    public int currentStageCounter { get; private set; }
    public int currentScoreCounter { get; private set; }

    protected GameManager() { }

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            return GameManager.instance;
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

        // Загружаем сохраненные данные
        LoadData();
    }

    private void Start()
    {
        PlayNewGame();
    }


    public event Action OnStartingNewGame;
    public void PlayNewGame()
    {
        pauseMenu.SetActive(false);

        StageManager.Instance.ResetStages();

        currentStageCounter = 0;
        currentScoreCounter = 0;

        SetStage(StageManager.Instance.currentStage);

        if (OnStartingNewGame != null)
        {
            OnStartingNewGame();
        }

        appleCounterText.text = appleCount.ToString();
        scoreCounterText.text = currentScoreCounter.ToString();
        stageNameText.GetComponent<Animator>().SetTrigger("Start");
        AudioManager.Instance?.SetStageClips(1);

        onPlay = true;
        SpawnKnife();
        SpawnLog();
    }

    private void SpawnLog()
    {
        Instantiate(logPrefab, logSpawnPoint.transform.position, Quaternion.identity);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(onPlay && currentKnife != null)
                ThrowKnife();
        }
    }

    /// <summary>
    /// Метод для спавна нового ножа, если нож еще не заспавнен
    /// </summary>
    private void SpawnKnife()
    {
        StartCoroutine(SpawnKnifeCoroutine());
    }

    private IEnumerator SpawnKnifeCoroutine()
    {
        if (currentKnife == null)
        {
            yield return new WaitForSeconds(spawnKnifeTimeDelay);

            currentKnife = Instantiate(knifePrefab, knifeSpawnPoint.transform.position, Quaternion.identity);
        }
    }

    /// <summary>
    /// Метод выкидывает заспавненный нож (вызывает метод Throw на ноже)
    /// </summary>
    private void ThrowKnife()
    {
        if (currentKnife != null)
        {
            knifeQuantity--;

            var knife = currentKnife.GetComponent<KnifeController>();
            knife.OnHittedSomething += OnKnifeHitted;
            knife.Throw();
            Debug.Log("SETTING KNIFE TO NULL FROM THROW KNIFE");
            currentKnife = null;

            KnifeWasThrownTrigger();
        }
    }

    private void OnKnifeHitted(GameObject sender, GameObject target)
    {
        if(target.GetComponent<AppleController>() != null)
        {
            //target.GetComponent<AppleController>().WasHit();
            AppleWasHitTrigger();
        } else if (target.GetComponent<LogController>() != null)
        {
            if(knifeQuantity > 0)
            {
                sender.GetComponent<KnifeController>().AttachToTheLog(target.transform);
                target.GetComponent<LogController>().Flash();
            }

            sender.GetComponent<KnifeController>().OnHittedSomething -= OnKnifeHitted;
            LogWasHitTrigger();
        } else if (target.GetComponent<KnifeController>() != null)
        {
            sender.GetComponent<KnifeController>().OnHittedSomething -= OnKnifeHitted;
            StartCoroutine(KnifeWasHitTrigger());
        }
    }

    /// <summary>
    /// Событие загрузки нового уровня.
    /// </summary>
    public Action onStartingNextStageTrigger;
    /// <summary>
    /// Метод устанавливает начинает новый уровень и оповещает слушателей о событии загрузки нового уровня.
    /// </summary>
    /// <returns></returns>
    private IEnumerator GoToNextStage()
    {
        StageManager.Instance.SetNextStage();

        yield return new WaitForSecondsRealtime(2f);
        
        SetStage(StageManager.Instance.currentStage);

        stageNameText.GetComponent<Animator>().SetTrigger("Start");

        onPlay = true;
        SpawnKnife();
        SpawnLog();

        if (onStartingNextStageTrigger != null)
        {
            onStartingNextStageTrigger();
        }
    }
    
    /// <summary>
    /// Событие для выкидывания ножа
    /// </summary>
    public event Action onKnifeWasThrownTrigger;
    /// <summary>
    /// Метод оповещает о событие выкинутого ножа
    /// </summary>
    public void KnifeWasThrownTrigger()
    {
        if (onKnifeWasThrownTrigger != null)
        {
            onKnifeWasThrownTrigger();
        }
    }

    /// <summary>
    /// Событие о задевании ножей
    /// </summary>
    public event Action onKnifeWasHitTrigger;
    /// <summary>
    /// Метод запускает проигрыш и оповещает о событии задевания ножа
    /// </summary>
    public IEnumerator KnifeWasHitTrigger()
    {
        if (onKnifeWasHitTrigger != null)
        {
            onKnifeWasHitTrigger();
        }

        yield return new WaitForSeconds(1f);

        LoseTrigger();
    }

    /// <summary>
    /// Событие попадания в бревно
    /// </summary>
    public event Action onLogWasHitTrigger;
    /// <summary>
    /// Метод оповещает о событии попадания в бревно
    /// </summary>
    public void LogWasHitTrigger()
    {
        currentScoreCounter++;
        if (currentStage.IsBoss)
            currentScoreCounter += 9;

        scoreCounterText.text = currentScoreCounter.ToString();

        if(knifeQuantity > 0)
        {
            SpawnKnife();
        } else
        {
            WinTrigger();
        }

        if (onLogWasHitTrigger != null)
        {
            onLogWasHitTrigger();
        }
    }

    /// <summary>
    /// Событие выигрыша
    /// </summary>
    public event Action onWinTrigger;
    /// <summary>
    /// Метод оповещает о победе на уровне
    /// </summary>
    public void WinTrigger()
    {
        currentStageCounter++;

        onPlay = false;

        stageNameText.GetComponent<Animator>().SetTrigger("End");
        SaveData();

        if (onWinTrigger != null)
        {
            onWinTrigger();
        }

        StartCoroutine(GoToNextStage());
    }

    /// <summary>
    /// Собыитие проигрыша
    /// </summary>
    public event Action onLoseTrigger;
    /// <summary>
    /// Метод оповещает о проигрыше
    /// </summary>
    public void LoseTrigger()
    {
        onPlay = false;
        pauseMenu.SetActive(true);
        pauseMenuScoreText.text = "YOUR SCORE \n" + currentScoreCounter;

        SaveData();

        if (onLoseTrigger != null)
        {
            onLoseTrigger();
        }
    }

    /// <summary>
    /// Событие попадание по яблоку
    /// </summary>
    public event Action onAppleWasHitTrigger;
    /// <summary>
    /// Метод оповещает о попадании в яблоко
    /// </summary>
    /// <param name="id">шd яблока</param>
    public void AppleWasHitTrigger()
    {
        appleCount++;
        appleCounterText.text = appleCount.ToString();

        onAppleWasHitTrigger?.Invoke();
    }

    public event Action onGoingMainMenu;
    public void GoToMainMenu()
    { 
        SceneManager.LoadScene(0);

        onGoingMainMenu?.Invoke();
    }

    private void SaveData()
    {
        if (currentStageCounter > stageCount)
            stageCount = currentStageCounter;
        if (currentScoreCounter > scoreCount)
            scoreCount = currentScoreCounter;

        gameData.AppleQuantity = appleCount;
        gameData.StageRecord = stageCount;
        gameData.Score = scoreCount;

        DataManager.SaveGameData(gameData);
    }
    private void LoadData()
    {
        gameData = DataManager.LoadGameData();

        appleCount = gameData.AppleQuantity;
        scoreCount = gameData.Score;
        stageCount = gameData.StageRecord;
    }

    /// <summary>
    /// Установка настроек текущего уровня
    /// </summary>
    private void SetStage(Stage stage)
    {
        currentStage = stage;

        background.GetComponent<SpriteRenderer>().sprite = currentStage.Background;
        appleSpawnChance = currentStage.AppleChance;
        knifeQuantity = currentStage.LogHealth;
        knifeStartSpawnQuantityMin = currentStage.KnifeSpawnMin;
        knifeStartSpawnQuantityMax = currentStage.KnifeSpawnMax;

        stageNameText.text = !currentStage.IsBoss ?"STAGE " + (currentStageCounter + 1).ToString() : "BOSS " + currentStage.BossName.ToUpper();
    }

}
