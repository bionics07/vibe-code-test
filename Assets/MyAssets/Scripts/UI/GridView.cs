using System;
using System.Collections;
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
        private CellState[,] shownStates;
        private Coroutine[,] animations;

        public event Action<int, int> CellTapped;

        public RectTransform Board => (RectTransform)transform;

        public int Size { get; private set; }

        public static GridView Create(RectTransform parent, GameConfig config)
        {
            RectTransform rect = UiFactory.CreateRect("Board", parent);
            UiFactory.AddRounded(rect, config.CardColor, config.PanelCornerRadius);
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
            StopAllCoroutines();
            foreach (Transform child in cellRoot)
            {
                Destroy(child.gameObject);
            }

            Size = level.Size;
            buttons = new Button[Size, Size];
            icons = new Image[Size, Size];
            shownStates = new CellState[Size, Size];
            animations = new Coroutine[Size, Size];
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    BuildCell(level, row, col);
                }
            }
        }

        /// <summary>Shows the game's cell states, animating only the cells that changed.</summary>
        public void Refresh(PuzzleGame game)
        {
            for (int row = 0; row < Size; row++)
            {
                for (int col = 0; col < Size; col++)
                {
                    CellState state = game.CellAt(row, col);
                    if (state != shownStates[row, col])
                    {
                        shownStates[row, col] = state;
                        ShowState(row, col, state);
                    }
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

            Image background = UiFactory.AddRounded(cell, config.ColorOf((RegionColor)level.RegionAt(row, col)), config.CellCornerRadius);
            Button button = UiFactory.AddButton(cell, background);
            button.onClick.AddListener(() => CellTapped?.Invoke(row, col));
            buttons[row, col] = button;

            RectTransform iconRect = UiFactory.CreateRect("Icon", cell);
            UiFactory.Stretch(iconRect);
            Image icon = UiFactory.AddImage(iconRect, Color.white);
            icon.preserveAspect = true;
            icon.enabled = false;
            icons[row, col] = icon;
        }

        private void ShowState(int row, int col, CellState state)
        {
            Image icon = icons[row, col];
            var cell = (RectTransform)buttons[row, col].transform;

            if (animations[row, col] != null)
            {
                StopCoroutine(animations[row, col]);
                animations[row, col] = null;
            }

            icon.rectTransform.localScale = Vector3.one;
            cell.anchoredPosition = Vector2.zero;
            icon.enabled = state != CellState.Empty;

            switch (state)
            {
                case CellState.Marked:
                    ApplyIcon(icon, GameSprites.Mark, config.MarkColor, config.MarkInsetRatio);
                    animations[row, col] = StartCoroutine(PopIn(icon.rectTransform));
                    break;
                case CellState.Mistake:
                    ApplyIcon(icon, GameSprites.Mark, config.MistakeColor, config.MarkInsetRatio);
                    animations[row, col] = StartCoroutine(Shake(cell));
                    break;
                case CellState.Cat:
                    ApplyIcon(icon, GameSprites.Cat, Color.white, config.CatInsetRatio);
                    cell.SetAsLastSibling();
                    animations[row, col] = StartCoroutine(PopIn(icon.rectTransform));
                    break;
            }
        }

        private static void ApplyIcon(Image icon, Sprite sprite, Color color, float inset)
        {
            icon.sprite = sprite;
            icon.color = color;
            UiFactory.Anchor(icon.rectTransform, new Vector2(inset, inset), new Vector2(1f - inset, 1f - inset));
        }

        private IEnumerator PopIn(RectTransform icon)
        {
            return UiAnimation.Run(config.PopInDuration, t => icon.localScale = Vector3.one * UiAnimation.EaseOutBack(t));
        }

        private IEnumerator Shake(RectTransform cell)
        {
            return UiAnimation.Run(config.MistakeShakeDuration, t =>
            {
                float offset = UiAnimation.Shake(t, config.MistakeShakeDistance, config.MistakeShakeOscillations);
                cell.anchoredPosition = new Vector2(offset, 0f);
            });
        }
    }
}
