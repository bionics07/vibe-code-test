using System;
using System.Collections.Generic;

namespace VibeCodeTest.Puzzle
{
    /// <summary>
    /// Finds cat placements that satisfy the three rules: one cat per row and column, one per
    /// region, and no two cats touching, diagonals included. A level's starter cat is a given.
    /// </summary>
    public static class LevelSolver
    {
        /// <summary>Returns up to <paramref name="limit"/> solutions, each the cat's column for every row.</summary>
        public static IReadOnlyList<int[]> FindSolutions(LevelDefinition level, int limit = 2)
        {
            var solutions = new List<int[]>();
            Place(level, 0, new int[level.Size], 0, 0, limit, solutions);
            return solutions;
        }

        private static void Place(LevelDefinition level, int row, int[] columns, int usedColumns, int usedRegions, int limit, List<int[]> solutions)
        {
            if (solutions.Count >= limit)
            {
                return;
            }

            if (row == level.Size)
            {
                solutions.Add((int[])columns.Clone());
                return;
            }

            for (int col = 0; col < level.Size; col++)
            {
                int columnBit = 1 << col;
                int regionBit = 1 << level.RegionAt(row, col);
                bool touchesPrevious = row > 0 && Math.Abs(col - columns[row - 1]) <= 1;
                bool skipsStarter = level.HasStarter && row == level.StarterRow && col != level.StarterCol;
                if ((usedColumns & columnBit) != 0 || (usedRegions & regionBit) != 0 || touchesPrevious || skipsStarter)
                {
                    continue;
                }

                columns[row] = col;
                Place(level, row + 1, columns, usedColumns | columnBit, usedRegions | regionBit, limit, solutions);
            }
        }
    }
}
