using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using VibeCodeTest.Data;
using VibeCodeTest.Puzzle;

namespace VibeCodeTest.Tests
{
    /// <summary>Checks the shipped assets against the reference game's first five levels.</summary>
    public sealed class LevelDataTests
    {
        private const string ConfigPath = "Assets/MyAssets/Config/GameConfig.asset";

        // Cat columns by row, read from the final screenshot of each level.
        private static readonly int[][] ScreenshotSolutions =
        {
            new[] { 2, 0, 3, 1 },
            new[] { 2, 4, 1, 3, 0 },
            new[] { 4, 0, 3, 1, 5, 2 },
            new[] { 5, 3, 0, 2, 4, 1 },
            new[] { 7, 5, 3, 6, 0, 2, 4, 1 },
        };

        // Sample cat (column, row), read from the first screenshot of each level.
        private static readonly Vector2Int[] ScreenshotStarters =
        {
            new Vector2Int(3, 2),
            new Vector2Int(2, 0),
            new Vector2Int(0, 1),
            new Vector2Int(5, 0),
            new Vector2Int(7, 0),
        };

        private static GameConfig Config => AssetDatabase.LoadAssetAtPath<GameConfig>(ConfigPath);

        private static IEnumerable<int> LevelIndices => Enumerable.Range(0, ScreenshotSolutions.Length);

        [Test]
        public void Levels_HaveTheGridSizesOfTheReferenceGame()
        {
            CollectionAssert.AreEqual(new[] { 4, 5, 6, 6, 8 }, Config.Levels.Select(level => level.GridSize));
        }

        [Test]
        public void Config_HasAColorForEveryRegion()
        {
            foreach (RegionColor color in System.Enum.GetValues(typeof(RegionColor)))
            {
                Assert.DoesNotThrow(() => Config.ColorOf(color));
            }
        }

        [TestCaseSource(nameof(LevelIndices))]
        public void Level_RegionsCoverEveryCellOnce(int index)
        {
            Assert.DoesNotThrow(() => Config.Levels[index].ToDefinition());
        }

        [TestCaseSource(nameof(LevelIndices))]
        public void Level_HasOneRegionPerRow(int index)
        {
            LevelData level = Config.Levels[index];

            Assert.AreEqual(level.GridSize, level.Regions.Count);
            Assert.AreEqual(level.GridSize, level.Regions.Select(region => region.Color).Distinct().Count());
        }

        [TestCaseSource(nameof(LevelIndices))]
        public void Level_RegionsAreConnected(int index)
        {
            foreach (LevelData.RegionData region in Config.Levels[index].Regions)
            {
                Assert.AreEqual(region.Cells.Count, CountReachable(region.Cells), $"Region {region.Color} is split.");
            }
        }

        [TestCaseSource(nameof(LevelIndices))]
        public void Level_StarterMatchesTheScreenshot(int index)
        {
            Assert.AreEqual(ScreenshotStarters[index], Config.Levels[index].StarterCat);
        }

        [TestCaseSource(nameof(LevelIndices))]
        public void Level_HasOneSolutionMatchingTheScreenshot(int index)
        {
            IReadOnlyList<int[]> solutions = LevelSolver.FindSolutions(Config.Levels[index].ToDefinition(), 2);

            Assert.AreEqual(1, solutions.Count);
            CollectionAssert.AreEqual(ScreenshotSolutions[index], solutions[0]);
        }

        private static int CountReachable(IReadOnlyList<Vector2Int> cells)
        {
            var remaining = new HashSet<Vector2Int>(cells);
            var pending = new Stack<Vector2Int>();
            pending.Push(cells[0]);
            remaining.Remove(cells[0]);
            int reached = 0;
            while (pending.Count > 0)
            {
                Vector2Int cell = pending.Pop();
                reached++;
                foreach (Vector2Int step in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
                {
                    if (remaining.Remove(cell + step))
                    {
                        pending.Push(cell + step);
                    }
                }
            }

            return reached;
        }
    }
}
