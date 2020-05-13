using NUnit.Framework;

namespace Day16
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("s1, x3/4, pe/b", 5, 1, "baedc")]
        [TestCase("s1, x3/4, pe/b", 5, 2, "ceadb")]
        public void DanceMoves(string moves, int length, int repeat, string expected)
        {
            Assert.That(Program.DanceMoves(moves, length, repeat, true), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("s1, x3/4, pe/b", 5, 10)]
        [TestCase("s1, x3/4, pe/b", 5, 100)]
        [TestCase("s1, x3/4, pe/b", 5, 1000)]
        public void DanceMovesRepeatCycle(string moves, int length, int repeat)
        {
            var bruteForce = Program.DanceMoves(moves, length, repeat, true);
            Assert.That(Program.DanceMoves(moves, length, repeat, false), Is.EqualTo(bruteForce));
        }
    }
}
