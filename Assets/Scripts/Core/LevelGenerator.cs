using System.Collections.Generic;

namespace Cat.Core
{
    public static class LevelGenerator
    {
        public static bool TryGenerate(
            int width,
            int height,
            int wallCount,
            System.Random rng,
            int maxAttempts,
            out LevelBlueprint result,
            bool requireDeductionSolvable = false)
        {
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                if (TryGenerateOnce(width, height, wallCount, rng, out result, requireDeductionSolvable))
                {
                    return true;
                }
            }
            result = null;
            return false;
        }

        private static bool TryGenerateOnce(
            int width,
            int height,
            int wallCount,
            System.Random rng,
            out LevelBlueprint result,
            bool requireDeductionSolvable)
        {
            result = null;

            List<(int x, int y)> allCells = new List<(int, int)>();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    allCells.Add((x, y));
                }
            }
            Shuffle(allCells, rng);

            int count = wallCount < allCells.Count ? wallCount : allCells.Count;
            List<(int x, int y)> wallPositions = allCells.GetRange(0, count);

            GridData grid = new GridData(width, height);
            foreach ((int x, int y) in wallPositions)
            {
                grid.GetCell(x, y).Type = CellType.Wall;
            }

            if (!PuzzleSolver.TryFindOneSolution(grid, out List<(int x, int y)> solutionBulbs))
            {
                return false;
            }

            HashSet<(int, int)> bulbSet = new HashSet<(int, int)>(solutionBulbs);
            List<LevelBlueprint.WallInfo> walls = new List<LevelBlueprint.WallInfo>();

            foreach ((int x, int y) in wallPositions)
            {
                int number = CountAdjacentBulbs(x, y, bulbSet);
                grid.GetCell(x, y).Type = CellType.WallNumbered;
                grid.GetCell(x, y).WallNumber = number;
                walls.Add(new LevelBlueprint.WallInfo { X = x, Y = y, HasNumber = true, Number = number });
            }

            if (!PuzzleSolver.HasUniqueSolution(grid))
            {
                return false;
            }

            if (requireDeductionSolvable)
            {
                GridData deductionCheck = new GridData(width, height);
                foreach (LevelBlueprint.WallInfo w in walls)
                {
                    CellData cell = deductionCheck.GetCell(w.X, w.Y);
                    cell.Type = CellType.WallNumbered;
                    cell.WallNumber = w.Number;
                }

                if (!DeductionSolver.CanSolveByDeductionAlone(deductionCheck))
                {
                    return false;
                }
            }

            result = new LevelBlueprint { Width = width, Height = height, Walls = walls };
            return true;
        }

        private static int CountAdjacentBulbs(int x, int y, HashSet<(int, int)> bulbSet)
        {
            int count = 0;
            if (bulbSet.Contains((x + 1, y))) count++;
            if (bulbSet.Contains((x - 1, y))) count++;
            if (bulbSet.Contains((x, y + 1))) count++;
            if (bulbSet.Contains((x, y - 1))) count++;
            return count;
        }

        private static void Shuffle(List<(int, int)> list, System.Random rng)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                ((int, int) a, (int, int) b) = (list[i], list[j]);
                list[i] = b;
                list[j] = a;
            }
        }
    }
}
