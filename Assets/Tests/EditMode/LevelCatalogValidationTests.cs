using System.Collections.Generic;
using System.Text.RegularExpressions;
using Cat.Core;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Cat.Core.Tests
{
    // Validates the shipped level assets. LevelData lives in Assembly-CSharp, which an asmdef cannot reference,
    // so assets are read through SerializedObject (fields: width, height, walls[x, y, hasNumber, number]).
    public class LevelCatalogValidationTests
    {
        private static readonly Regex LevelName = new Regex(@"^Level_\d\d$");

        private static List<(string name, GridData grid)> LoadLevels()
        {
            List<(string name, GridData grid)> levels = new List<(string name, GridData grid)>();
            foreach (string guid in AssetDatabase.FindAssets("Level_", new[] { "Assets/Levels" }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Object asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
                if (asset == null || !LevelName.IsMatch(asset.name)) continue;

                SerializedObject so = new SerializedObject(asset);
                GridData grid = new GridData(so.FindProperty("width").intValue, so.FindProperty("height").intValue);
                SerializedProperty walls = so.FindProperty("walls");
                for (int i = 0; i < walls.arraySize; i++)
                {
                    SerializedProperty w = walls.GetArrayElementAtIndex(i);
                    bool hasNumber = w.FindPropertyRelative("hasNumber").boolValue;
                    CellData cell = grid.GetCell(w.FindPropertyRelative("x").intValue, w.FindPropertyRelative("y").intValue);
                    cell.Type = hasNumber ? CellType.WallNumbered : CellType.Wall;
                    cell.WallNumber = hasNumber ? w.FindPropertyRelative("number").intValue : -1;
                }
                levels.Add((asset.name, grid));
            }
            levels.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
            return levels;
        }

        [Test]
        public void ThereAreTwentyLevels()
        {
            Assert.AreEqual(20, LoadLevels().Count);
        }

        [Test]
        public void EveryLevel_HasExactlyOneSolution()
        {
            foreach ((string name, GridData grid) in LoadLevels())
                Assert.AreEqual(1, PuzzleSolver.CountSolutions(grid, 2), name);
        }

        [Test]
        public void Levels01To15_AreSolvableByDeductionAlone()
        {
            List<(string name, GridData grid)> levels = LoadLevels();
            for (int i = 0; i < 15 && i < levels.Count; i++)
                Assert.IsTrue(DeductionSolver.CanSolveByDeductionAlone(levels[i].grid), levels[i].name);
        }
    }
}
