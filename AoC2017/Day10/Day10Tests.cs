using NUnit.Framework;

namespace Day10
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("3, 4, 1, 5", 12)]
        public void RunSequence(string lengths, int expected)
        {
            Assert.That(Program.RunSequence(5, lengths), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("", "a2582a3a0e66e6e86e3812dcb672a272")]
        [TestCase("AoC 2017", "33efeb34ea91902bb2f59c9920caa6cd")]
        [TestCase("1,2,3", "3efbe78a8d82f29979031a4aa0b16a9d")]
        [TestCase("1,2,4", "63960835bcdc130f0b66d7ff4f6a5a8e")]
        public void KnotHash(string input, string expectedHash)
        {
            Assert.That(Program.KnotHash(input), Is.EqualTo(expectedHash));
        }
    }
}
