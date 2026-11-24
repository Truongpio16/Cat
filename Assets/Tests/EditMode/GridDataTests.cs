using Cat.Core;
using NUnit.Framework;

namespace Cat.Core.Tests
{
    public class GridDataTests
    {
        [Test]
        public void NewGrid_IsAllEmptyWithoutBulbs()
        {
            GridData grid = new GridData(3, 2);
            Assert.AreEqual(3, grid.Width);
            Assert.AreEqual(2, grid.Height);
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 2; y++)
                {
                    Assert.AreEqual(CellType.Empty, grid.GetCell(x, y).Type);
                    Assert.IsFalse(grid.GetCell(x, y).HasBulb);
                }
        }

        [Test]
        public void IsInBounds_RejectsOutside()
        {
            GridData grid = new GridData(3, 3);
            Assert.IsTrue(grid.IsInBounds(0, 0));
            Assert.IsTrue(grid.IsInBounds(2, 2));
            Assert.IsFalse(grid.IsInBounds(-1, 0));
            Assert.IsFalse(grid.IsInBounds(0, 3));
        }

        [Test]
        public void Bulb_LightsItsRowAndColumnOnly()
        {
            GridData grid = new GridData(3, 3);
            grid.ToggleBulb(1, 1);
            grid.RecalculateLight();
            Assert.IsTrue(grid.GetCell(1, 1).IsLit);
            Assert.IsTrue(grid.GetCell(0, 1).IsLit);
            Assert.IsTrue(grid.GetCell(2, 1).IsLit);
            Assert.IsTrue(grid.GetCell(1, 0).IsLit);
            Assert.IsTrue(grid.GetCell(1, 2).IsLit);
            Assert.IsFalse(grid.GetCell(0, 0).IsLit);
            Assert.IsFalse(grid.GetCell(2, 2).IsLit);
        }

        [Test]
        public void Wall_BlocksLight()
        {
            GridData grid = TestGrids.Make(5, 1, (2, 0, -1));
            grid.ToggleBulb(0, 0);
            grid.RecalculateLight();
            Assert.IsTrue(grid.GetCell(1, 0).IsLit);
            Assert.IsFalse(grid.GetCell(3, 0).IsLit);
            Assert.IsFalse(grid.GetCell(4, 0).IsLit);
        }

        [Test]
        public void TwoBulbsSeeingEachOther_AreBothInConflict()
        {
            GridData grid = new GridData(3, 1);
            grid.ToggleBulb(0, 0);
            grid.ToggleBulb(2, 0);
            grid.RecalculateLight();
            Assert.IsTrue(grid.GetCell(0, 0).HasConflict);
            Assert.IsTrue(grid.GetCell(2, 0).HasConflict);
            Assert.IsFalse(grid.CheckWin());
        }

        [Test]
        public void ToggleBulb_OnWall_DoesNothing()
        {
            GridData grid = TestGrids.Make(2, 1, (0, 0, -1));
            grid.ToggleBulb(0, 0);
            Assert.IsFalse(grid.GetCell(0, 0).HasBulb);
        }

        [Test]
        public void ToggleBulb_TwiceRemovesBulb()
        {
            GridData grid = new GridData(2, 1);
            grid.ToggleBulb(0, 0);
            grid.ToggleBulb(0, 0);
            Assert.IsFalse(grid.GetCell(0, 0).HasBulb);
        }

        [Test]
        public void CheckWin_FalseWhenCellsUnlit()
        {
            GridData grid = TestGrids.Level01Like();
            grid.RecalculateLight();
            Assert.IsFalse(grid.CheckWin());
        }

        [Test]
        public void CheckWin_TrueOnKnownSolution()
        {
            GridData grid = TestGrids.Level01Like();
            grid.ToggleBulb(0, 2);
            grid.ToggleBulb(1, 1);
            grid.ToggleBulb(2, 2);
            grid.RecalculateLight();
            Assert.IsTrue(grid.CheckWin());
        }

        [Test]
        public void CheckWin_FalseWhenNumberedWallCountIsWrong()
        {
            // Everything lit with no conflicts, but wall (2,0) demands 0 adjacent bulbs and has one at (2,1).
            GridData grid = TestGrids.Make(3, 3, (2, 0, 0));
            grid.ToggleBulb(2, 1);
            grid.ToggleBulb(0, 0);
            grid.ToggleBulb(1, 2);
            grid.RecalculateLight();
            Assert.IsFalse(grid.CheckWin());
        }
    }
}
