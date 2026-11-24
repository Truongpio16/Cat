using Cat.Core;

namespace Cat.Core.Tests
{
    internal static class TestGrids
    {
        // n >= 0: numbered wall; n < 0: plain wall.
        public static GridData Make(int width, int height, params (int x, int y, int n)[] walls)
        {
            GridData grid = new GridData(width, height);
            foreach ((int x, int y, int n) in walls)
            {
                CellData cell = grid.GetCell(x, y);
                cell.Type = n >= 0 ? CellType.WallNumbered : CellType.Wall;
                cell.WallNumber = n >= 0 ? n : -1;
            }
            return grid;
        }

        // Same layout as Assets/Levels/Level_01.asset (3x3, unique solution = bulbs at (0,2),(1,1),(2,2)).
        public static GridData Level01Like()
        {
            return Make(3, 3, (2, 0, 0), (1, 2, 3));
        }
    }
}
