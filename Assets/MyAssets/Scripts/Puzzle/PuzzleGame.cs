namespace VibeCodeTest.Puzzle
{
    public enum CellState
    {
        Empty,
        Marked,
        Cat,
        Mistake,
    }

    public enum PuzzleStatus
    {
        Playing,
        Won,
        Lost,
    }

    /// <summary>
    /// Rules of one play of a level. A single tap toggles the X on a cell. A double tap attempts a
    /// cat: a cell outside the solution becomes a locked mistake and costs a life.
    /// </summary>
    public sealed class PuzzleGame
    {
        private const int NoCell = -1;

        private readonly float doubleTapWindow;
        private readonly int[] solution;
        private readonly CellState[,] cells;

        private int lastTapRow = NoCell;
        private int lastTapCol = NoCell;
        private float lastTapTime;

        public PuzzleGame(LevelDefinition level, int startingLives, float doubleTapWindow)
        {
            Level = level;
            StartingLives = startingLives;
            Lives = startingLives;
            this.doubleTapWindow = doubleTapWindow;
            solution = LevelSolver.FindSolutions(level, 1)[0];
            cells = new CellState[level.Size, level.Size];

            if (level.HasStarter)
            {
                cells[level.StarterRow, level.StarterCol] = CellState.Cat;
                CatsPlaced = 1;
            }
        }

        public LevelDefinition Level { get; }

        public int StartingLives { get; }

        public int Lives { get; private set; }

        public int CatsPlaced { get; private set; }

        public int CatsRequired => Level.Size;

        public PuzzleStatus Status { get; private set; } = PuzzleStatus.Playing;

        public CellState CellAt(int row, int col)
        {
            return cells[row, col];
        }

        /// <param name="time">Current time in seconds, used to detect double taps.</param>
        public void Tap(int row, int col, float time)
        {
            CellState state = cells[row, col];
            if (Status != PuzzleStatus.Playing || state == CellState.Cat || state == CellState.Mistake)
            {
                return;
            }

            if (IsSecondTapOfDouble(row, col, time))
            {
                lastTapRow = NoCell;
                AttemptCat(row, col);
                return;
            }

            lastTapRow = row;
            lastTapCol = col;
            lastTapTime = time;
            cells[row, col] = state == CellState.Empty ? CellState.Marked : CellState.Empty;
        }

        private bool IsSecondTapOfDouble(int row, int col, float time)
        {
            return row == lastTapRow && col == lastTapCol && time - lastTapTime <= doubleTapWindow;
        }

        private void AttemptCat(int row, int col)
        {
            if (solution[row] == col)
            {
                cells[row, col] = CellState.Cat;
                CatsPlaced++;
                if (CatsPlaced == CatsRequired)
                {
                    Status = PuzzleStatus.Won;
                }

                return;
            }

            cells[row, col] = CellState.Mistake;
            Lives--;
            if (Lives == 0)
            {
                Status = PuzzleStatus.Lost;
            }
        }
    }
}
