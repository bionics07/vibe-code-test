using NUnit.Framework;
using VibeCodeTest.UI;

namespace VibeCodeTest.Tests
{
    public sealed class BoardLayoutTests
    {
        private const float Gap = 0.13f;
        private const float Padding = 0.22f;
        private const float Tolerance = 0.0001f;

        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(8)]
        public void Cells_FillTheBoardWithEqualPaddingOnBothSides(int size)
        {
            float cell = BoardLayout.CellFraction(size, Gap, Padding);
            float lastEnd = BoardLayout.CellStart(size - 1, size, Gap, Padding) + cell;

            Assert.AreEqual(Padding * cell, BoardLayout.CellStart(0, size, Gap, Padding), Tolerance);
            Assert.AreEqual(1f - (Padding * cell), lastEnd, Tolerance);
        }

        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(8)]
        public void Gap_StaysProportionalToTheCell(int size)
        {
            float cell = BoardLayout.CellFraction(size, Gap, Padding);
            float step = BoardLayout.CellStart(1, size, Gap, Padding) - BoardLayout.CellStart(0, size, Gap, Padding);

            Assert.AreEqual(Gap * cell, step - cell, Tolerance);
        }

        // Cell widths measured on the 480-wide board of the screenshots, in the same pixels.
        [TestCase(4, 100f)]
        [TestCase(5, 80f)]
        [TestCase(6, 67f)]
        [TestCase(8, 52f)]
        public void CellSize_MatchesTheReferenceGame(int size, float measuredCell)
        {
            const float boardWidth = 480f;
            const float measurementError = 2f;

            Assert.AreEqual(measuredCell, BoardLayout.CellFraction(size, Gap, Padding) * boardWidth, measurementError);
        }
    }
}
