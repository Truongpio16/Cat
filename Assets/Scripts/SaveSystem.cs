using System;
using System.Collections.Generic;
using System.IO;
using Cat.Core;
using UnityEngine;

[Serializable]
public class SaveData
{
    public List<int> completedLevels = new List<int>();
}

public static class SaveSystem
{
    private static string FilePath => Path.Combine(Application.persistentDataPath, "save.json");

    // Never throws for a missing, empty or damaged file: the player starts with empty progress instead of
    // crashing on the Home screen. A damaged non-empty file is kept as save.json.corrupt, not overwritten.
    public static SaveData Load()
    {
        if (!SafeFile.TryRead(FilePath, out string json))
        {
            return new SaveData();
        }

        if (string.IsNullOrWhiteSpace(json))
        {
            return new SaveData();
        }

        SaveData data = null;
        try
        {
            data = JsonUtility.FromJson<SaveData>(json);
        }
        catch (ArgumentException e)
        {
            Debug.LogWarning("save.json could not be parsed: " + e.Message);
        }

        if (data == null || data.completedLevels == null)
        {
            bool kept = SafeFile.Quarantine(FilePath);
            Debug.LogWarning(kept
                ? "save.json is damaged; kept as save.json.corrupt and started with empty progress."
                : "save.json is damaged and could not be moved aside; started with empty progress.");
            return new SaveData();
        }

        return data;
    }

    // Atomic write; an IO failure is logged and leaves the previous save file intact.
    public static void Save(SaveData data)
    {
        try
        {
            SafeFile.WriteAtomic(FilePath, JsonUtility.ToJson(data));
        }
        catch (IOException e)
        {
            Debug.LogError("save.json could not be written: " + e.Message);
        }
        catch (UnauthorizedAccessException e)
        {
            Debug.LogError("save.json could not be written: " + e.Message);
        }
    }
}
