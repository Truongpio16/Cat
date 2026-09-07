using System.Collections.Generic;

namespace Cat.Core
{
    public class LevelBlueprint
    {
        public int Width;
        public int Height;
        public List<WallInfo> Walls = new List<WallInfo>();

        public class WallInfo
        {
            public int X;
            public int Y;
            public bool HasNumber;
            public int Number;
        }
    }
}
