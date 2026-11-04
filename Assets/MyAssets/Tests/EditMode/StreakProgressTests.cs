using NUnit.Framework;
using VibeCodeTest.Puzzle;

namespace VibeCodeTest.Tests
{
    public sealed class StreakProgressTests
    {
        private StreakProgress streak;

        [SetUp]
        public void SetUp()
        {
            streak = new StreakProgress(0);
        }

        [Test]
        public void NewStreak_HasNoDays()
        {
            Assert.AreEqual(0, streak.Days);
        }

        [Test]
        public void CompletingTheChosenLevel_GrantsOneDay()
        {
            Assert.IsTrue(streak.TryCelebrate(0));
            Assert.AreEqual(1, streak.Days);
        }

        [Test]
        public void CompletingItAgain_DoesNotCelebrateOrCount()
        {
            streak.TryCelebrate(0);

            Assert.IsFalse(streak.TryCelebrate(0));
            Assert.AreEqual(1, streak.Days);
        }

        [Test]
        public void CompletingAnotherLevel_ChangesNothing()
        {
            Assert.IsFalse(streak.TryCelebrate(1));
            Assert.AreEqual(0, streak.Days);
        }

        [Test]
        public void ChosenLevel_ComesFromTheCaller()
        {
            var other = new StreakProgress(2);

            Assert.IsFalse(other.TryCelebrate(0));
            Assert.IsTrue(other.TryCelebrate(2));
        }
    }
}
