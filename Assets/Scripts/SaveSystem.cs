using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class SaveData
{
    public List<int> completedLevels = new List<int>();
}

public static class SaveSystem
{
    private static string FilePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static SaveData Load()
    {
        if (!File.Exists(FilePath))
        {
            return new SaveData();
        }

        string json = File.ReadAllText(FilePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(FilePath, json);
    }
}
