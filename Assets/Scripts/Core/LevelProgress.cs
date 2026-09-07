using System.Collections.Generic;

namespace Cat.Core
{
    public class LevelProgress
    {
        private readonly HashSet<int> _completedLevels;

        public LevelProgress(IEnumerable<int> completedLevels = null)
        {
            _completedLevels = completedLevels != null
                ? new HashSet<int>(completedLevels)
                : new HashSet<int>();
        }

        public bool IsCompleted(int levelIndex)
        {
            return _completedLevels.Contains(levelIndex);
        }

        public bool IsUnlocked(int levelIndex)
        {
            if (levelIndex <= 0) return true;
            return IsCompleted(levelIndex - 1);
        }

        public void MarkCompleted(int levelIndex)
        {
            _completedLevels.Add(levelIndex);
        }

        public IReadOnlyCollection<int> CompletedLevels => _completedLevels;
    }
}
