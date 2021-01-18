using System;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField]
    private Stage[] stageList;
    [SerializeField]
    private Stage[] bossesList;
    private int normalStageNumber;
    private int commonStageNumber;
    public Stage currentStage { get; private set; }
    protected StageManager() { }

    private static StageManager instance;

    public static StageManager Instance
    {
        get
        {
            return StageManager.instance;
        }
    }
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        normalStageNumber = 1;
        commonStageNumber = 1;
        currentStage = stageList[0];
    }

    public Stage SetNextStage()
    {
        commonStageNumber++;

        if (commonStageNumber % 5 != 0) // Каждый пятый раунд босс
        {
            currentStage = GetNormalStage();
        } else
        {
            if (commonStageNumber == 0)
            {
                currentStage = GetNormalStage();
            }
            else
            {
                currentStage = GetBossStage();
            }
        }

        return currentStage;
    }

    private Stage GetNormalStage()
    {
        if (normalStageNumber >= stageList.Length)
            normalStageNumber = 0;
        return stageList[normalStageNumber++];
    }

    private Stage GetBossStage()
    {
        return bossesList[UnityEngine.Random.Range(0, bossesList.Length)];
    }

    public void ResetStages()
    {
        commonStageNumber = 1;
        normalStageNumber = 1;
        currentStage = stageList[0];
    }
}
