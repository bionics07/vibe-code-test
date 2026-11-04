using UnityEngine;
using UnityEngine.UI;
using VibeCodeTest.Data;
using VibeCodeTest.Puzzle;

namespace VibeCodeTest.UI
{
    /// <summary>Top bar with the level, the cats placed and the lives left.</summary>
    public sealed class HudView : MonoBehaviour
    {
        public Text LevelLabel { get; private set; }

        public Text CatsLabel { get; private set; }

        public Text LivesLabel { get; private set; }

        public static HudView Create(RectTransform rect, GameConfig config)
        {
            HudView view = rect.gameObject.AddComponent<HudView>();
            view.LevelLabel = CreateColumn(rect, "Level", 0, config);
            view.CatsLabel = CreateColumn(rect, "Cats", 1, config);
            view.LivesLabel = CreateColumn(rect, "Lives", 2, config);
            return view;
        }

        public void Refresh(PuzzleGame game, int levelNumber)
        {
            LevelLabel.text = $"Level {levelNumber}";
            CatsLabel.text = $"Cats {game.CatsPlaced}/{game.CatsRequired}";
            LivesLabel.text = $"Lives {game.Lives}/{game.StartingLives}";
        }

        private static Text CreateColumn(RectTransform parent, string name, int index, GameConfig config)
        {
            const float width = 1f / 3f;
            RectTransform rect = UiFactory.CreateRect(name, parent);
            UiFactory.Anchor(rect, new Vector2(index * width, 0f), new Vector2((index + 1) * width, 1f));
            return UiFactory.AddText(rect, string.Empty, config.HudFontSize, config.InkColor);
        }
    }
}
