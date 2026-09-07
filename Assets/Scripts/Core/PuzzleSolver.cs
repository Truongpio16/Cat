using System.Collections.Generic;

namespace Cat.Core
{
    public static class PuzzleSolver
    {
        public static List<List<(int x, int y)>> FindSolutions(GridData grid, int limit)
        {
            List<(int x, int y)> emptyCells = CollectEmptyCells(grid);
            List<List<(int, int)>> solutions = new List<List<(int, int)>>();
            int count = 0;

            Backtrack(grid, emptyCells, 0, limit, ref count, solutions);
            ResetBulbs(grid, emptyCells);

            return solutions;
        }

        public static int CountSolutions(GridData grid, int limit)
        {
            return FindSolutions(grid, limit).Count;
        }

        public static bool HasUniqueSolution(GridData grid)
        {
            return CountSolutions(grid, 2) == 1;
        }

        public static bool TryFindOneSolution(GridData grid, out List<(int x, int y)> bulbPositions)
        {
            List<List<(int, int)>> solutions = FindSolutions(grid, 1);
            if (solutions.Count == 0)
            {
                bulbPositions = null;
                return false;
            }

            bulbPositions = solutions[0];
            return true;
        }

        private static void Backtrack(
            GridData grid,
            List<(int x, int y)> cells,
            int index,
            int limit,
            ref int count,
            List<List<(int, int)>> foundSolutions)
        {
            if (count >= limit) return;

            if (index == cells.Count)
            {
                grid.RecalculateLight();
                if (grid.CheckWin())
                {
                    count++;
                    List<(int, int)> solutionBulbs = new List<(int, int)>();
                    foreach ((int x, int y) cell in cells)
                    {
                        if (grid.GetCell(cell.x, cell.y).HasBulb)
                        {
                            solutionBulbs.Add(cell);
                        }
                    }
                    foundSolutions.Add(solutionBulbs);
                }
                return;
            }

            (int x, int y) = cells[index];

            Backtrack(grid, cells, index + 1, limit, ref count, foundSolutions);
            if (count >= limit) return;

            grid.GetCell(x, y).HasBulb = true;
            if (!HasSightConflict(grid, x, y) && IsPartiallyValid(grid))
            {
                Backtrack(grid, cells, index + 1, limit, ref count, foundSolutions);
            }
            grid.GetCell(x, y).HasBulb = false;
        }

        private static bool HasSightConflict(GridData grid, int x, int y)
        {
            return SeesBulb(grid, x, y, 1, 0)
                || SeesBulb(grid, x, y, -1, 0)
                || SeesBulb(grid, x, y, 0, 1)
                || SeesBulb(grid, x, y, 0, -1);
        }

        private static bool SeesBulb(GridData grid, int x, int y, int dx, int dy)
        {
            int cx = x + dx;
            int cy = y + dy;

            while (grid.IsInBounds(cx, cy) && grid.GetCell(cx, cy).Type == CellType.Empty)
            {
                if (grid.GetCell(cx, cy).HasBulb) return true;
                cx += dx;
                cy += dy;
            }

            return false;
        }

        private static bool IsPartiallyValid(GridData grid)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    CellData cell = grid.GetCell(x, y);
                    if (cell.Type != CellType.WallNumbered) continue;
                    if (AdjacentBulbCount(grid, x, y) > cell.WallNumber) return false;
                }
            }
            return true;
        }

        private static int AdjacentBulbCount(GridData grid, int x, int y)
        {
            int count = 0;
            count += HasBulbAt(grid, x + 1, y);
            count += HasBulbAt(grid, x - 1, y);
            count += HasBulbAt(grid, x, y + 1);
            count += HasBulbAt(grid, x, y - 1);
            return count;
        }

        private static int HasBulbAt(GridData grid, int x, int y)
        {
            if (!grid.IsInBounds(x, y)) return 0;
            return grid.GetCell(x, y).HasBulb ? 1 : 0;
        }

        private static List<(int, int)> CollectEmptyCells(GridData grid)
        {
            List<(int, int)> cells = new List<(int, int)>();
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    if (grid.GetCell(x, y).Type == CellType.Empty)
                    {
                        cells.Add((x, y));
                    }
                }
            }
            return cells;
        }

        private static void ResetBulbs(GridData grid, List<(int, int)> cells)
        {
            foreach ((int x, int y) in cells)
            {
                grid.GetCell(x, y).HasBulb = false;
            }
        }
    }
}
