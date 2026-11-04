using NUnit.Framework;
using VibeCodeTest.Puzzle;

namespace VibeCodeTest.Tests
{
    public sealed class PuzzleGameTests
    {
        private const int Lives = 3;
        private const float Window = 0.4f;
        private const float SlowGap = Window + 1f;
        private const float QuickGap = Window / 2f;
        private const int StarterRow = 2;
        private const int StarterCol = 3;

        // Unique solution by row: columns 2, 0, 3, 1. The cat in row 2 starts on the board.
        private static readonly string[] Rows = { "AABA", "CAAA", "AADD", "AAAD" };
        private static readonly int[] Solution = { 2, 0, 3, 1 };

        private PuzzleGame game;
        private float clock;

        [SetUp]
        public void SetUp()
        {
            clock = 0f;
            game = new PuzzleGame(LevelDefinition.FromRows(Rows, StarterRow, StarterCol), Lives, Window);
        }

        [Test]
        public void NewGame_StartsPlayingWithAllLivesAndTheStarterCat()
        {
            Assert.AreEqual(PuzzleStatus.Playing, game.Status);
            Assert.AreEqual(Lives, game.Lives);
            Assert.AreEqual(1, game.CatsPlaced);
            Assert.AreEqual(4, game.CatsRequired);
            Assert.AreEqual(CellState.Cat, game.CellAt(StarterRow, StarterCol));
            Assert.AreEqual(CellState.Empty, game.CellAt(0, 0));
        }

        [Test]
        public void LevelWithoutStarter_StartsWithNoCats()
        {
            var empty = new PuzzleGame(LevelDefinition.FromRows(Rows), Lives, Window);

            Assert.AreEqual(0, empty.CatsPlaced);
        }

        [Test]
        public void StartingLives_ComeFromTheCaller()
        {
            var game = new PuzzleGame(LevelDefinition.FromRows(Rows, StarterRow, StarterCol), 5, Window);

            Assert.AreEqual(5, game.Lives);
            Assert.AreEqual(5, game.StartingLives);
        }

        [Test]
        public void TapOnStarterCat_ChangesNothingAtAnySpeed()
        {
            QuickTap(StarterRow, StarterCol);
            SlowTap(StarterRow, StarterCol);
            DoubleTap(StarterRow, StarterCol);

            Assert.AreEqual(CellState.Cat, game.CellAt(StarterRow, StarterCol));
            Assert.AreEqual(1, game.CatsPlaced);
            Assert.AreEqual(Lives, game.Lives);
        }

        [Test]
        public void SingleTap_MarksWithoutCostingALife()
        {
            SlowTap(0, 0);

            Assert.AreEqual(CellState.Marked, game.CellAt(0, 0));
            Assert.AreEqual(Lives, game.Lives);
        }

        [Test]
        public void SlowSecondTap_RemovesTheMarkEvenOnASolutionCell()
        {
            SlowTap(0, Solution[0]);
            SlowTap(0, Solution[0]);

            Assert.AreEqual(CellState.Empty, game.CellAt(0, Solution[0]));
            Assert.AreEqual(1, game.CatsPlaced);
            Assert.AreEqual(Lives, game.Lives);
        }

        [Test]
        public void SlowThirdTap_MarksAgain()
        {
            SlowTap(0, 0);
            SlowTap(0, 0);
            SlowTap(0, 0);

            Assert.AreEqual(CellState.Marked, game.CellAt(0, 0));
        }

        [Test]
        public void TapsExactlyAtTheWindow_CountAsDouble()
        {
            game.Tap(0, 0, 10f);
            game.Tap(0, 0, 10f + Window);

            Assert.AreEqual(CellState.Mistake, game.CellAt(0, 0));
        }

        [Test]
        public void TapsJustPastTheWindow_RemoveTheMark()
        {
            game.Tap(0, 0, 10f);
            game.Tap(0, 0, 10f + Window + 0.01f);

            Assert.AreEqual(CellState.Empty, game.CellAt(0, 0));
            Assert.AreEqual(Lives, game.Lives);
        }

        [Test]
        public void WindowFromTheCaller_DecidesWhatIsADoubleTap()
        {
            var patient = new PuzzleGame(LevelDefinition.FromRows(Rows, StarterRow, StarterCol), Lives, 5f);

            patient.Tap(0, 0, 10f);
            patient.Tap(0, 0, 14f);

            Assert.AreEqual(CellState.Mistake, patient.CellAt(0, 0));
        }

        [Test]
        public void DoubleTapOnSolutionCell_PlacesCat()
        {
            DoubleTap(0, Solution[0]);

            Assert.AreEqual(CellState.Cat, game.CellAt(0, Solution[0]));
            Assert.AreEqual(2, game.CatsPlaced);
            Assert.AreEqual(Lives, game.Lives);
        }

        [Test]
        public void DoubleTapOnWrongCell_LocksMistakeAndCostsALife()
        {
            DoubleTap(0, 0);

            Assert.AreEqual(CellState.Mistake, game.CellAt(0, 0));
            Assert.AreEqual(1, game.CatsPlaced);
            Assert.AreEqual(Lives - 1, game.Lives);
        }

        [Test]
        public void DoubleTapOnExistingMark_StillPlacesCat()
        {
            SlowTap(0, Solution[0]);

            DoubleTap(0, Solution[0]);

            Assert.AreEqual(CellState.Cat, game.CellAt(0, Solution[0]));
        }

        [Test]
        public void DoubleTapOnExistingMark_StillRegistersMistake()
        {
            SlowTap(0, 0);

            DoubleTap(0, 0);

            Assert.AreEqual(CellState.Mistake, game.CellAt(0, 0));
            Assert.AreEqual(Lives - 1, game.Lives);
        }

        [Test]
        public void QuickTapsOnDifferentCells_AreNotADoubleTap()
        {
            QuickTap(0, 0);
            QuickTap(0, 1);
            QuickTap(0, 0);

            Assert.AreEqual(CellState.Empty, game.CellAt(0, 0));
            Assert.AreEqual(CellState.Marked, game.CellAt(0, 1));
            Assert.AreEqual(Lives, game.Lives);
        }

        [Test]
        public void TapOnMistake_ChangesNothingAtAnySpeed()
        {
            DoubleTap(0, 0);

            QuickTap(0, 0);
            SlowTap(0, 0);
            DoubleTap(0, 0);

            Assert.AreEqual(CellState.Mistake, game.CellAt(0, 0));
            Assert.AreEqual(Lives - 1, game.Lives);
        }

        [Test]
        public void TapOnPlacedCat_ChangesNothingAtAnySpeed()
        {
            DoubleTap(0, Solution[0]);

            QuickTap(0, Solution[0]);
            SlowTap(0, Solution[0]);
            DoubleTap(0, Solution[0]);

            Assert.AreEqual(CellState.Cat, game.CellAt(0, Solution[0]));
            Assert.AreEqual(2, game.CatsPlaced);
        }

        [Test]
        public void ThreeMistakes_LoseTheLevel()
        {
            DoubleTap(0, 0);
            DoubleTap(0, 1);
            Assert.AreEqual(PuzzleStatus.Playing, game.Status);

            DoubleTap(0, 3);

            Assert.AreEqual(0, game.Lives);
            Assert.AreEqual(PuzzleStatus.Lost, game.Status);
        }

        [Test]
        public void AfterLosing_TapsAreIgnored()
        {
            DoubleTap(0, 0);
            DoubleTap(0, 1);
            DoubleTap(0, 3);

            SlowTap(3, 3);
            DoubleTap(0, Solution[0]);

            Assert.AreEqual(CellState.Empty, game.CellAt(3, 3));
            Assert.AreEqual(1, game.CatsPlaced);
        }

        [Test]
        public void PlacingEveryCat_WinsTheLevel()
        {
            SolveAll();

            Assert.AreEqual(PuzzleStatus.Won, game.Status);
            Assert.AreEqual(4, game.CatsPlaced);
        }

        [Test]
        public void WinningAfterTwoMistakes_StillWins()
        {
            DoubleTap(0, 0);
            DoubleTap(0, 1);
            SolveAll();

            Assert.AreEqual(PuzzleStatus.Won, game.Status);
            Assert.AreEqual(1, game.Lives);
        }

        [Test]
        public void AfterWinning_TapsAreIgnored()
        {
            SolveAll();

            SlowTap(0, 0);

            Assert.AreEqual(CellState.Empty, game.CellAt(0, 0));
            Assert.AreEqual(PuzzleStatus.Won, game.Status);
        }

        private void SolveAll()
        {
            for (int row = 0; row < Solution.Length; row++)
            {
                DoubleTap(row, Solution[row]);
            }
        }

        private void SlowTap(int row, int col)
        {
            clock += SlowGap;
            game.Tap(row, col, clock);
        }

        private void QuickTap(int row, int col)
        {
            clock += QuickGap;
            game.Tap(row, col, clock);
        }

        private void DoubleTap(int row, int col)
        {
            SlowTap(row, col);
            QuickTap(row, col);
        }
    }
}
