using NUnit.Framework;
using VibeCodeTest.UI;

namespace VibeCodeTest.Tests
{
    public sealed class UiAnimationTests
    {
        private const float Tolerance = 0.0001f;

        [Test]
        public void EaseOutBack_GoesFromZeroToOneWithOvershoot()
        {
            Assert.AreEqual(0f, UiAnimation.EaseOutBack(0f), Tolerance);
            Assert.AreEqual(1f, UiAnimation.EaseOutBack(1f), Tolerance);
            Assert.Greater(UiAnimation.EaseOutBack(0.7f), 1f);
        }

        [Test]
        public void Punch_StartsAndEndsAtOneAndPeaksHalfway()
        {
            Assert.AreEqual(1f, UiAnimation.Punch(0f, 0.3f), Tolerance);
            Assert.AreEqual(1.3f, UiAnimation.Punch(0.5f, 0.3f), Tolerance);
            Assert.AreEqual(1f, UiAnimation.Punch(1f, 0.3f));
        }

        [Test]
        public void Shake_EndsExactlyAtRestAndSwingsBothWays()
        {
            const float oscillations = 3f;

            Assert.AreEqual(0f, UiAnimation.Shake(1f, 14f, oscillations));
            Assert.Greater(UiAnimation.Shake(1f / 12f, 14f, oscillations), 0f);
            Assert.Less(UiAnimation.Shake(3f / 12f, 14f, oscillations), 0f);
        }
    }
}
