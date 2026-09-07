using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Cat.Core;

public static class LevelBatchGenerator
{
    private struct Tier
    {
        public int Width;
        public int Height;
        public int WallCount;
        public bool RequireDeduction;
    }

    // 1 dong = 1 level, dung dung thu tu Level_01..Level_20.
    private static readonly Tier[] LevelSettings =
    {
        // Tutorial (1-5): sieu de, day co ban cho nguoi moi
        new Tier { Width = 3, Height = 3, WallCount = 2, RequireDeduction = true },
        new Tier { Width = 3, Height = 3, WallCount = 3, RequireDeduction = true },
        new Tier { Width = 4, Height = 4, WallCount = 3, RequireDeduction = true },
        new Tier { Width = 4, Height = 4, WallCount = 4, RequireDeduction = true },
        new Tier { Width = 4, Height = 4, WallCount = 5, RequireDeduction = true },
        // De (6-10)
        new Tier { Width = 5, Height = 5, WallCount = 6, RequireDeduction = true },
        new Tier { Width = 5, Height = 5, WallCount = 6, RequireDeduction = true },
        new Tier { Width = 5, Height = 5, WallCount = 6, RequireDeduction = true },
        new Tier { Width = 5, Height = 5, WallCount = 6, RequireDeduction = true },
        new Tier { Width = 5, Height = 5, WallCount = 6, RequireDeduction = true },
        // Vua (11-15)
        new Tier { Width = 6, Height = 6, WallCount = 9, RequireDeduction = true },
        new Tier { Width = 6, Height = 6, WallCount = 9, RequireDeduction = true },
        new Tier { Width = 6, Height = 6, WallCount = 9, RequireDeduction = true },
        new Tier { Width = 6, Height = 6, WallCount = 9, RequireDeduction = true },
        new Tier { Width = 6, Height = 6, WallCount = 9, RequireDeduction = true },
        // Kho (16-20)
        new Tier { Width = 7, Height = 7, WallCount = 12, RequireDeduction = false },
        new Tier { Width = 7, Height = 7, WallCount = 12, RequireDeduction = false },
        new Tier { Width = 7, Height = 7, WallCount = 12, RequireDeduction = false },
        new Tier { Width = 7, Height = 7, WallCount = 12, RequireDeduction = false },
        new Tier { Width = 7, Height = 7, WallCount = 12, RequireDeduction = false },
    };

    public const int TotalLevels = 20;

    public static bool GenerateOne(int levelIndex, int maxAttempts, int seed)
    {
        Tier tier = LevelSettings[levelIndex - 1];
        System.Random rng = new System.Random(seed);

        bool ok = LevelGenerator.TryGenerate(
            tier.Width, tier.Height, tier.WallCount, rng, maxAttempts,
            out LevelBlueprint blueprint, tier.RequireDeduction);

        if (!ok)
        {
            Debug.LogError($"Level {levelIndex}: khong sinh duoc sau {maxAttempts} lan thu (seed {seed}).");
            return false;
        }

        CreateLevelAsset(blueprint, levelIndex);
        AssetDatabase.SaveAssets();
        Debug.Log($"Level {levelIndex}: OK ({tier.Width}x{tier.Height}, {blueprint.Walls.Count} tuong).");
        return true;
    }

    private static void CreateLevelAsset(LevelBlueprint blueprint, int levelIndex)
    {
        LevelData data = ScriptableObject.CreateInstance<LevelData>();
        data.width = blueprint.Width;
        data.height = blueprint.Height;

        LevelData.WallEntry[] entries = new LevelData.WallEntry[blueprint.Walls.Count];
        for (int i = 0; i < blueprint.Walls.Count; i++)
        {
            LevelBlueprint.WallInfo w = blueprint.Walls[i];
            entries[i] = new LevelData.WallEntry
            {
                x = w.X,
                y = w.Y,
                hasNumber = w.HasNumber,
                number = w.Number
            };
        }
        data.walls = entries;

        string path = $"Assets/Levels/Level_{levelIndex:00}.asset";
        AssetDatabase.CreateAsset(data, path);
    }

    [MenuItem("Cat/Migrate Old 15 Levels To Slots 6-20")]
    public static void MigrateOldLevelsToSlotsSixToTwenty()
    {
        for (int i = 1; i <= 15; i++)
        {
            string oldPath = $"Assets/Levels/Level_{i:00}.asset";
            string tempPath = $"Assets/Levels/TempShift_{i:00}.asset";
            string error = AssetDatabase.MoveAsset(oldPath, tempPath);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError($"Loi doi ten {oldPath} -> {tempPath}: {error}");
            }
        }

        for (int i = 1; i <= 15; i++)
        {
            string tempPath = $"Assets/Levels/TempShift_{i:00}.asset";
            string newPath = $"Assets/Levels/Level_{(i + 5):00}.asset";
            string error = AssetDatabase.MoveAsset(tempPath, newPath);
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError($"Loi doi ten {tempPath} -> {newPath}: {error}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Da doi 15 level cu sang Level_06..Level_20.");
    }

    [MenuItem("Cat/Rebuild Level Catalog From Level_XX Assets")]
    public static void RebuildCatalog()
    {
        List<LevelData> levels = new List<LevelData>();

        for (int i = 1; i <= TotalLevels; i++)
        {
            string path = $"Assets/Levels/Level_{i:00}.asset";
            LevelData data = AssetDatabase.LoadAssetAtPath<LevelData>(path);
            if (data == null)
            {
                Debug.LogWarning($"Thieu {path}, bo qua khi build catalog.");
                continue;
            }
            levels.Add(data);
        }

        LevelCatalog catalog = AssetDatabase.LoadAssetAtPath<LevelCatalog>("Assets/Levels/LevelCatalog.asset");
        if (catalog == null)
        {
            Debug.LogError("Khong tim thay LevelCatalog.asset tai Assets/Levels/LevelCatalog.asset");
            return;
        }

        catalog.levels = levels.ToArray();
        EditorUtility.SetDirty(catalog);
        AssetDatabase.SaveAssets();
        Debug.Log($"LevelCatalog da duoc gan {levels.Count} level.");
    }
}
