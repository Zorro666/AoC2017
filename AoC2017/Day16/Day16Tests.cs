using NUnit.Framework;

namespace Day16
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("s1, x3/4, pe/b", 5, "baedc")]
        public void DanceMoves(string moves, int length, string expected)
        {
            Assert.That(Program.DanceMoves(moves, length), Is.EqualTo(expected));
        }
    }
}
