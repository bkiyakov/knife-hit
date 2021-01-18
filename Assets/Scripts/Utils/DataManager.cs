using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class DataManager
{
    public const string GAME_DATA_FILENAME = "gamedata.bk";
    public const string SETTINGS_DATA_FILENAME = "settings.bk";
    public static void SaveGameData(GameData data)
    {
        SaveData(data, GAME_DATA_FILENAME);
    }

    public static void SaveSettingsData(SettingsData data)
    {
        SaveData(data, SETTINGS_DATA_FILENAME);
    }

    public static GameData LoadGameData()
    {
        string path = Application.persistentDataPath + "/" + GAME_DATA_FILENAME;

        if (File.Exists(path))
        {
            BinaryFormatter bFormatter = new BinaryFormatter();
            using(FileStream stream = new FileStream(path, FileMode.Open))
            {
                GameData gameData = (GameData) bFormatter.Deserialize(stream);

                return gameData;
            }
        } else
        {
            return GameData.GetInitialData();
        }
    }

    public static SettingsData LoadSettingsData()
    {
        string path = Application.persistentDataPath + "/" + SETTINGS_DATA_FILENAME;

        if (File.Exists(path))
        {
            BinaryFormatter bFormatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                SettingsData settingsData = (SettingsData)bFormatter.Deserialize(stream);

                return settingsData;
            }
        }
        else
        {
            return SettingsData.GetInitialData();
        }
    }

    private static void SaveData(object data, string fileName)
    {
        BinaryFormatter bFormatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/" + fileName;

        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            bFormatter.Serialize(stream, data);
        }
    }
}
