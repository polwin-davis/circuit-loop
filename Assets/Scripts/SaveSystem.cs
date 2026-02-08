using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string SavePath =>
        Application.persistentDataPath + "/save.json";

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static SaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            SaveData newData = new SaveData();
            Save(newData);
            return newData;
        }

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<SaveData>(json);
    }
}
