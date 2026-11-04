using System;
using System.Collections;
using UnityEngine;

namespace VibeCodeTest.UI
{
    /// <summary>Small time-based animations for uGUI, unaffected by the time scale.</summary>
    public static class UiAnimation
    {
        private const float BackOvershoot = 1.70158f;

        /// <summary>Calls <paramref name="apply"/> with progress from 0 to 1, ending on exactly 1.</summary>
        public static IEnumerator Run(float duration, Action<float> apply)
        {
            for (float elapsed = 0f; elapsed < duration; elapsed += Time.unscaledDeltaTime)
            {
                apply(elapsed / duration);
                yield return null;
            }

            apply(1f);
        }

        /// <summary>Grows from 0 to 1 with a small overshoot.</summary>
        public static float EaseOutBack(float t)
        {
            float shifted = t - 1f;
            return 1f + ((BackOvershoot + 1f) * shifted * shifted * shifted) + (BackOvershoot * shifted * shifted);
        }

        /// <summary>Scale that starts and ends at 1, peaking at 1 + <paramref name="amount"/> halfway.</summary>
        public static float Punch(float t, float amount)
        {
            return t >= 1f ? 1f : 1f + (amount * Mathf.Sin(Mathf.PI * t));
        }

        /// <summary>Sideways offset that swings and fades out, ending on exactly 0.</summary>
        public static float Shake(float t, float distance, float oscillations)
        {
            return t >= 1f ? 0f : distance * (1f - t) * Mathf.Sin(2f * Mathf.PI * oscillations * t);
        }
    }
}
