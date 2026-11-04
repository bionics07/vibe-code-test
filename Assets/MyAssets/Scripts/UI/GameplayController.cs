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

        private void BuildScreen()
        {
            var root = (RectTransform)transform;
            float hudBottom = config.HudTopMargin + config.HudHeight;

            RectTransform background = UiFactory.CreateRect("Background", root);
            UiFactory.Stretch(background);
            UiFactory.AddImage(background, config.BackgroundColor);
            background.SetAsFirstSibling();

            RectTransform hud = UiFactory.CreateRect("HUD", root);
            hud.anchorMin = new Vector2(0f, 1f);
            hud.anchorMax = Vector2.one;
            hud.offsetMin = new Vector2(config.SideMargin, -hudBottom);
            hud.offsetMax = new Vector2(-config.SideMargin, -config.HudTopMargin);
            Hud = HudView.Create(hud, config);

            RectTransform boardArea = UiFactory.CreateRect("Board Area", root);
            boardArea.anchorMin = Vector2.zero;
            boardArea.anchorMax = Vector2.one;
            boardArea.offsetMin = new Vector2(config.SideMargin, config.BoardBottomMargin);
            boardArea.offsetMax = new Vector2(-config.SideMargin, -hudBottom);
            Grid = GridView.Create(boardArea, config);
            Grid.CellTapped += OnCellTapped;

            Result = ResultPanel.Create(root, config);
            Streak = StreakView.Create(root, config);
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
                ShowLevelWon();
            }
            else if (Game.Status == PuzzleStatus.Lost)
            {
                Result.Show("Out of lives", "Retry", () => StartLevel(LevelIndex));
            }
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
