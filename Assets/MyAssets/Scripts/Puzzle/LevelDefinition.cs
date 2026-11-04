using System;

namespace VibeCodeTest.Puzzle
{
    /// <summary>A square board split into regions, with an optional cat already placed.</summary>
    public sealed class LevelDefinition
    {
        private const int NoStarter = -1;

        private readonly int[,] regions;

        public LevelDefinition(int[,] regions, int starterRow = NoStarter, int starterCol = NoStarter)
        {
            if (regions.GetLength(0) != regions.GetLength(1))
            {
                throw new ArgumentException("A level must be square.");
            }

            this.regions = regions;
            Size = regions.GetLength(0);
            StarterRow = starterRow;
            StarterCol = starterCol;
        }

        public int Size { get; }

        public int StarterRow { get; }

        public int StarterCol { get; }

        public bool HasStarter => StarterRow != NoStarter;

        /// <summary>Builds a level from text rows, one letter per region (A = region 0).</summary>
        public static LevelDefinition FromRows(string[] rows, int starterRow = NoStarter, int starterCol = NoStarter)
        {
            var regions = new int[rows.Length, rows.Length];
            for (int row = 0; row < rows.Length; row++)
            {
                if (rows[row].Length != rows.Length)
                {
                    throw new ArgumentException($"Row {row} has {rows[row].Length} cells, expected {rows.Length}.");
                }

                for (int col = 0; col < rows.Length; col++)
                {
                    regions[row, col] = rows[row][col] - 'A';
                }
            }

            return new LevelDefinition(regions, starterRow, starterCol);
        }

        public int RegionAt(int row, int col)
        {
            return regions[row, col];
        }
    }
}
