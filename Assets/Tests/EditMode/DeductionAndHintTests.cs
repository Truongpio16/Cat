using Cat.Core;
using NUnit.Framework;

namespace Cat.Core.Tests
{
    public class DeductionAndHintTests
    {
        [Test]
        public void Level01Like_IsSolvableByDeductionAlone()
        {
            Assert.IsTrue(DeductionSolver.CanSolveByDeductionAlone(TestGrids.Level01Like()));
        }

        [Test]
        public void OpenTwoCellRow_IsNotSolvableByDeduction()
        {
            Assert.IsFalse(DeductionSolver.CanSolveByDeductionAlone(new GridData(2, 1)));
        }

        [Test]
        public void Hint_IsolatedCell_SaysPlaceBulb()
        {
            HintMove hint = new GridData(1, 1).FindHint();
            Assert.IsNotNull(hint);
            Assert.AreEqual(HintType.PlaceBulb, hint.Type);
            Assert.AreEqual((0, 0), (hint.X, hint.Y));
            Assert.IsNotEmpty(hint.Reason);
        }

        [Test]
        public void Hint_ForcedPlacement_PointsAtNeighbourOfFullWall()
        {
            HintMove hint = TestGrids.Level01Like().FindHint();
            Assert.IsNotNull(hint);
            Assert.AreEqual(HintType.PlaceBulb, hint.Type);
            CollectionAssert.Contains(new[] { (0, 2), (1, 1), (2, 2) }, (hint.X, hint.Y));
        }

        [Test]
        public void Hint_WallWithZero_SaysAvoidBulb()
        {
            GridData grid = TestGrids.Make(3, 3, (1, 1, 0));
            HintMove hint = grid.FindHint();
            Assert.IsNotNull(hint);
            Assert.AreEqual(HintType.AvoidBulb, hint.Type);
            CollectionAssert.Contains(new[] { (0, 1), (2, 1), (1, 0), (1, 2) }, (hint.X, hint.Y));
        }
    }
}
