namespace VibeCodeTest.UI
{
    /// <summary>
    /// Board geometry as fractions of the board side. The board keeps its size on every level:
    /// cells shrink as the grid grows, and gap and padding stay proportional to the cell, as in
    /// the reference game.
    /// </summary>
    public static class BoardLayout
    {
        public static float CellFraction(int gridSize, float gapRatio, float paddingRatio)
        {
            return 1f / (gridSize + ((gridSize - 1) * gapRatio) + (2f * paddingRatio));
        }

        public static float CellStart(int index, int gridSize, float gapRatio, float paddingRatio)
        {
            return (paddingRatio + (index * (1f + gapRatio))) * CellFraction(gridSize, gapRatio, paddingRatio);
        }
    }
}
