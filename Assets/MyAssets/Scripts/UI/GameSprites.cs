using System;
using UnityEngine;

namespace VibeCodeTest.UI
{
    /// <summary>Icons drawn from code, so the prototype ships no art assets. White ones are tinted by their Image.</summary>
    public static class GameSprites
    {
        /// <summary>Corner radius of <see cref="RoundedRect"/>, in canvas units at a multiplier of 1.</summary>
        public const float RoundedRectRadius = 16f;

        private const int IconSize = 192;
        private const int RoundedRectSize = 64;
        private const int SamplesPerAxis = 3;

        private static readonly Color CatFur = new Color32(0x22, 0x20, 0x26, 0xFF);
        private static readonly Color CatPink = new Color32(0xF2, 0x9A, 0xA8, 0xFF);
        private static readonly Color CatEar = new Color32(0x6B, 0x4A, 0x52, 0xFF);
        private static readonly Color GiftBox = new Color32(0xFF, 0xC7, 0x3A, 0xFF);
        private static readonly Color GiftLid = new Color32(0xFF, 0xDA, 0x66, 0xFF);
        private static readonly Color GiftRibbon = new Color32(0xE8, 0x4A, 0x3C, 0xFF);

        private static Sprite roundedRect;
        private static Sprite mark;
        private static Sprite cat;
        private static Sprite sun;
        private static Sprite disc;
        private static Sprite check;
        private static Sprite gift;
        private static Sprite fish;
        private static Sprite backArrow;

        public static Sprite RoundedRect => roundedRect != null ? roundedRect : roundedRect = DrawRoundedRect();

        public static Sprite Mark => mark != null ? mark : mark = Draw(ShadeMark);

        public static Sprite Cat => cat != null ? cat : cat = Draw(ShadeCat);

        public static Sprite Sun => sun != null ? sun : sun = Draw(ShadeSun);

        public static Sprite Disc => disc != null ? disc : disc = Draw(ShadeDisc);

        public static Sprite Check => check != null ? check : check = Draw(ShadeCheck);

        public static Sprite Gift => gift != null ? gift : gift = Draw(ShadeGift);

        public static Sprite Fish => fish != null ? fish : fish = Draw(ShadeFish);

        public static Sprite BackArrow => backArrow != null ? backArrow : backArrow = Draw(ShadeBackArrow);

        private static Sprite Draw(Func<Vector2, Color> shade)
        {
            Texture2D texture = DrawTexture(IconSize, shade);
            return Sprite.Create(texture, new Rect(0, 0, IconSize, IconSize), new Vector2(0.5f, 0.5f));
        }

        private static Sprite DrawRoundedRect()
        {
            const float radius = 0.5f;
            Texture2D texture = DrawTexture(RoundedRectSize, p =>
            {
                float x = Mathf.Max(Mathf.Abs(p.x) - (1f - radius), 0f);
                float y = Mathf.Max(Mathf.Abs(p.y) - (1f - radius), 0f);
                return (x * x) + (y * y) <= radius * radius ? Color.white : Color.clear;
            });
            float border = RoundedRectRadius + 1f;
            return Sprite.Create(
                texture,
                new Rect(0, 0, RoundedRectSize, RoundedRectSize),
                new Vector2(0.5f, 0.5f),
                100f,
                0,
                SpriteMeshType.FullRect,
                new Vector4(border, border, border, border));
        }

        private static Texture2D DrawTexture(int size, Func<Vector2, Color> shade)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var pixels = new Color[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    pixels[(y * size) + x] = SamplePixel(x, y, size, shade);
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        private static Color SamplePixel(int x, int y, int size, Func<Vector2, Color> shade)
        {
            Color sum = Color.clear;
            for (int sy = 0; sy < SamplesPerAxis; sy++)
            {
                for (int sx = 0; sx < SamplesPerAxis; sx++)
                {
                    var uv = new Vector2(
                        ((x + ((sx + 0.5f) / SamplesPerAxis)) / size * 2f) - 1f,
                        ((y + ((sy + 0.5f) / SamplesPerAxis)) / size * 2f) - 1f);
                    Color sample = shade(uv);
                    sum.r += sample.r * sample.a;
                    sum.g += sample.g * sample.a;
                    sum.b += sample.b * sample.a;
                    sum.a += sample.a;
                }
            }

            return sum.a <= 0f
                ? new Color(1f, 1f, 1f, 0f)
                : new Color(sum.r / sum.a, sum.g / sum.a, sum.b / sum.a, sum.a / (SamplesPerAxis * SamplesPerAxis));
        }

        private static Color ShadeMark(Vector2 p)
        {
            const float reach = 0.62f;
            const float thickness = 0.17f;
            float diagonalA = DistanceToSegment(p, new Vector2(-reach, -reach), new Vector2(reach, reach));
            float diagonalB = DistanceToSegment(p, new Vector2(-reach, reach), new Vector2(reach, -reach));
            return Mathf.Min(diagonalA, diagonalB) <= thickness ? Color.white : Color.clear;
        }

        private static Color ShadeCat(Vector2 p)
        {
            var mirrored = new Vector2(Mathf.Abs(p.x), p.y);
            var eye = new Vector2(0.31f, 0.0f);

            if (InEllipse(p, new Vector2(eye.x - 0.05f, eye.y + 0.08f), 0.05f, 0.05f)
                || InEllipse(p, new Vector2(-eye.x - 0.05f, eye.y + 0.08f), 0.05f, 0.05f))
            {
                return Color.white;
            }

            if (InEllipse(mirrored, eye, 0.21f, 0.23f))
            {
                return InEllipse(mirrored, eye, 0.13f, 0.17f) ? CatFur : Color.white;
            }

            if (InTriangle(p, new Vector2(-0.08f, -0.25f), new Vector2(0.08f, -0.25f), new Vector2(0f, -0.35f)))
            {
                return CatPink;
            }

            bool whisker = DistanceToSegment(mirrored, new Vector2(0.55f, -0.3f), new Vector2(0.97f, -0.22f)) <= 0.016f
                || DistanceToSegment(mirrored, new Vector2(0.55f, -0.42f), new Vector2(0.97f, -0.48f)) <= 0.016f;
            if (whisker)
            {
                return CatFur;
            }

            bool muzzle = InEllipse(mirrored, new Vector2(0.2f, -0.44f), 0.38f, 0.27f);
            bool chin = InEllipse(p, new Vector2(0f, -0.52f), 0.52f, 0.26f);
            bool blaze = InTriangle(p, new Vector2(-0.17f, -0.3f), new Vector2(0.17f, -0.3f), new Vector2(0f, 0.46f));
            if (muzzle || chin || blaze)
            {
                return Color.white;
            }

            if (InTriangle(mirrored, new Vector2(0.44f, 0.44f), new Vector2(0.74f, 0.32f), new Vector2(0.69f, 0.76f)))
            {
                return CatEar;
            }

            bool head = InEllipse(p, new Vector2(0f, -0.1f), 0.88f, 0.7f);
            bool ear = InTriangle(mirrored, new Vector2(0.24f, 0.42f), new Vector2(0.86f, 0.18f), new Vector2(0.75f, 0.94f));
            return head || ear ? CatFur : Color.clear;
        }

        private static Color ShadeSun(Vector2 p)
        {
            const float points = 8f;
            const float baseRadius = 0.64f;
            const float rayLength = 0.33f;
            const float raySharpness = 1.6f;

            float angle = Mathf.Atan2(p.x, p.y);
            float wave = (1f + Mathf.Cos(points * angle)) / 2f;
            float radius = baseRadius + (rayLength * Mathf.Pow(wave, raySharpness));
            return p.magnitude <= radius ? Color.white : Color.clear;
        }

        private static Color ShadeDisc(Vector2 p)
        {
            return p.sqrMagnitude <= 0.92f ? Color.white : Color.clear;
        }

        private static Color ShadeCheck(Vector2 p)
        {
            const float thickness = 0.14f;
            var corner = new Vector2(-0.14f, -0.42f);
            float shortArm = DistanceToSegment(p, new Vector2(-0.55f, 0f), corner);
            float longArm = DistanceToSegment(p, corner, new Vector2(0.58f, 0.46f));
            return Mathf.Min(shortArm, longArm) <= thickness ? Color.white : Color.clear;
        }

        private static Color ShadeGift(Vector2 p)
        {
            var mirrored = new Vector2(Mathf.Abs(p.x), p.y);

            if (mirrored.x <= 0.14f && p.y >= -0.86f && p.y <= 0.34f)
            {
                return GiftRibbon;
            }

            bool lid = mirrored.x <= 0.84f && p.y >= 0.02f && p.y <= 0.34f;
            if (lid)
            {
                return GiftLid;
            }

            bool box = mirrored.x <= 0.72f && p.y >= -0.86f && p.y < 0.02f;
            if (box)
            {
                return GiftBox;
            }

            bool bow = InEllipse(mirrored, new Vector2(0.3f, 0.56f), 0.3f, 0.2f);
            bool knot = InEllipse(p, new Vector2(0f, 0.46f), 0.13f, 0.13f);
            return bow || knot ? GiftRibbon : Color.clear;
        }

        private static Color ShadeFish(Vector2 p)
        {
            if (InEllipse(p, new Vector2(-0.38f, 0.1f), 0.08f, 0.08f))
            {
                return Color.clear;
            }

            bool body = InEllipse(p, new Vector2(-0.14f, 0f), 0.68f, 0.44f);
            bool tail = InTriangle(p, new Vector2(0.38f, 0f), new Vector2(0.9f, 0.46f), new Vector2(0.9f, -0.46f));
            bool fin = InTriangle(p, new Vector2(-0.3f, 0.3f), new Vector2(0.14f, 0.3f), new Vector2(-0.02f, 0.66f));
            return body || tail || fin ? Color.white : Color.clear;
        }

        private static Color ShadeBackArrow(Vector2 p)
        {
            const float thickness = 0.13f;
            var tip = new Vector2(-0.62f, 0f);
            float shaft = DistanceToSegment(p, tip, new Vector2(0.66f, 0f));
            float upper = DistanceToSegment(p, tip, new Vector2(-0.08f, 0.54f));
            float lower = DistanceToSegment(p, tip, new Vector2(-0.08f, -0.54f));
            return Mathf.Min(shaft, Mathf.Min(upper, lower)) <= thickness ? Color.white : Color.clear;
        }

        private static float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
        {
            Vector2 segment = b - a;
            float along = Mathf.Clamp01(Vector2.Dot(p - a, segment) / segment.sqrMagnitude);
            return Vector2.Distance(p, a + (segment * along));
        }

        private static bool InEllipse(Vector2 p, Vector2 center, float radiusX, float radiusY)
        {
            float x = (p.x - center.x) / radiusX;
            float y = (p.y - center.y) / radiusY;
            return (x * x) + (y * y) <= 1f;
        }

        private static bool InTriangle(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            float d1 = Cross(p, a, b);
            float d2 = Cross(p, b, c);
            float d3 = Cross(p, c, a);
            bool hasNegative = d1 < 0f || d2 < 0f || d3 < 0f;
            bool hasPositive = d1 > 0f || d2 > 0f || d3 > 0f;
            return !(hasNegative && hasPositive);
        }

        private static float Cross(Vector2 p, Vector2 a, Vector2 b)
        {
            return ((p.x - b.x) * (a.y - b.y)) - ((a.x - b.x) * (p.y - b.y));
        }
    }
}
