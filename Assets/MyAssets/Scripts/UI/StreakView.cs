using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VibeCodeTest.Data;

namespace VibeCodeTest.UI
{
    /// <summary>
    /// Full-screen streak celebration: the flame pops in, the number counts up and today gets its check.
    /// Content is a 9:16 column that fits the screen, so it stays whole in landscape.
    /// </summary>
    public sealed class StreakView : MonoBehaviour
    {
        private const float ContentAspect = 9f / 16f;
        private const float BackOvershoot = 1.70158f;

        private GameConfig config;
        private Image[] dayDiscs;
        private Image[] dayChecks;

        public RectTransform Flame { get; private set; }

        public Text NumberLabel { get; private set; }

        public Text TitleLabel { get; private set; }

        public RectTransform WeekRow { get; private set; }

        public Button ContinueButton { get; private set; }

        public bool IsVisible => gameObject.activeSelf;

        private static int TodayIndex => (int)DateTime.Today.DayOfWeek;

        public Image TodayCheck => dayChecks[TodayIndex];

        public static StreakView Create(RectTransform parent, GameConfig config)
        {
            RectTransform root = UiFactory.CreateRect("Streak Screen", parent);
            UiFactory.Stretch(root);
            UiFactory.AddImage(root, config.BackgroundColor, raycastTarget: true);
            StreakView view = root.gameObject.AddComponent<StreakView>();
            view.config = config;
            view.BuildContent(root);
            view.Hide();
            return view;
        }

        public void Show(int days, Action onContinue)
        {
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
            RectTransform content = UiFactory.CreateRect("Content", root);
            var fitter = content.gameObject.AddComponent<AspectRatioFitter>();
            fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            fitter.aspectRatio = ContentAspect;

            Flame = CreateBand("Flame", content, 0.6f, 0.92f);
            UiFactory.AddIcon(Flame, GameSprites.Flame, config.StreakAccentColor);
            RectTransform core = UiFactory.CreateRect("Core", Flame);
            UiFactory.Anchor(core, new Vector2(0.3f, 0.08f), new Vector2(0.7f, 0.58f));
            UiFactory.AddIcon(core, GameSprites.Flame, config.StreakFlameCoreColor);

            RectTransform number = CreateBand("Number", content, 0.42f, 0.6f);
            NumberLabel = UiFactory.AddFittedText(number, string.Empty, config.StreakNumberFontSize, config.StreakAccentColor);

            RectTransform title = CreateBand("Title", content, 0.34f, 0.42f);
            TitleLabel = UiFactory.AddFittedText(title, config.StreakTitle, config.StreakTitleFontSize, config.StreakAccentColor);

            WeekRow = CreateBand("Week", content, 0.17f, 0.3f);
            WeekRow.anchorMin = new Vector2(0.05f, WeekRow.anchorMin.y);
            WeekRow.anchorMax = new Vector2(0.95f, WeekRow.anchorMax.y);
            BuildWeek();

            RectTransform button = CreateBand("Continue Button", content, 0.05f, 0.14f);
            button.anchorMin = new Vector2(0.15f, button.anchorMin.y);
            button.anchorMax = new Vector2(0.85f, button.anchorMax.y);
            ContinueButton = UiFactory.AddButton(button, UiFactory.AddImage(button, config.StreakAccentColor));
            RectTransform label = UiFactory.CreateRect("Label", button);
            UiFactory.Stretch(label);
            UiFactory.AddFittedText(label, config.StreakContinueLabel, config.ResultButtonFontSize, Color.white);
        }

        private static RectTransform CreateBand(string name, RectTransform parent, float bottom, float top)
        {
            RectTransform rect = UiFactory.CreateRect(name, parent);
            UiFactory.Anchor(rect, new Vector2(0f, bottom), new Vector2(1f, top));
            return rect;
        }

        private void BuildWeek()
        {
            int dayCount = config.StreakWeekLetters.Length;
            dayDiscs = new Image[dayCount];
            dayChecks = new Image[dayCount];
            for (int day = 0; day < dayCount; day++)
            {
                RectTransform column = UiFactory.CreateRect($"Day {day}", WeekRow);
                float width = 1f / dayCount;
                UiFactory.Anchor(column, new Vector2(day * width, 0f), new Vector2((day + 1) * width, 1f));

                RectTransform letter = UiFactory.CreateRect("Letter", column);
                UiFactory.Anchor(letter, new Vector2(0f, 0.6f), Vector2.one);
                UiFactory.AddFittedText(letter, config.StreakWeekLetters[day].ToString(), config.StreakWeekFontSize, config.InkColor);

                RectTransform discRect = UiFactory.CreateRect("Disc", column);
                UiFactory.Anchor(discRect, Vector2.zero, new Vector2(1f, 0.58f));
                dayDiscs[day] = UiFactory.AddIcon(discRect, GameSprites.Disc, config.StreakPendingDayColor);

                RectTransform checkRect = UiFactory.CreateRect("Check", discRect);
                UiFactory.Anchor(checkRect, new Vector2(0.2f, 0.2f), new Vector2(0.8f, 0.8f));
                dayChecks[day] = UiFactory.AddIcon(checkRect, GameSprites.Check, Color.white);
                dayChecks[day].enabled = false;
            }
        }

        private IEnumerator PlayCelebration(int days)
        {
            NumberLabel.text = (days - 1).ToString();
            Flame.localScale = Vector3.zero;
            Image todayDisc = dayDiscs[TodayIndex];
            todayDisc.color = config.StreakPendingDayColor;
            TodayCheck.enabled = false;

            yield return Animate(config.StreakFlameInDuration, t => Flame.localScale = Vector3.one * EaseOutBack(t));
            yield return new WaitForSecondsRealtime(config.StreakCountDelay);

            NumberLabel.text = days.ToString();
            todayDisc.color = config.StreakAccentColor;
            TodayCheck.enabled = true;
            yield return Animate(config.StreakPopDuration, t =>
            {
                Vector3 scale = Vector3.one * (1f + (config.StreakPopScale * Mathf.Sin(Mathf.PI * t)));
                NumberLabel.rectTransform.localScale = scale;
                todayDisc.rectTransform.localScale = scale;
            });
        }

        private static IEnumerator Animate(float duration, Action<float> apply)
        {
            for (float elapsed = 0f; elapsed < duration; elapsed += Time.unscaledDeltaTime)
            {
                apply(elapsed / duration);
                yield return null;
            }

            apply(1f);
        }

        private static float EaseOutBack(float t)
        {
            float shifted = t - 1f;
            return 1f + ((BackOvershoot + 1f) * shifted * shifted * shifted) + (BackOvershoot * shifted * shifted);
        }
    }
}
