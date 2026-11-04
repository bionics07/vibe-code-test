using System;
using UnityEngine;
using UnityEngine.UI;
using VibeCodeTest.Data;
using VibeCodeTest.Puzzle;

namespace VibeCodeTest.UI
{
    /// <summary>Draws the board as a square that fits its parent. Cells are anchored as fractions of it.</summary>
    public sealed class GridView : MonoBehaviour
    {
        private GameConfig config;
        private RectTransform cellRoot;
        private Button[,] buttons;
        private Image[,] icons;

        public event Action<int, int> CellTapped;

        public RectTransform Board => (RectTransform)transform;

        public int Size { get; private set; }

        public static GridView Create(RectTransform parent, GameConfig config)
        {
            RectTransform rect = UiFactory.CreateRect("Board", parent);
            UiFactory.AddImage(rect, config.BoardColor);
            var fitter = rect.gameObject.AddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            fitter.aspectRatio = 1f;

            GridView view = rect.gameObject.AddComponent<GridView>();
            view.config = config;
            view.cellRoot = UiFactory.CreateRect("Cells", rect);
            UiFactory.Stretch(view.cellRoot);
            return view;
        }

        public Button ButtonAt(int row, int col)
        {
            return buttons[row, col];
        }

        public Image IconAt(int row, int col)
        {
            return icons[row, col];
        }

        public void Build(LevelDefinition level)
        {
            foreach (Transform child in cellRoot)
            {
                Destroy(child.gameObject);
            }

            Size = level.Size;
            buttons = new Button[Size, Size];
            icons = new Image[Size, Size];
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    BuildCell(level, row, col);
                }
            }
        }

        public void Refresh(PuzzleGame game)
        {
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    ApplyState(icons[row, col], game.CellAt(row, col));
                }
            }
        }

        private void BuildCell(LevelDefinition level, int row, int col)
        {
            RectTransform cell = UiFactory.CreateRect($"Cell {row},{col}", cellRoot);
            float side = BoardLayout.CellFraction(Size, config.CellGapRatio, config.BoardPaddingRatio);
            float left = BoardLayout.CellStart(col, Size, config.CellGapRatio, config.BoardPaddingRatio);
            float top = 1f - BoardLayout.CellStart(row, Size, config.CellGapRatio, config.BoardPaddingRatio);
            UiFactory.Anchor(cell, new Vector2(left, top - side), new Vector2(left + side, top));

            Image background = UiFactory.AddImage(cell, config.ColorOf((RegionColor)level.RegionAt(row, col)));
            Button button = UiFactory.AddButton(cell, background);
            button.onClick.AddListener(() => CellTapped?.Invoke(row, col));
            buttons[row, col] = button;

            RectTransform iconRect = UiFactory.CreateRect("Icon", cell);
            float inset = config.IconInsetRatio;
            UiFactory.Anchor(iconRect, new Vector2(inset, inset), new Vector2(1f - inset, 1f - inset));
            Image icon = UiFactory.AddImage(iconRect, Color.white);
            icon.preserveAspect = true;
            icon.enabled = false;
            icons[row, col] = icon;
        }

        private void ApplyState(Image icon, CellState state)
        {
            icon.enabled = state != CellState.Empty;
            switch (state)
            {
                case CellState.Marked:
                    icon.sprite = GameSprites.Mark;
                    icon.color = config.MarkColor;
                    break;
                case CellState.Mistake:
                    icon.sprite = GameSprites.Mark;
                    icon.color = config.MistakeColor;
                    break;
                case CellState.Cat:
                    icon.sprite = GameSprites.Cat;
                    icon.color = Color.white;
                    break;
            }
        }
    }
}
