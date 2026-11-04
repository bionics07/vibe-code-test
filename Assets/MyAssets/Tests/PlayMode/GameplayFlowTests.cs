using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using VibeCodeTest.Data;
using VibeCodeTest.Puzzle;
using VibeCodeTest.UI;

namespace VibeCodeTest.Tests
{
    /// <summary>Drives the real gameplay scene through its buttons.</summary>
    public sealed class GameplayFlowTests
    {
        private const int GameplayBuildIndex = 2;
        private const int MainMenuBuildIndex = 1;
        private const int MaxWaitFrames = 60;

        private static readonly Vector2Int[] ScreenSizes =
        {
            new Vector2Int(1080, 1920),
            new Vector2Int(720, 1560),
            new Vector2Int(1560, 720),
            new Vector2Int(1920, 1080),
            new Vector2Int(1536, 2048),
        };

        private GameplayController controller;

        private static IEnumerable<Vector2Int> SizesToCheck => ScreenSizes;

        private GameConfig Config => controller.Config;

        private float PastDoubleTapWindow => Config.DoubleTapWindow + 0.1f;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            SceneManager.LoadScene(GameplayBuildIndex);
            yield return null;
            yield return null;
            controller = Object.FindAnyObjectByType<GameplayController>();
            Assert.IsNotNull(controller, "The gameplay scene needs a GameplayController.");
            Assert.IsNotNull(controller.Config, "The GameplayController needs a GameConfig.");
        }

        [Test]
        public void Scene_StartsOnLevelOneWithTheStarterCatCounted()
        {
            Vector2Int starter = Config.Levels[0].StarterCat;

            Assert.AreEqual(0, controller.LevelIndex);
            Assert.AreEqual(4, controller.Grid.Size);
            Assert.AreEqual("Level 1", controller.Hud.LevelLabel.text);
            Assert.AreEqual("Cats 1/4", controller.Hud.CatsLabel.text);
            Assert.AreEqual("Lives 3/3", controller.Hud.LivesLabel.text);
            Assert.AreEqual(GameSprites.Cat, controller.Grid.IconAt(starter.y, starter.x).sprite);
            Assert.IsFalse(controller.Result.IsVisible);
        }

        [Test]
        public void Cells_UseTheColorsOfTheLevelAsset()
        {
            LevelData level = Config.Levels[0];
            foreach (LevelData.RegionData region in level.Regions)
            {
                foreach (Vector2Int cell in region.Cells)
                {
                    var image = (Image)controller.Grid.ButtonAt(cell.y, cell.x).targetGraphic;
                    Assert.AreEqual(Config.ColorOf(region.Color), image.color, $"Cell {cell}");
                }
            }
        }

        [Test]
        public void TapOnce_ShowsWhiteMark()
        {
            Tap(0, 0);

            Image icon = controller.Grid.IconAt(0, 0);
            Assert.IsTrue(icon.enabled);
            Assert.AreEqual(GameSprites.Mark, icon.sprite);
            Assert.AreEqual(Config.MarkColor, icon.color);
        }

        [Test]
        public void TapTwiceOnSolution_ShowsCatAndCountsIt()
        {
            int col = SolutionFor(controller.Game.Level)[0];

            Tap(0, col);
            Tap(0, col);

            Assert.AreEqual(GameSprites.Cat, controller.Grid.IconAt(0, col).sprite);
            Assert.AreEqual("Cats 2/4", controller.Hud.CatsLabel.text);
        }

        [Test]
        public void TapTwiceOnWrongCell_ShowsRedMarkAndLosesALife()
        {
            int col = WrongColumns(0).First();

            Tap(0, col);
            Tap(0, col);

            Image icon = controller.Grid.IconAt(0, col);
            Assert.AreEqual(GameSprites.Mark, icon.sprite);
            Assert.AreEqual(Config.MistakeColor, icon.color);
            Assert.AreEqual("Lives 2/3", controller.Hud.LivesLabel.text);
        }

        [UnityTest]
        public IEnumerator SlowSecondTap_RemovesTheMark()
        {
            int col = SolutionFor(controller.Game.Level)[0];

            Tap(0, col);
            yield return new WaitForSecondsRealtime(PastDoubleTapWindow);
            Tap(0, col);

            Assert.IsFalse(controller.Grid.IconAt(0, col).enabled);
            Assert.AreEqual("Cats 1/4", controller.Hud.CatsLabel.text);
            Assert.AreEqual("Lives 3/3", controller.Hud.LivesLabel.text);
        }

        [UnityTest]
        public IEnumerator RealMouseDoubleClick_PlacesCat()
        {
            int col = SolutionFor(controller.Game.Level)[0];
            yield return null;
            Vector2 position = CenterOf((RectTransform)controller.Grid.ButtonAt(0, col).transform);

            yield return ClickWithMouse(position);
            Assert.AreEqual(CellState.Marked, controller.Game.CellAt(0, col), "The first click must land on the cell.");
            yield return ClickWithMouse(position);

            Assert.AreEqual(CellState.Cat, controller.Game.CellAt(0, col));
        }

        [UnityTest]
        public IEnumerator RealMouseSlowClicks_MarkThenUnmark()
        {
            yield return null;
            Vector2 position = CenterOf((RectTransform)controller.Grid.ButtonAt(0, 0).transform);

            yield return ClickWithMouse(position);
            Assert.AreEqual(CellState.Marked, controller.Game.CellAt(0, 0), "The first click must land on the cell.");
            yield return new WaitForSecondsRealtime(PastDoubleTapWindow);
            yield return ClickWithMouse(position);

            Assert.AreEqual(CellState.Empty, controller.Game.CellAt(0, 0));
        }

        [UnityTest]
        public IEnumerator ThreeMistakes_ShowRetry_WhichRestartsTheSameLevelWithItsStarter()
        {
            foreach (int col in WrongColumns(0))
            {
                Tap(0, col);
                Tap(0, col);
            }

            Assert.IsTrue(controller.Result.IsVisible);
            Assert.AreEqual("Retry", controller.Result.ActionLabel.text);

            controller.Result.ActionButton.onClick.Invoke();
            yield return null;

            Vector2Int starter = Config.Levels[0].StarterCat;
            Assert.IsFalse(controller.Result.IsVisible);
            Assert.AreEqual(0, controller.LevelIndex);
            Assert.AreEqual("Lives 3/3", controller.Hud.LivesLabel.text);
            Assert.AreEqual("Cats 1/4", controller.Hud.CatsLabel.text);
            Assert.IsFalse(controller.Grid.IconAt(0, WrongColumns(0).First()).enabled);
            Assert.IsTrue(controller.Grid.IconAt(starter.y, starter.x).enabled);
        }

        [UnityTest]
        public IEnumerator CompletingEveryLevel_ReachesTheMenu()
        {
            for (int index = 0; index < Config.Levels.Count; index++)
            {
                Assert.AreEqual(index, controller.LevelIndex);
                Assert.AreEqual(Config.Levels[index].GridSize, controller.Grid.Size);
                SolveCurrentLevel();
                Assert.IsTrue(controller.Result.IsVisible);

                bool isLast = index == Config.Levels.Count - 1;
                Assert.AreEqual(isLast ? "Menu" : "Next", controller.Result.ActionLabel.text);
                controller.Result.ActionButton.onClick.Invoke();
                yield return null;
                yield return null;
            }

            Assert.AreEqual(MainMenuBuildIndex, SceneManager.GetActiveScene().buildIndex);
        }

        [UnityTest]
        public IEnumerator EveryLevel_KeepsTheBoardSizeAndProportionalSquareCells()
        {
            yield return null;
            Rect firstBoard = ScreenRect(controller.Grid.Board);

            for (int index = 0; index < Config.Levels.Count; index++)
            {
                yield return null;
                Rect board = ScreenRect(controller.Grid.Board);
                Assert.AreEqual(firstBoard.width, board.width, 0.5f, $"Level {index + 1} board size");

                int last = controller.Grid.Size - 1;
                Rect first = ScreenRect((RectTransform)controller.Grid.ButtonAt(0, 0).transform);
                Rect next = ScreenRect((RectTransform)controller.Grid.ButtonAt(0, 1).transform);
                Rect lastCell = ScreenRect((RectTransform)controller.Grid.ButtonAt(last, last).transform);

                Assert.AreEqual(first.width, first.height, 0.5f, $"Level {index + 1} cell must be square");
                Assert.AreEqual(Config.CellGapRatio * first.width, next.xMin - first.xMax, 0.5f, $"Level {index + 1} gap");
                Assert.AreEqual(Config.BoardPaddingRatio * first.width, first.xMin - board.xMin, 0.5f, $"Level {index + 1} padding");
                Assert.AreEqual(first.xMin - board.xMin, board.xMax - lastCell.xMax, 0.5f, $"Level {index + 1} right padding");
                Assert.AreEqual(first.xMin - board.xMin, lastCell.yMin - board.yMin, 0.5f, $"Level {index + 1} bottom padding");

                if (index < Config.Levels.Count - 1)
                {
                    SolveCurrentLevel();
                    controller.Result.ActionButton.onClick.Invoke();
                }
            }
        }

        [UnityTest]
        public IEnumerator Cells_ReceiveRaycasts_UntilTheResultPanelCoversThem()
        {
            yield return null;
            var cell = (RectTransform)controller.Grid.ButtonAt(1, 1).transform;
            Assert.AreEqual(controller.Grid.ButtonAt(1, 1).gameObject, TopRaycastAt(cell));

            SolveCurrentLevel();
            yield return null;

            Assert.AreEqual(controller.Result.gameObject, TopRaycastAt(cell));
        }

#if UNITY_EDITOR
        [UnityTest]
        public IEnumerator Layout_KeepsEverythingOnScreenWithoutOverlap([ValueSource(nameof(SizesToCheck))] Vector2Int size)
        {
            UnityEditor.PlayModeWindow.GetRenderingResolution(out uint originalWidth, out uint originalHeight);
            UnityEditor.PlayModeWindow.SetCustomRenderingResolution((uint)size.x, (uint)size.y, "Layout test");
            try
            {
                for (int frame = 0; frame < MaxWaitFrames && (Screen.width != size.x || Screen.height != size.y); frame++)
                {
                    yield return null;
                }

                if (Screen.width != size.x || Screen.height != size.y)
                {
                    Assert.Inconclusive($"Game View stayed at {Screen.width}x{Screen.height}; it must be open.");
                }

                yield return null;
                yield return null;
                AssertLayoutFits();
            }
            finally
            {
                UnityEditor.PlayModeWindow.SetCustomRenderingResolution(originalWidth, originalHeight, "Layout test");
            }
        }

        private void AssertLayoutFits()
        {
            Rect screen = new Rect(0f, 0f, Screen.width, Screen.height);
            Rect board = ScreenRect(controller.Grid.Board);
            Rect hud = ScreenRect((RectTransform)controller.Hud.transform);
            Rect back = ScreenRect((RectTransform)GameObject.Find("Back Button").transform);

            AssertInside(screen, board, "Board");
            AssertInside(screen, hud, "HUD");
            AssertInside(screen, back, "Back Button");
            Assert.IsFalse(board.Overlaps(hud), $"Board {board} overlaps HUD {hud}.");
            Assert.IsFalse(board.Overlaps(back), $"Board {board} overlaps Back {back}.");
            Assert.AreEqual(board.width, board.height, 1f, "Board must stay square.");
            AssertInside(board, ScreenRect((RectTransform)controller.Grid.ButtonAt(3, 3).transform), "Last cell");
        }

        private static void AssertInside(Rect outer, Rect inner, string label)
        {
            const float tolerance = 1f;
            Assert.IsTrue(
                inner.xMin >= outer.xMin - tolerance && inner.xMax <= outer.xMax + tolerance &&
                inner.yMin >= outer.yMin - tolerance && inner.yMax <= outer.yMax + tolerance,
                $"{label} {inner} leaves {outer}.");
        }
#endif

        private static Rect ScreenRect(RectTransform rect)
        {
            var corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            return Rect.MinMaxRect(corners[0].x, corners[0].y, corners[2].x, corners[2].y);
        }

        private static IEnumerator ClickWithMouse(Vector2 position)
        {
            Mouse mouse = Mouse.current;
            if (mouse == null)
            {
                Assert.Inconclusive("No mouse device to drive.");
            }

            SendPointer(mouse, position, mouse.leftButton, true);
            SendPointer(mouse, position, mouse.leftButton, false);
            InputSystem.Update();
            yield return null;
            yield return null;
        }

        private static void SendPointer(Mouse mouse, Vector2 position, ButtonControl button, bool pressed)
        {
            using (StateEvent.From(mouse, out InputEventPtr eventPtr))
            {
                mouse.position.WriteValueIntoEvent(position, eventPtr);
                button.WriteValueIntoEvent(pressed ? 1f : 0f, eventPtr);
                InputSystem.QueueEvent(eventPtr);
            }
        }

        /// <summary>Two calls in the same frame arrive with the same time, so they count as a double tap.</summary>
        private void Tap(int row, int col)
        {
            controller.Grid.ButtonAt(row, col).onClick.Invoke();
        }

        private void SolveCurrentLevel()
        {
            int[] solution = SolutionFor(controller.Game.Level);
            for (int row = 0; row < solution.Length; row++)
            {
                Tap(row, solution[row]);
                Tap(row, solution[row]);
            }
        }

        private IEnumerable<int> WrongColumns(int row)
        {
            int solved = SolutionFor(controller.Game.Level)[row];
            return Enumerable.Range(0, controller.Grid.Size).Where(col => col != solved).Take(Config.StartingLives);
        }

        private static int[] SolutionFor(LevelDefinition level)
        {
            return LevelSolver.FindSolutions(level, 1)[0];
        }

        private static Vector2 CenterOf(RectTransform target)
        {
            var corners = new Vector3[4];
            target.GetWorldCorners(corners);
            return (corners[0] + corners[2]) / 2f;
        }

        private static GameObject TopRaycastAt(RectTransform target)
        {
            var pointer = new PointerEventData(EventSystem.current) { position = CenterOf(target) };
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, results);
            return results.Count > 0 ? results[0].gameObject : null;
        }
    }
}
