using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VibeCodeTest.Data;
using VibeCodeTest.Puzzle;

namespace VibeCodeTest.UI
{
    /// <summary>Top of the screen: back button, level, cat counter, lives and rules, stacked from the top edge.</summary>
    public sealed class HudView : MonoBehaviour
    {
        private readonly List<Image> lifeIcons = new List<Image>();

        private GameConfig config;
        private Coroutine punch;
        private int shownLevel;
        private int shownCats;

        public Button BackButton { get; private set; }

        public Text LevelNumberLabel { get; private set; }

        public RectTransform CatCounter { get; private set; }

        public RectTransform CatIcon { get; private set; }

        public Text CatsLabel { get; private set; }

        public RectTransform Lives { get; private set; }

        public IReadOnlyList<Image> LifeIcons => lifeIcons;

        public RectTransform Rules { get; private set; }

        public static HudView Create(RectTransform rect, GameConfig config)
        {
            HudView view = rect.gameObject.AddComponent<HudView>();
            view.config = config;
            view.BuildTitleRow(rect);
            view.BuildCounterRow(rect);
            view.BuildRules(rect);
            return view;
        }

        public void Refresh(PuzzleGame game, int levelNumber)
        {
            bool foundCat = levelNumber == shownLevel && game.CatsPlaced > shownCats;
            shownLevel = levelNumber;
            shownCats = game.CatsPlaced;

            LevelNumberLabel.text = levelNumber.ToString();
            string found = ColorUtility.ToHtmlStringRGB(config.CatCountColor);
            CatsLabel.text = $"<color=#{found}>{game.CatsPlaced}</color>/{game.CatsRequired}";
            for (int life = 0; life < lifeIcons.Count; life++)
            {
                lifeIcons[life].color = life < game.Lives ? config.LifeColor : config.LifeLostColor;
            }

            if (foundCat)
            {
                PunchCatCounter();
            }
        }

        private void BuildTitleRow(RectTransform parent)
        {
            float size = config.BackButtonSize;
            RectTransform back = UiFactory.CreateRect("Back Button", parent);
            back.anchorMin = back.anchorMax = back.pivot = new Vector2(0f, 1f);
            back.sizeDelta = new Vector2(size, size);
            back.anchoredPosition = new Vector2(0f, -(config.HudTitleRowHeight - size) / 2f);
            Image disc = UiFactory.AddIcon(back, GameSprites.Disc, config.CardColor);
            UiFactory.AddSoftShadow(disc, config.SoftShadowColor);
            BackButton = UiFactory.AddButton(back, disc);

            RectTransform arrow = UiFactory.CreateRect("Arrow", back);
            UiFactory.Anchor(arrow, new Vector2(0.24f, 0.24f), new Vector2(0.76f, 0.76f));
            UiFactory.AddIcon(arrow, GameSprites.BackArrow, config.InkColor);

            RectTransform title = UiFactory.CreateRect("Level", parent);
            UiFactory.AnchorTopRow(title, 0f, config.HudTitleRowHeight);

            RectTransform caption = UiFactory.CreateRect("Caption", title);
            UiFactory.Anchor(caption, new Vector2(0f, 0.5f), Vector2.one);
            UiFactory.AddText(caption, config.LevelCaption, config.HudCaptionFontSize, config.InkColor);

            RectTransform number = UiFactory.CreateRect("Number", title);
            UiFactory.Anchor(number, Vector2.zero, new Vector2(1f, 0.5f));
            LevelNumberLabel = UiFactory.AddFittedText(number, string.Empty, config.HudNumberFontSize, config.InkColor);
        }

        private void BuildCounterRow(RectTransform parent)
        {
            RectTransform row = UiFactory.CreateRect("Counters", parent);
            UiFactory.AnchorTopRow(row, config.HudPillRowTop, config.HudPillRowHeight);

            CatCounter = CreatePill("Cat Counter", row, 0.13f, 0.49f);
            RectTransform iconArea = UiFactory.CreateRect("Cat Icon", CatCounter);
            UiFactory.Anchor(iconArea, new Vector2(0.1f, 0.1f), new Vector2(0.42f, 0.9f));
            UiFactory.AddIcon(iconArea, GameSprites.Cat, Color.white);
            CatIcon = iconArea;

            RectTransform count = UiFactory.CreateRect("Count", CatCounter);
            UiFactory.Anchor(count, new Vector2(0.42f, 0.1f), new Vector2(0.92f, 0.9f));
            CatsLabel = UiFactory.AddFittedText(count, string.Empty, config.HudCounterFontSize, config.InkColor);

            Lives = CreatePill("Lives", row, 0.51f, 0.87f);
            float width = 0.84f / config.StartingLives;
            for (int life = 0; life < config.StartingLives; life++)
            {
                RectTransform icon = UiFactory.CreateRect($"Life {life + 1}", Lives);
                float left = 0.08f + (life * width);
                UiFactory.Anchor(icon, new Vector2(left, 0.18f), new Vector2(left + width, 0.82f));
                lifeIcons.Add(UiFactory.AddIcon(icon, GameSprites.Fish, config.LifeColor));
            }
        }

        private void BuildRules(RectTransform parent)
        {
            Rules = UiFactory.CreateRect("Rules", parent);
            UiFactory.AnchorTopRow(Rules, config.HudRulesTop, config.HudRulesHeight);
            Image panel = UiFactory.AddRounded(Rules, config.CardColor, config.PanelCornerRadius);
            UiFactory.AddSoftShadow(panel, config.SoftShadowColor);

            RectTransform cards = UiFactory.CreateRect("Cards", Rules);
            UiFactory.Stretch(cards, 12f);
            for (int rule = 0; rule < RuleCard.Count; rule++)
            {
                RuleCard.Create(cards, rule, config);
            }
        }

        private RectTransform CreatePill(string name, RectTransform parent, float left, float right)
        {
            RectTransform pill = UiFactory.CreateRect(name, parent);
            UiFactory.Anchor(pill, new Vector2(left, 0f), new Vector2(right, 1f));
            Image background = UiFactory.AddRounded(pill, config.CardColor, config.PillCornerRadius);
            UiFactory.AddSoftShadow(background, config.SoftShadowColor);
            return pill;
        }

        private void PunchCatCounter()
        {
            if (punch != null)
            {
                StopCoroutine(punch);
            }

            punch = StartCoroutine(UiAnimation.Run(config.CatCounterPunchDuration, t =>
            {
                Vector3 scale = Vector3.one * UiAnimation.Punch(t, config.CatCounterPunchScale);
                CatIcon.localScale = scale;
                CatsLabel.rectTransform.localScale = scale;
            }));
        }
    }
}
