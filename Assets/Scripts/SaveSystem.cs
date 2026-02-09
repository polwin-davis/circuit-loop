using UnityEngine;
using System.IO;
using System.Collections.Generic;

public static class SaveSystem
{
    private static string SavePath =>
        Application.persistentDataPath + "/save.json";

    private const int CURRENT_APP_VERSION = 1;

    public static SaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            SaveData fresh = CreateFreshData();
            Save(fresh);
            return fresh;
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data == null || data.appVersion != CURRENT_APP_VERSION)
        {
            SaveData fresh = CreateFreshData();
            Save(fresh);
            return fresh;
        }

        return data;
    }

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static void ResetProgress()
    {
        Save(CreateFreshData());
    }

    private static SaveData CreateFreshData()
    {
        return new SaveData
        {
            unlockedLevel = 1,
            levelScores = new List<LevelScore>(),
            finalLevelCompleted = false,
            appVersion = CURRENT_APP_VERSION
        };
    }
}
