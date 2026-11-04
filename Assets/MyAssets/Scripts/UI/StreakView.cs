using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VibeCodeTest.Data;

namespace VibeCodeTest.UI
{
    /// <summary>
    /// Full-screen streak celebration, laid out like the reference game. The sun pops in, the number
    /// counts up and today, always the first slot, gets its check. The last slot holds the reward.
    /// Content is a 9:16 column that fits the screen, so it stays whole in landscape.
    /// </summary>
    public sealed class StreakView : MonoBehaviour
    {
        private const float ContentAspect = 9f / 16f;

        private GameConfig config;
        private Image todayDisc;

        public RectTransform HeaderBar { get; private set; }

        public Text TitleLabel { get; private set; }

        public RectTransform Sun { get; private set; }

        public Text NumberLabel { get; private set; }

        public Text CurrentLabel { get; private set; }

        public RectTransform BestPill { get; private set; }

        public Text BestLabel { get; private set; }

        public RectTransform WeekRow { get; private set; }

        public Text TodayLabel { get; private set; }

        public Image TodayCheck { get; private set; }

        public Image RewardIcon { get; private set; }

        public Button ContinueButton { get; private set; }

        public bool IsVisible => gameObject.activeSelf;

        public static StreakView Create(RectTransform parent, GameConfig config)
        {
            RectTransform root = UiFactory.CreateRect("Streak Screen", parent);
            UiFactory.Stretch(root);
            UiFactory.AddImage(root, config.StreakBackgroundColor, raycastTarget: true);
            StreakView view = root.gameObject.AddComponent<StreakView>();
            view.config = config;
            view.BuildContent(root);
            view.Hide();
            return view;
        }

        public void Show(int days, Action onContinue)
        {
            BestLabel.text = string.Format(config.StreakBestFormat, days);
            ContinueButton.onClick.RemoveAllListeners();
            ContinueButton.onClick.AddListener(() => onContinue());
            gameObject.SetActive(true);
            StartCoroutine(PlayCelebration(days));
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void BuildContent(RectTransform root)
        {
            HeaderBar = UiFactory.CreateRect("Header", root);
            UiFactory.Anchor(HeaderBar, new Vector2(0f, 0.92f), Vector2.one);
            UiFactory.AddImage(HeaderBar, config.StreakHeaderColor);
            TitleLabel = UiFactory.AddFittedText(CreateChild("Title", HeaderBar), config.StreakScreenTitle, config.StreakScreenTitleFontSize, config.InkColor);

            RectTransform body = UiFactory.CreateRect("Body", root);
            UiFactory.Anchor(body, Vector2.zero, new Vector2(1f, 0.92f));
            RectTransform content = UiFactory.CreateRect("Content", body);
            var fitter = content.gameObject.AddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            fitter.aspectRatio = ContentAspect;

            Sun = CreateBand("Sun", content, 0.63f, 0.98f, 0f, 1f);
            UiFactory.AddIcon(Sun, GameSprites.Sun, config.StreakAccentColor);
            RectTransform core = UiFactory.CreateRect("Core", Sun);
            UiFactory.Anchor(core, new Vector2(0.28f, 0.28f), new Vector2(0.72f, 0.72f));
            UiFactory.AddIcon(core, GameSprites.Disc, config.StreakSunCoreColor);

            RectTransform number = CreateBand("Number", content, 0.46f, 0.63f, 0f, 1f);
            NumberLabel = UiFactory.AddFittedText(number, string.Empty, config.StreakNumberFontSize, config.StreakAccentColor);

            RectTransform current = CreateBand("Current", content, 0.395f, 0.46f, 0f, 1f);
            CurrentLabel = UiFactory.AddFittedText(current, config.StreakCurrentLabel, config.StreakCurrentFontSize, config.StreakAccentColor);

            BestPill = CreateBand("Best", content, 0.335f, 0.385f, 0.2f, 0.8f);
            UiFactory.AddRounded(BestPill, config.CardColor, config.PillCornerRadius);
            BestLabel = UiFactory.AddFittedText(CreateChild("Label", BestPill), string.Empty, config.StreakBestFontSize, config.InkColor);
            BestLabel.fontStyle = FontStyle.Normal;

            WeekRow = CreateBand("Week", content, 0.2f, 0.305f, 0.04f, 0.96f);
            BuildWeek();

            RectTransform button = CreateBand("Continue Button", content, 0.05f, 0.13f, 0.14f, 0.86f);
            Image background = UiFactory.AddRounded(button, config.StreakAccentColor, config.PillCornerRadius);
            UiFactory.AddSoftShadow(background, config.SoftShadowColor);
            ContinueButton = UiFactory.AddButton(button, background);
            UiFactory.AddFittedText(CreateChild("Label", button), config.StreakContinueLabel, config.ResultButtonFontSize, Color.white);
        }

        private static RectTransform CreateChild(string name, RectTransform parent)
        {
            RectTransform rect = UiFactory.CreateRect(name, parent);
            UiFactory.Stretch(rect);
            return rect;
        }

        private static RectTransform CreateBand(string name, RectTransform parent, float bottom, float top, float left, float right)
        {
            RectTransform rect = UiFactory.CreateRect(name, parent);
            UiFactory.Anchor(rect, new Vector2(left, bottom), new Vector2(right, top));
            return rect;
        }

        private void BuildWeek()
        {
            int slots = config.StreakWeekLabels.Count;
            int today = (int)DateTime.Today.DayOfWeek;
            for (int slot = 0; slot < slots; slot++)
            {
                float width = 1f / slots;
                RectTransform column = CreateBand($"Slot {slot}", WeekRow, 0f, 1f, slot * width, (slot + 1) * width);

                RectTransform label = CreateBand("Label", column, 0.62f, 1f, 0f, 1f);
                bool isToday = slot == 0;
                Color labelColor = isToday ? config.StreakAccentColor : config.InkColor;
                string dayName = config.StreakWeekLabels[(today + slot) % slots];
                Text dayLabel = UiFactory.AddFittedText(label, dayName, config.StreakWeekFontSize, labelColor);
                dayLabel.fontStyle = FontStyle.Normal;

                RectTransform slotIcon = CreateBand("Icon", column, 0f, 0.58f, 0.04f, 0.96f);
                if (slot == slots - 1)
                {
                    RewardIcon = UiFactory.AddIcon(slotIcon, GameSprites.Gift, Color.white);
                    continue;
                }

                Image disc = UiFactory.AddIcon(slotIcon, GameSprites.Disc, config.StreakPendingDayColor);
                if (isToday)
                {
                    todayDisc = disc;
                    TodayLabel = dayLabel;
                    TodayCheck = UiFactory.AddIcon(CreateBand("Check", slotIcon, 0.22f, 0.78f, 0.22f, 0.78f), GameSprites.Check, Color.white);
                }
            }
        }

        private IEnumerator PlayCelebration(int days)
        {
            NumberLabel.text = (days - 1).ToString();
            Sun.localScale = Vector3.zero;
            todayDisc.color = config.StreakPendingDayColor;
            TodayCheck.enabled = false;

            yield return UiAnimation.Run(config.StreakSunInDuration, t => Sun.localScale = Vector3.one * UiAnimation.EaseOutBack(t));
            yield return new WaitForSecondsRealtime(config.StreakCountDelay);

            NumberLabel.text = days.ToString();
            todayDisc.color = config.StreakAccentColor;
            TodayCheck.enabled = true;
            yield return UiAnimation.Run(config.StreakPopDuration, t =>
            {
                Vector3 scale = Vector3.one * UiAnimation.Punch(t, config.StreakPopScale);
                NumberLabel.rectTransform.localScale = scale;
                todayDisc.rectTransform.localScale = scale;
            });
        }
    }
}
