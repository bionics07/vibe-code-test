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

        [SerializeField, Tooltip("Space around the X inside its cell, as a ratio of the cell.")]
        private float markInsetRatio = 0.14f;

        [SerializeField, Tooltip("Space around the cat inside its cell. Negative lets it spill over, as in the reference.")]
        private float catInsetRatio = -0.05f;

        [Header("Corner radii, in canvas units")]
        [SerializeField, Min(1f)]
        private float cellCornerRadius = 16f;

        [SerializeField, Min(1f)]
        private float panelCornerRadius = 36f;

        [SerializeField, Min(1f)]
        private float pillCornerRadius = 80f;

        [Header("Animations, in seconds")]
        [SerializeField, Min(0.01f)]
        private float popInDuration = 0.25f;

        [SerializeField, Min(0.01f)]
        private float mistakeShakeDuration = 0.35f;

        [SerializeField, Tooltip("Largest sideways move of the shake, in canvas units.")]
        private float mistakeShakeDistance = 14f;

        [SerializeField, Min(1f)]
        private float mistakeShakeOscillations = 3f;

        [SerializeField, Min(0.01f)]
        private float catCounterPunchDuration = 0.3f;

        [SerializeField, Min(0f), Tooltip("Extra scale at the peak of the punch on the cat counter.")]
        private float catCounterPunchScale = 0.3f;

        [SerializeField, Min(0f), Tooltip("Wait before the result panel, so the last move stays visible.")]
        private float resultPanelDelay = 0.7f;

        [Header("Screen margins and HUD, in canvas units")]
        [SerializeField]
        private float sideMargin = 40f;

        [SerializeField]
        private float hudTopMargin = 30f;

        [SerializeField]
        private float hudHeight = 500f;

        [SerializeField]
        private float boardBottomMargin = 60f;

        [SerializeField]
        private float backButtonSize = 120f;

        [SerializeField]
        private float hudTitleRowHeight = 170f;

        [SerializeField]
        private float hudPillRowTop = 185f;

        [SerializeField]
        private float hudPillRowHeight = 110f;

        [SerializeField]
        private float hudRulesTop = 310f;

        [SerializeField]
        private float hudRulesHeight = 190f;

        [Header("Text and panels")]
        [SerializeField]
        private string levelCaption = "Level";

        [SerializeField, Tooltip("One text per rule card, in order: color, column and row, touching.")]
        private string[] ruleTexts = { "1 Cat per color", "1 Cat per column and row", "Cats cannot touch" };

        [SerializeField]
        private int hudCaptionFontSize = 52;

        [SerializeField]
        private int hudNumberFontSize = 60;

        [SerializeField]
        private int hudCounterFontSize = 58;

        [SerializeField]
        private int ruleFontSize = 32;

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
        private Color cardColor = Color.white;

        [SerializeField]
        private Color ruleCardColor = new Color32(0xF8, 0xF1, 0xEC, 0xFF);

        [SerializeField]
        private Color ruleCellColor = new Color32(0xA6, 0x6E, 0x4B, 0xFF);

        [SerializeField]
        private Color ruleEmptyCellColor = new Color32(0xE3, 0xCD, 0xBB, 0xFF);

        [SerializeField]
        private Color catCountColor = new Color32(0x2F, 0xB3, 0x44, 0xFF);

        [SerializeField]
        private Color lifeColor = new Color32(0xFF, 0x8A, 0x3C, 0xFF);

        [SerializeField]
        private Color lifeLostColor = new Color32(0xF7, 0xDE, 0xD0, 0xFF);

        [SerializeField]
        private Color softShadowColor = new Color(0f, 0f, 0f, 0.08f);

        [SerializeField]
        private List<RegionColorEntry> regionColors = new List<RegionColorEntry>();

        [Header("Fake streak")]
        [SerializeField, Min(0), Tooltip("Zero-based level whose first completion grants the streak.")]
        private int streakLevelIndex;

        [SerializeField]
        private string streakScreenTitle = "Daily Streak";

        [SerializeField]
        private string streakCurrentLabel = "Current Streak";

        [SerializeField, Tooltip("{0} is the best streak.")]
        private string streakBestFormat = "Best Streak: {0}";

        [SerializeField]
        private string streakContinueLabel = "Continue";

        [SerializeField, Tooltip("Exactly seven labels, starting on Sunday.")]
        private string[] streakWeekLabels = { "SUN", "MON", "TUE", "WED", "THU", "FRI", "SAT" };

        [SerializeField]
        private int streakScreenTitleFontSize = 56;

        [SerializeField]
        private int streakNumberFontSize = 220;

        [SerializeField]
        private int streakCurrentFontSize = 72;

        [SerializeField]
        private int streakBestFontSize = 48;

        [SerializeField]
        private int streakWeekFontSize = 38;

        [SerializeField]
        private Color streakBackgroundColor = new Color32(0xFB, 0xF1, 0xDF, 0xFF);

        [SerializeField]
        private Color streakHeaderColor = new Color32(0xF9, 0xE6, 0xC8, 0xFF);

        [SerializeField, Min(0.01f)]
        private float streakSunInDuration = 0.45f;

        [SerializeField, Min(0f)]
        private float streakCountDelay = 0.35f;

        [SerializeField, Min(0.01f)]
        private float streakPopDuration = 0.3f;

        [SerializeField, Min(0f), Tooltip("Extra scale at the peak of the pop when the number changes.")]
        private float streakPopScale = 0.35f;

        [SerializeField]
        private Color streakAccentColor = new Color32(0xFF, 0x96, 0x00, 0xFF);

        [SerializeField]
        private Color streakSunCoreColor = new Color32(0xFF, 0xDE, 0x3D, 0xFF);

        [SerializeField]
        private Color streakPendingDayColor = new Color32(0xE2, 0xD6, 0xC2, 0xFF);

        public int StartingLives => startingLives;

        public float MarkInsetRatio => markInsetRatio;

        public float CatInsetRatio => catInsetRatio;

        public float CellCornerRadius => cellCornerRadius;

        public float PanelCornerRadius => panelCornerRadius;

        public float PillCornerRadius => pillCornerRadius;

        public float PopInDuration => popInDuration;

        public float MistakeShakeDuration => mistakeShakeDuration;

        public float MistakeShakeDistance => mistakeShakeDistance;

        public float MistakeShakeOscillations => mistakeShakeOscillations;

        public float CatCounterPunchDuration => catCounterPunchDuration;

        public float CatCounterPunchScale => catCounterPunchScale;

        public float ResultPanelDelay => resultPanelDelay;

        public float BackButtonSize => backButtonSize;

        public float HudTitleRowHeight => hudTitleRowHeight;

        public float HudPillRowTop => hudPillRowTop;

        public float HudPillRowHeight => hudPillRowHeight;

        public float HudRulesTop => hudRulesTop;

        public float HudRulesHeight => hudRulesHeight;

        public string LevelCaption => levelCaption;

        public IReadOnlyList<string> RuleTexts => ruleTexts;

        public int HudCaptionFontSize => hudCaptionFontSize;

        public int HudNumberFontSize => hudNumberFontSize;

        public int HudCounterFontSize => hudCounterFontSize;

        public int RuleFontSize => ruleFontSize;

        public Color CardColor => cardColor;

        public Color RuleCardColor => ruleCardColor;

        public Color RuleCellColor => ruleCellColor;

        public Color RuleEmptyCellColor => ruleEmptyCellColor;

        public Color CatCountColor => catCountColor;

        public Color LifeColor => lifeColor;

        public Color LifeLostColor => lifeLostColor;

        public Color SoftShadowColor => softShadowColor;

        public int StreakLevelIndex => streakLevelIndex;

        public string StreakScreenTitle => streakScreenTitle;

        public string StreakCurrentLabel => streakCurrentLabel;

        public string StreakBestFormat => streakBestFormat;

        public string StreakContinueLabel => streakContinueLabel;

        public IReadOnlyList<string> StreakWeekLabels => streakWeekLabels;

        public int StreakScreenTitleFontSize => streakScreenTitleFontSize;

        public int StreakNumberFontSize => streakNumberFontSize;

        public int StreakCurrentFontSize => streakCurrentFontSize;

        public int StreakBestFontSize => streakBestFontSize;

        public int StreakWeekFontSize => streakWeekFontSize;

        public Color StreakBackgroundColor => streakBackgroundColor;

        public Color StreakHeaderColor => streakHeaderColor;

        public float StreakSunInDuration => streakSunInDuration;

        public float StreakCountDelay => streakCountDelay;

        public float StreakPopDuration => streakPopDuration;

        public float StreakPopScale => streakPopScale;

        public Color StreakAccentColor => streakAccentColor;

        public Color StreakSunCoreColor => streakSunCoreColor;

        public Color StreakPendingDayColor => streakPendingDayColor;

        public float DoubleTapWindow => doubleTapWindow;

        public IReadOnlyList<LevelData> Levels => levels;

        public float CellGapRatio => cellGapRatio;

        public float BoardPaddingRatio => boardPaddingRatio;

        public float SideMargin => sideMargin;

        public float HudTopMargin => hudTopMargin;

        public float HudHeight => hudHeight;

        public float BoardBottomMargin => boardBottomMargin;

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
