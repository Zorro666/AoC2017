using NUnit.Framework;

namespace Day14
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("flqrgnkx", 8108)]
        public void UsedSquares(string hash, int expected)
        {
            Assert.That(Program.UsedSquares(hash), Is.EqualTo(expected));
        }
    }
}
