using System;
using System.Collections.Generic;
using UnityEngine;
using VibeCodeTest.Puzzle;

namespace VibeCodeTest.Data
{
    /// <summary>
    /// One level as plain lists of positions. Every position is (x = column, y = row), counted
    /// from zero at the top-left cell.
    /// </summary>
    [CreateAssetMenu(fileName = "Level", menuName = "Vibe Code Test/Level")]
    public sealed class LevelData : ScriptableObject
    {
        [SerializeField, Min(1)]
        private int gridSize = 4;

        [SerializeField, Tooltip("Sample cat already on the board when the level starts.")]
        private Vector2Int starterCat;

        [SerializeField]
        private List<RegionData> regions = new List<RegionData>();

        public int GridSize => gridSize;

        public Vector2Int StarterCat => starterCat;

        public IReadOnlyList<RegionData> Regions => regions;

        /// <summary>Builds the rules model. Throws when the regions do not cover each cell exactly once.</summary>
        public LevelDefinition ToDefinition()
        {
            var cells = new int[gridSize, gridSize];
            var covered = new bool[gridSize, gridSize];
            foreach (RegionData region in regions)
            {
                foreach (Vector2Int cell in region.Cells)
                {
                    if (covered[cell.y, cell.x])
                    {
                        throw new InvalidOperationException($"{name}: cell {cell} is in two regions.");
                    }

                    covered[cell.y, cell.x] = true;
                    cells[cell.y, cell.x] = (int)region.Color;
                }
            }

            foreach (bool isCovered in covered)
            {
                if (!isCovered)
                {
                    throw new InvalidOperationException($"{name}: a cell has no region.");
                }
            }

            return new LevelDefinition(cells, starterCat.y, starterCat.x);
        }

        [Serializable]
        public sealed class RegionData
        {
            [SerializeField]
            private RegionColor color;

            [SerializeField]
            private List<Vector2Int> cells = new List<Vector2Int>();

            public RegionColor Color => color;

            public IReadOnlyList<Vector2Int> Cells => cells;
        }
    }
}
