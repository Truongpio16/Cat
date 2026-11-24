using System.Collections.Generic;
using Cat.Core;
using NUnit.Framework;

namespace Cat.Core.Tests
{
    public class PuzzleSolverTests
    {
        [Test]
        public void Level01Like_HasExactlyOneSolution()
        {
            GridData grid = TestGrids.Level01Like();
            Assert.AreEqual(1, PuzzleSolver.CountSolutions(grid, 2));
            Assert.IsTrue(PuzzleSolver.HasUniqueSolution(grid));
        }

        [Test]
        public void Level01Like_SolutionIsTheThreeNeighboursOfTheWall()
        {
            GridData grid = TestGrids.Level01Like();
            Assert.IsTrue(PuzzleSolver.TryFindOneSolution(grid, out List<(int x, int y)> bulbs));
            CollectionAssert.AreEquivalent(new[] { (0, 2), (1, 1), (2, 2) }, bulbs);
        }

        [Test]
        public void Solver_LeavesGridWithoutBulbs()
        {
            GridData grid = TestGrids.Level01Like();
            PuzzleSolver.FindSolutions(grid, 2);
            for (int x = 0; x < grid.Width; x++)
                for (int y = 0; y < grid.Height; y++)
                    Assert.IsFalse(grid.GetCell(x, y).HasBulb, $"bulb left at ({x},{y})");
        }

        [Test]
        public void OpenTwoCellRow_HasTwoSolutions_NotUnique()
        {
            GridData grid = new GridData(2, 1);
            Assert.AreEqual(2, PuzzleSolver.CountSolutions(grid, 5));
            Assert.IsFalse(PuzzleSolver.HasUniqueSolution(grid));
        }

        [Test]
        public void Limit_CapsNumberOfSolutionsFound()
        {
            GridData grid = new GridData(2, 1);
            Assert.AreEqual(1, PuzzleSolver.CountSolutions(grid, 1));
        }

        [Test]
        public void ImpossibleNumber_HasNoSolution()
        {
            // The wall wants 3 neighbouring bulbs but has only 2 neighbours.
            GridData grid = TestGrids.Make(3, 1, (1, 0, 3));
            Assert.AreEqual(0, PuzzleSolver.CountSolutions(grid, 2));
            Assert.IsFalse(PuzzleSolver.TryFindOneSolution(grid, out List<(int x, int y)> bulbs));
            Assert.IsNull(bulbs);
        }
    }
}
