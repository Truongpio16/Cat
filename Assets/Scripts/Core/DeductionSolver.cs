using System.Collections.Generic;

namespace Cat.Core
{
    public static class DeductionSolver
    {
        public static bool CanSolveByDeductionAlone(GridData grid)
        {
            HashSet<(int, int)> confirmedNoBulb = new HashSet<(int, int)>();
            bool progress = true;

            while (progress)
            {
                progress = false;

                if (ApplyIsolatedCellRule(grid, confirmedNoBulb)) progress = true;
                if (ApplyWallCountingRule(grid, confirmedNoBulb)) progress = true;
            }

            grid.RecalculateLight();
            return grid.CheckWin();
        }

        private static bool ApplyIsolatedCellRule(GridData grid, HashSet<(int, int)> confirmedNoBulb)
        {
            bool progress = false;

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    CellData cell = grid.GetCell(x, y);
                    if (cell.Type != CellType.Empty || cell.HasBulb) continue;
                    if (confirmedNoBulb.Contains((x, y))) continue;

                    bool leftBlocked = !IsOpenEmpty(grid, x - 1, y);
                    bool rightBlocked = !IsOpenEmpty(grid, x + 1, y);
                    bool upBlocked = !IsOpenEmpty(grid, x, y - 1);
                    bool downBlocked = !IsOpenEmpty(grid, x, y + 1);

                    if (leftBlocked && rightBlocked && upBlocked && downBlocked)
                    {
                        cell.HasBulb = true;
                        progress = true;
                    }
                }
            }

            return progress;
        }

        private static bool ApplyWallCountingRule(GridData grid, HashSet<(int, int)> confirmedNoBulb)
        {
            bool progress = false;

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    CellData wall = grid.GetCell(x, y);
                    if (wall.Type != CellType.WallNumbered) continue;

                    int bulbCount = 0;
                    List<(int, int)> candidates = new List<(int, int)>();
                    CollectNeighbor(grid, x + 1, y, confirmedNoBulb, ref bulbCount, candidates);
                    CollectNeighbor(grid, x - 1, y, confirmedNoBulb, ref bulbCount, candidates);
                    CollectNeighbor(grid, x, y + 1, confirmedNoBulb, ref bulbCount, candidates);
                    CollectNeighbor(grid, x, y - 1, confirmedNoBulb, ref bulbCount, candidates);

                    int needed = wall.WallNumber - bulbCount;

                    if (needed > 0 && needed == candidates.Count)
                    {
                        foreach ((int cx, int cy) in candidates)
                        {
                            grid.GetCell(cx, cy).HasBulb = true;
                        }
                        progress = true;
                    }
                    else if (needed == 0 && candidates.Count > 0)
                    {
                        foreach ((int cx, int cy) in candidates)
                        {
                            confirmedNoBulb.Add((cx, cy));
                        }
                        progress = true;
                    }
                }
            }

            return progress;
        }

        private static bool IsOpenEmpty(GridData grid, int x, int y)
        {
            return grid.IsInBounds(x, y) && grid.GetCell(x, y).Type == CellType.Empty;
        }

        private static void CollectNeighbor(
            GridData grid,
            int x,
            int y,
            HashSet<(int, int)> confirmedNoBulb,
            ref int bulbCount,
            List<(int, int)> candidates)
        {
            if (!IsOpenEmpty(grid, x, y)) return;

            CellData cell = grid.GetCell(x, y);
            if (cell.HasBulb)
            {
                bulbCount++;
            }
            else if (!confirmedNoBulb.Contains((x, y)))
            {
                candidates.Add((x, y));
            }
        }
    }
}
