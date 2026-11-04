using UnityEngine;
using UnityEngine.UI;

namespace VibeCodeTest.UI
{
    /// <summary>Small helpers to build anchored uGUI elements from code.</summary>
    public static class UiFactory
    {
        public static RectTransform CreateRect(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            var rect = (RectTransform)gameObject.transform;
            rect.SetParent(parent, false);
            return rect;
        }

        /// <summary>Anchors a rect to a fraction of its parent, inset by <paramref name="inset"/> on every side.</summary>
        public static void Anchor(RectTransform rect, Vector2 min, Vector2 max, float inset = 0f)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = new Vector2(inset, inset);
            rect.offsetMax = new Vector2(-inset, -inset);
        }

        public static void Stretch(RectTransform rect, float inset = 0f)
        {
            Anchor(rect, Vector2.zero, Vector2.one, inset);
        }

        public static Image AddImage(RectTransform rect, Color color, bool raycastTarget = false)
        {
            var image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = raycastTarget;
            return image;
        }

        public static Text AddText(RectTransform rect, string content, int fontSize, Color color)
        {
            var text = rect.gameObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = content;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = TextAnchor.MiddleCenter;
            text.raycastTarget = false;
            return text;
        }

        public static Button AddButton(RectTransform rect, Image targetGraphic)
        {
            targetGraphic.raycastTarget = true;
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = targetGraphic;
            button.transition = Selectable.Transition.None;
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            return button;
        }
    }
}
