using NUnit.Framework;
using VibeCodeTest.Puzzle;

namespace VibeCodeTest.Tests
{
    public sealed class LevelSolverTests
    {
        private static readonly string[] OneRegionPerRow = { "AAAA", "BBBB", "CCCC", "DDDD" };

        [Test]
        public void FourByFourWithOwnRegionPerRow_HasBothNonTouchingPermutations()
        {
            var solutions = LevelSolver.FindSolutions(LevelDefinition.FromRows(OneRegionPerRow), 10);

            Assert.AreEqual(2, solutions.Count);
            CollectionAssert.AreEqual(new[] { 1, 3, 0, 2 }, solutions[0]);
            CollectionAssert.AreEqual(new[] { 2, 0, 3, 1 }, solutions[1]);
        }

        [Test]
        public void StarterCat_KeepsOnlyTheSolutionsThatContainIt()
        {
            var solutions = LevelSolver.FindSolutions(LevelDefinition.FromRows(OneRegionPerRow, 0, 2), 10);

            Assert.AreEqual(1, solutions.Count);
            CollectionAssert.AreEqual(new[] { 2, 0, 3, 1 }, solutions[0]);
        }

        [Test]
        public void ThreeByThree_HasNoSolutionBecauseCatsWouldTouch()
        {
            var level = LevelDefinition.FromRows(new[] { "AAA", "BBB", "CCC" });

            Assert.IsEmpty(LevelSolver.FindSolutions(level, 10));
        }

        [Test]
        public void Region_CannotHoldTwoCats()
        {
            // Two whole rows belong to region A, so any placement puts two cats in it.
            var level = LevelDefinition.FromRows(new[] { "AAAA", "AAAA", "BCDB", "BCDB" });

            Assert.IsEmpty(LevelSolver.FindSolutions(level, 10));
        }

        [Test]
        public void Limit_StopsTheSearch()
        {
            Assert.AreEqual(1, LevelSolver.FindSolutions(LevelDefinition.FromRows(OneRegionPerRow), 1).Count);
        }
    }
}
