using Cat.Core;
using NUnit.Framework;

namespace Cat.Core.Tests
{
    public class LevelGeneratorTests
    {
        private static GridData ToGrid(LevelBlueprint bp)
        {
            GridData grid = new GridData(bp.Width, bp.Height);
            foreach (LevelBlueprint.WallInfo w in bp.Walls)
            {
                CellData cell = grid.GetCell(w.X, w.Y);
                cell.Type = w.HasNumber ? CellType.WallNumbered : CellType.Wall;
                cell.WallNumber = w.HasNumber ? w.Number : -1;
            }
            return grid;
        }

        [Test]
        public void Generated5x5_IsUniqueAndDeductionSolvable()
        {
            Assert.IsTrue(LevelGenerator.TryGenerate(5, 5, 6, new System.Random(12345), 500, out LevelBlueprint bp, true));
            Assert.AreEqual(5, bp.Width);
            Assert.AreEqual(5, bp.Height);
            Assert.AreEqual(6, bp.Walls.Count);
            Assert.IsTrue(PuzzleSolver.HasUniqueSolution(ToGrid(bp)));
            Assert.IsTrue(DeductionSolver.CanSolveByDeductionAlone(ToGrid(bp)));
        }

        [Test]
        public void SameSeed_GivesSameLevel()
        {
            Assert.IsTrue(LevelGenerator.TryGenerate(5, 5, 6, new System.Random(777), 500, out LevelBlueprint a, true));
            Assert.IsTrue(LevelGenerator.TryGenerate(5, 5, 6, new System.Random(777), 500, out LevelBlueprint b, true));
            Assert.AreEqual(a.Walls.Count, b.Walls.Count);
            for (int i = 0; i < a.Walls.Count; i++)
            {
                Assert.AreEqual(a.Walls[i].X, b.Walls[i].X);
                Assert.AreEqual(a.Walls[i].Y, b.Walls[i].Y);
                Assert.AreEqual(a.Walls[i].Number, b.Walls[i].Number);
            }
        }
    }
}
