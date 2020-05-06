using NUnit.Framework;

namespace Day06
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("0  2   7   0", 5)]
        public void InfiniteLoop(string start, int expected)
        {
            Assert.That(Program.InfiniteLoop(start).stepCount, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("0  2   7   0", 4)]
        public void InfiniteLoop2(string start, int expected)
        {
            Assert.That(Program.InfiniteLoop(start).loop, Is.EqualTo(expected));
        }
    }
}
