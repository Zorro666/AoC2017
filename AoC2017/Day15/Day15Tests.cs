using NUnit.Framework;

namespace Day15
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(65, 8921, 588)]
        public void CountMatches(int aStart, int bStart, int expected)
        {
            Assert.That(Program.CountMatches(aStart, bStart), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(65, 8921, 309)]
        public void CountMatches2(int aStart, int bStart, int expected)
        {
            Assert.That(Program.CountMatches2(aStart, bStart), Is.EqualTo(expected));
        }

    }
}
