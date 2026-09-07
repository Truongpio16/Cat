using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Cat/Level")]
public class LevelData : ScriptableObject
{
    public int width = 5;
    public int height = 5;
    public WallEntry[] walls;

    [Serializable]
    public struct WallEntry
    {
        public int x;
        public int y;
        public bool hasNumber;
        public int number;
    }
}
