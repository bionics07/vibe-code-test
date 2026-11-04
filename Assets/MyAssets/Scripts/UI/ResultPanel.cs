using System;
using UnityEngine;
using UnityEngine.UI;
using VibeCodeTest.Data;

namespace VibeCodeTest.UI
{
    /// <summary>Full-screen overlay that blocks the board and offers one action.</summary>
    public sealed class ResultPanel : MonoBehaviour
    {
        public Text Title { get; private set; }

        public Text ActionLabel { get; private set; }

        public Button ActionButton { get; private set; }

        public bool IsVisible => gameObject.activeSelf;

        public static ResultPanel Create(RectTransform parent, GameConfig config)
        {
            RectTransform root = UiFactory.CreateRect("Result Panel", parent);
            UiFactory.Stretch(root);
            UiFactory.AddImage(root, config.DimColor, raycastTarget: true);
            ResultPanel view = root.gameObject.AddComponent<ResultPanel>();

            RectTransform panel = UiFactory.CreateRect("Panel", root);
            panel.anchorMin = panel.anchorMax = new Vector2(0.5f, 0.5f);
            panel.sizeDelta = config.ResultPanelSize;
            UiFactory.AddRounded(panel, config.BackgroundColor, config.PanelCornerRadius);

            RectTransform title = UiFactory.CreateRect("Title", panel);
            UiFactory.Anchor(title, new Vector2(0f, 0.45f), Vector2.one, 30f);
            view.Title = UiFactory.AddFittedText(title, string.Empty, config.ResultTitleFontSize, config.InkColor);

            RectTransform button = UiFactory.CreateRect("Action Button", panel);
            UiFactory.Anchor(button, new Vector2(0.2f, 0.1f), new Vector2(0.8f, 0.42f));
            Image background = UiFactory.AddRounded(button, config.ResultButtonColor, config.PillCornerRadius);
            view.ActionButton = UiFactory.AddButton(button, background);

            RectTransform label = UiFactory.CreateRect("Label", button);
            UiFactory.Stretch(label);
            view.ActionLabel = UiFactory.AddFittedText(label, string.Empty, config.ResultButtonFontSize, Color.white);

            view.Hide();
            return view;
        }

        public void Show(string title, string actionLabel, Action action)
        {
            Title.text = title;
            ActionLabel.text = actionLabel;
            ActionButton.onClick.RemoveAllListeners();
            ActionButton.onClick.AddListener(() => action());
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
