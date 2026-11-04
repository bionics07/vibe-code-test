using System;
using System.Collections.Generic;
using UnityEngine;

namespace VibeCodeTest.Data
{
    /// <summary>Every tunable value of the game: rules, level order, layout, text and colors.</summary>
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Vibe Code Test/Game Config")]
    public sealed class GameConfig : ScriptableObject
    {
        [Header("Rules")]
        [SerializeField, Min(1)]
        private int startingLives = 3;

        [SerializeField, Min(0.01f), Tooltip("Longest gap in seconds between two taps on a cell that still counts as a double tap.")]
        private float doubleTapWindow = 0.4f;

        [Header("Levels")]
        [SerializeField]
        private List<LevelData> levels = new List<LevelData>();

        [Header("Board, as ratios of the cell size, so any grid size keeps the same look")]
        [SerializeField, Min(0f)]
        private float cellGapRatio = 0.13f;

        [SerializeField, Min(0f)]
        private float boardPaddingRatio = 0.22f;

        [SerializeField, Range(0f, 0.4f)]
        private float iconInsetRatio = 0.06f;

        [Header("Screen margins, in canvas units")]
        [SerializeField]
        private float sideMargin = 40f;

        [SerializeField]
        private float hudTopMargin = 40f;

        [SerializeField]
        private float hudHeight = 140f;

        [SerializeField]
        private float boardBottomMargin = 240f;

        [Header("Text and panels")]
        [SerializeField]
        private int hudFontSize = 54;

        [SerializeField]
        private int resultTitleFontSize = 64;

        [SerializeField]
        private int resultButtonFontSize = 56;

        [SerializeField]
        private Vector2 resultPanelSize = new Vector2(820f, 520f);

        [Header("Colors")]
        [SerializeField]
        private Color backgroundColor = new Color32(0xF6, 0xEF, 0xEB, 0xFF);

        [SerializeField]
        private Color boardColor = Color.white;

        [SerializeField]
        private Color inkColor = new Color32(0x8A, 0x4F, 0x52, 0xFF);

        [SerializeField]
        private Color markColor = Color.white;

        [SerializeField]
        private Color mistakeColor = new Color32(0xFF, 0x6A, 0x2B, 0xFF);

        [SerializeField]
        private Color dimColor = new Color(0f, 0f, 0f, 0.55f);

        [SerializeField]
        private Color resultButtonColor = new Color32(0x8F, 0xD9, 0x7E, 0xFF);

        [SerializeField]
        private List<RegionColorEntry> regionColors = new List<RegionColorEntry>();

        public int StartingLives => startingLives;

        public float DoubleTapWindow => doubleTapWindow;

        public IReadOnlyList<LevelData> Levels => levels;

        public float CellGapRatio => cellGapRatio;

        public float BoardPaddingRatio => boardPaddingRatio;

        public float IconInsetRatio => iconInsetRatio;

        public float SideMargin => sideMargin;

        public float HudTopMargin => hudTopMargin;

        public float HudHeight => hudHeight;

        public float BoardBottomMargin => boardBottomMargin;

        public int HudFontSize => hudFontSize;

        public int ResultTitleFontSize => resultTitleFontSize;

        public int ResultButtonFontSize => resultButtonFontSize;

        public Vector2 ResultPanelSize => resultPanelSize;

        public Color BackgroundColor => backgroundColor;

        public Color BoardColor => boardColor;

        public Color InkColor => inkColor;

        public Color MarkColor => markColor;

        public Color MistakeColor => mistakeColor;

        public Color DimColor => dimColor;

        public Color ResultButtonColor => resultButtonColor;

        public Color ColorOf(RegionColor region)
        {
            foreach (RegionColorEntry entry in regionColors)
            {
                if (entry.Region == region)
                {
                    return entry.Value;
                }
            }

            throw new InvalidOperationException($"{name} has no color for {region}.");
        }

        [Serializable]
        public sealed class RegionColorEntry
        {
            [SerializeField]
            private RegionColor region;

            [SerializeField]
            private Color value = Color.white;

            public RegionColor Region => region;

            public Color Value => value;
        }
    }
}
