using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SettingsData
{
    public bool IsSoundOn { get; set; }
    public bool IsMusicOn { get; set; }
    public bool IsVibrationOn { get; set; }

    public SettingsData(bool soundOn, bool musicOn, bool vibrationOn)
    {
        IsSoundOn = soundOn;
        IsMusicOn = musicOn;
        IsVibrationOn = vibrationOn;
    }

    public static SettingsData GetInitialData()
    {
        return new SettingsData(true, true, true);
    }
}
