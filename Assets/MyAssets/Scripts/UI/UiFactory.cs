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

        /// <summary>Stretches a rect across its parent's width, at a fixed distance from the parent's top.</summary>
        public static void AnchorTopRow(RectTransform rect, float top, float height)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(0f, -(top + height));
            rect.offsetMax = new Vector2(0f, -top);
        }

        public static Image AddImage(RectTransform rect, Color color, bool raycastTarget = false)
        {
            var image = rect.gameObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = raycastTarget;
            return image;
        }

        /// <summary>Adds a rounded rectangle whose corners keep the given radius at any size.</summary>
        public static Image AddRounded(RectTransform rect, Color color, float cornerRadius, bool raycastTarget = false)
        {
            Image image = AddImage(rect, color, raycastTarget);
            image.sprite = GameSprites.RoundedRect;
            image.type = Image.Type.Sliced;
            image.pixelsPerUnitMultiplier = GameSprites.RoundedRectRadius / cornerRadius;
            return image;
        }

        /// <summary>Adds a tinted sprite that keeps its aspect and fits inside its rect.</summary>
        public static Image AddIcon(RectTransform rect, Sprite sprite, Color color)
        {
            Image image = AddImage(rect, color);
            image.sprite = sprite;
            image.preserveAspect = true;
            return image;
        }

        public static void AddSoftShadow(Graphic graphic, Color color)
        {
            var shadow = graphic.gameObject.AddComponent<Shadow>();
            shadow.effectColor = color;
            shadow.effectDistance = new Vector2(0f, -6f);
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

        /// <summary>Adds bold text that shrinks to fit its rect, for labels shared by portrait and landscape.</summary>
        public static Text AddFittedText(RectTransform rect, string content, int maxFontSize, Color color)
        {
            Text text = AddText(rect, content, maxFontSize, color);
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 10;
            text.resizeTextMaxSize = maxFontSize;
            text.fontStyle = FontStyle.Bold;
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
