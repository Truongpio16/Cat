using Cat.Core;
using NUnit.Framework;

namespace Cat.Core.Tests
{
    public class LevelProgressTests
    {
        [Test]
        public void FirstLevel_IsAlwaysUnlocked()
        {
            Assert.IsTrue(new LevelProgress().IsUnlocked(0));
        }

        [Test]
        public void NextLevel_UnlocksOnlyAfterPreviousCompleted()
        {
            LevelProgress progress = new LevelProgress();
            Assert.IsFalse(progress.IsUnlocked(1));
            progress.MarkCompleted(0);
            Assert.IsTrue(progress.IsCompleted(0));
            Assert.IsTrue(progress.IsUnlocked(1));
            Assert.IsFalse(progress.IsUnlocked(2));
        }

        [Test]
        public void ConstructorRestoresCompletedLevels()
        {
            LevelProgress progress = new LevelProgress(new[] { 0, 1, 2 });
            Assert.IsTrue(progress.IsUnlocked(3));
            Assert.AreEqual(3, progress.CompletedLevels.Count);
        }

        [Test]
        public void MarkingTwice_DoesNotDuplicate()
        {
            LevelProgress progress = new LevelProgress();
            progress.MarkCompleted(4);
            progress.MarkCompleted(4);
            Assert.AreEqual(1, progress.CompletedLevels.Count);
        }
    }
}
