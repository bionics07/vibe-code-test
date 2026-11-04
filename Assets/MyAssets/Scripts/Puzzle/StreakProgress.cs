namespace VibeCodeTest.Puzzle
{
    /// <summary>The prototype's fake streak: completing one chosen level grants a single day, once.</summary>
    public sealed class StreakProgress
    {
        private readonly int celebratedLevelIndex;

        public StreakProgress(int celebratedLevelIndex)
        {
            this.celebratedLevelIndex = celebratedLevelIndex;
        }

        public int Days { get; private set; }

        /// <summary>Records a completed level and returns whether it earned the streak just now.</summary>
        public bool TryCelebrate(int completedLevelIndex)
        {
            if (Days > 0 || completedLevelIndex != celebratedLevelIndex)
            {
                return false;
            }

            Days = 1;
            return true;
        }
    }
}
