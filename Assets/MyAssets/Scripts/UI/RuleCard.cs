using UnityEngine;
using UnityEngine.UI;
using VibeCodeTest.Data;

namespace VibeCodeTest.UI
{
    /// <summary>One rule of the game: a tiny 3x3 board that illustrates it, beside a short text.</summary>
    public static class RuleCard
    {
        /// <summary>Tiny boards by rule. X is a marked cell, C a cat and a dot an empty cell.</summary>
        private static readonly string[][] Patterns =
        {
            new[] { "XXX", "XC.", "X.." },
            new[] { "XCX", ".X.", ".X." },
            new[] { "XXX", "XCX", "XXX" },
        };

        private const float CellGapRatio = 0.08f;
        private const float TinyCornerRadius = 5f;

        public static int Count => Patterns.Length;

        public static void Create(RectTransform parent, int rule, GameConfig config)
        {
            float width = 1f / Count;
            RectTransform card = UiFactory.CreateRect($"Rule {rule + 1}", parent);
            UiFactory.Anchor(card, new Vector2(rule * width, 0f), new Vector2((rule + 1) * width, 1f), 8f);
            UiFactory.AddRounded(card, config.RuleCardColor, config.CellCornerRadius);

            RectTransform iconArea = UiFactory.CreateRect("Illustration Area", card);
            UiFactory.Anchor(iconArea, new Vector2(0.03f, 0.18f), new Vector2(0.4f, 0.82f));
            RectTransform illustration = UiFactory.CreateRect("Illustration", iconArea);
            var fitter = illustration.gameObject.AddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            fitter.aspectRatio = 1f;
            BuildIllustration(illustration, Patterns[rule], config);

            RectTransform label = UiFactory.CreateRect("Text", card);
            UiFactory.Anchor(label, new Vector2(0.43f, 0.08f), new Vector2(0.97f, 0.92f));
            Text text = UiFactory.AddFittedText(label, config.RuleTexts[rule], config.RuleFontSize, config.InkColor);
            text.fontStyle = FontStyle.Normal;
            text.alignment = TextAnchor.MiddleLeft;
        }

        private static void BuildIllustration(RectTransform parent, string[] pattern, GameConfig config)
        {
            int size = pattern.Length;
            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    float side = BoardLayout.CellFraction(size, CellGapRatio, 0f);
                    float left = BoardLayout.CellStart(col, size, CellGapRatio, 0f);
                    float top = 1f - BoardLayout.CellStart(row, size, CellGapRatio, 0f);
                    RectTransform cell = UiFactory.CreateRect($"Cell {row},{col}", parent);
                    UiFactory.Anchor(cell, new Vector2(left, top - side), new Vector2(left + side, top));
                    BuildTinyCell(cell, pattern[row][col], config);
                }
            }
        }

        private static void BuildTinyCell(RectTransform cell, char kind, GameConfig config)
        {
            bool isEmpty = kind == '.';
            UiFactory.AddRounded(cell, isEmpty ? config.RuleEmptyCellColor : config.RuleCellColor, TinyCornerRadius);
            if (isEmpty)
            {
                return;
            }

            RectTransform icon = UiFactory.CreateRect("Icon", cell);
            UiFactory.Stretch(icon);
            bool isCat = kind == 'C';
            UiFactory.AddIcon(icon, isCat ? GameSprites.Cat : GameSprites.Mark, Color.white);
        }
    }
}
