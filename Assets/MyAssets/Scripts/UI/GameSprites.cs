using System;
using UnityEngine;

namespace VibeCodeTest.UI
{
    /// <summary>Icons drawn from code, so the prototype ships no art assets.</summary>
    public static class GameSprites
    {
        private const int TextureSize = 192;
        private const int SamplesPerAxis = 3;

        private static readonly Color CatFur = new Color32(0x22, 0x20, 0x26, 0xFF);
        private static readonly Color CatNose = new Color32(0xF2, 0x9A, 0xA8, 0xFF);

        private static Sprite mark;
        private static Sprite cat;
        private static Sprite flame;
        private static Sprite disc;
        private static Sprite check;

        /// <summary>White X, tinted by the Image that shows it.</summary>
        public static Sprite Mark => mark != null ? mark : mark = Draw(ShadeMark);

        public static Sprite Cat => cat != null ? cat : cat = Draw(ShadeCat);

        /// <summary>White flame, tinted by the Image that shows it.</summary>
        public static Sprite Flame => flame != null ? flame : flame = Draw(ShadeFlame);

        /// <summary>White disc, tinted by the Image that shows it.</summary>
        public static Sprite Disc => disc != null ? disc : disc = Draw(ShadeDisc);

        /// <summary>White check mark, tinted by the Image that shows it.</summary>
        public static Sprite Check => check != null ? check : check = Draw(ShadeCheck);

        private static Sprite Draw(Func<Vector2, Color> shade)
        {
            var texture = new Texture2D(TextureSize, TextureSize, TextureFormat.RGBA32, false);
            var pixels = new Color[TextureSize * TextureSize];
            for (int y = 0; y < TextureSize; y++)
            {
                for (int x = 0; x < TextureSize; x++)
                {
                    pixels[y * TextureSize + x] = SamplePixel(x, y, shade);
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, TextureSize, TextureSize), new Vector2(0.5f, 0.5f));
        }

        private static Color SamplePixel(int x, int y, Func<Vector2, Color> shade)
        {
            Color sum = Color.clear;
            for (int sy = 0; sy < SamplesPerAxis; sy++)
            {
                for (int sx = 0; sx < SamplesPerAxis; sx++)
                {
                    var uv = new Vector2(
                        ((x + (sx + 0.5f) / SamplesPerAxis) / TextureSize * 2f) - 1f,
                        ((y + (sy + 0.5f) / SamplesPerAxis) / TextureSize * 2f) - 1f);
                    Color sample = shade(uv);
                    sum.r += sample.r * sample.a;
                    sum.g += sample.g * sample.a;
                    sum.b += sample.b * sample.a;
                    sum.a += sample.a;
                }
            }

            return sum.a <= 0f
                ? Color.clear
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

            if (InEllipse(mirrored, new Vector2(0.33f, 0.02f), 0.17f, 0.19f))
            {
                return InEllipse(mirrored, new Vector2(0.33f, 0.02f), 0.09f, 0.13f) ? CatFur : Color.white;
            }

            if (InTriangle(p, new Vector2(-0.09f, -0.27f), new Vector2(0.09f, -0.27f), new Vector2(0f, -0.38f)))
            {
                return CatNose;
            }

            bool muzzle = InEllipse(mirrored, new Vector2(0.16f, -0.42f), 0.3f, 0.24f);
            bool blaze = InTriangle(p, new Vector2(-0.2f, -0.3f), new Vector2(0.2f, -0.3f), new Vector2(0f, 0.55f));
            if (muzzle || blaze)
            {
                return Color.white;
            }

            bool head = InEllipse(p, new Vector2(0f, -0.1f), 0.86f, 0.68f);
            bool ear = InTriangle(mirrored, new Vector2(0.28f, 0.45f), new Vector2(0.82f, 0.2f), new Vector2(0.72f, 0.88f));
            return head || ear ? CatFur : Color.clear;
        }

        private static Color ShadeFlame(Vector2 p)
        {
            const float bottom = -0.92f;
            const float height = 1.86f;
            const float maxHalfWidth = 0.74f;
            const float taper = 0.62f;
            const float lean = 0.16f;

            float t = (p.y - bottom) / height;
            if (t < 0f || t > 1f)
            {
                return Color.clear;
            }

            float halfWidth = maxHalfWidth * Mathf.Pow(Mathf.Sin(Mathf.PI * Mathf.Pow(t, taper)), 0.8f);
            return Mathf.Abs(p.x - (lean * t * t)) <= halfWidth ? Color.white : Color.clear;
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
