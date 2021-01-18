using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public int AppleQuantity { get; set; }
    public int StageRecord { get; set; }
    public int Score { get; set; }

    public GameData(int appleQuantity, int stageRecord, int score)
    {
        AppleQuantity = appleQuantity;
        StageRecord = stageRecord;
        Score = score;
    }

    public static GameData GetInitialData()
    {
        return new GameData(0, 0, 0);
    }
}
