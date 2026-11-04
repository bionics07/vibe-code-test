using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using VibeCodeTest.Data;
using VibeCodeTest.Puzzle;

namespace VibeCodeTest.UI
{
    /// <summary>Lives on the gameplay Canvas: builds the screen and runs the level flow.</summary>
    public sealed class GameplayController : MonoBehaviour
    {
        private const int MainMenuBuildIndex = 1;

        // Kept for the browser session: going back to the menu must not grant the streak again.
        private static StreakProgress sessionStreak;

        [SerializeField]
        private GameConfig config;

        private RectTransform hudRect;
        private RectTransform boardArea;

        public GameConfig Config => config;

        public StreakView Streak { get; private set; }

        public PuzzleGame Game { get; private set; }

        public int LevelIndex { get; private set; }

        public GridView Grid { get; private set; }

        public HudView Hud { get; private set; }

        public ResultPanel Result { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ResetSessionProgress()
        {
            sessionStreak = null;
        }

        private void Awake()
        {
            sessionStreak ??= new StreakProgress(config.StreakLevelIndex);
            BuildScreen();
            StartLevel(0);
        }

        // The Canvas resizes with the screen, including when a phone rotates.
        private void OnRectTransformDimensionsChange()
        {
            if (hudRect != null)
            {
                ApplyLayout();
            }
        }

        private void BuildScreen()
        {
            var root = (RectTransform)transform;

            RectTransform background = UiFactory.CreateRect("Background", root);
            UiFactory.Stretch(background);
            UiFactory.AddImage(background, config.BackgroundColor);
            background.SetAsFirstSibling();

            hudRect = UiFactory.CreateRect("HUD", root);
            Hud = HudView.Create(hudRect, config);
            Hud.BackButton.onClick.AddListener(() => SceneManager.LoadScene(MainMenuBuildIndex));

            boardArea = UiFactory.CreateRect("Board Area", root);
            Grid = GridView.Create(boardArea, config);
            Grid.CellTapped += OnCellTapped;

            Result = ResultPanel.Create(root, config);
            Streak = StreakView.Create(root, config);
            ApplyLayout();
        }

        /// <summary>Portrait stacks the HUD over the board. Landscape puts them side by side, so the board stays large.</summary>
        private void ApplyLayout()
        {
            Rect canvas = ((RectTransform)transform).rect;
            float margin = config.SideMargin;
            float top = config.HudTopMargin;
            float bottom = config.BoardBottomMargin;

            if (canvas.width > canvas.height)
            {
                SetRect(hudRect, Vector2.zero, new Vector2(0.5f, 1f), new Vector2(margin, bottom), new Vector2(-margin / 2f, -top));
                SetRect(boardArea, new Vector2(0.5f, 0f), Vector2.one, new Vector2(margin / 2f, bottom), new Vector2(-margin, -top));
            }
            else
            {
                float hudBottom = top + config.HudHeight;
                SetRect(hudRect, new Vector2(0f, 1f), Vector2.one, new Vector2(margin, -hudBottom), new Vector2(-margin, -top));
                SetRect(boardArea, Vector2.zero, Vector2.one, new Vector2(margin, bottom), new Vector2(-margin, -(hudBottom + margin)));
            }
        }

        private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        private void StartLevel(int levelIndex)
        {
            LevelIndex = levelIndex;
            Game = new PuzzleGame(config.Levels[levelIndex].ToDefinition(), config.StartingLives, config.DoubleTapWindow);
            Grid.Build(Game.Level);
            Result.Hide();
            Streak.Hide();
            Refresh();
        }

        private void OnCellTapped(int row, int col)
        {
            Game.Tap(row, col, Time.unscaledTime);
            Refresh();

            if (Game.Status == PuzzleStatus.Won)
            {
                StartCoroutine(AfterResultDelay(ShowLevelWon));
            }
            else if (Game.Status == PuzzleStatus.Lost)
            {
                StartCoroutine(AfterResultDelay(() => Result.Show("Out of lives", "Retry", () => StartLevel(LevelIndex))));
            }
        }

        private IEnumerator AfterResultDelay(Action show)
        {
            yield return new WaitForSecondsRealtime(config.ResultPanelDelay);
            show();
        }

        private void ShowLevelWon()
        {
            int nextLevel = LevelIndex + 1;
            if (nextLevel < config.Levels.Count)
            {
                Result.Show($"Level {LevelIndex + 1} complete", "Next", () => LeaveWonLevel(nextLevel));
            }
            else
            {
                Result.Show("All levels complete", "Menu", () => SceneManager.LoadScene(MainMenuBuildIndex));
            }
        }

        private void LeaveWonLevel(int nextLevel)
        {
            if (sessionStreak.TryCelebrate(LevelIndex))
            {
                Result.Hide();
                Streak.Show(sessionStreak.Days, () => StartLevel(nextLevel));
            }
            else
            {
                StartLevel(nextLevel);
            }
        }

        private void Refresh()
        {
            Grid.Refresh(Game);
            Hud.Refresh(Game, LevelIndex + 1);
        }
    }
}
