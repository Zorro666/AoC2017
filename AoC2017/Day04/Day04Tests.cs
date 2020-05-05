using NUnit.Framework;

namespace Day04
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("aa bb cc dd ee", 1)]
        [TestCase("aa bb cc dd aa", 0)]
        [TestCase("aa bb cc dd aaa", 1)]
        public void ValidPassphrase(string passphrase, int expected)
        {
            string[] lines = new string[] { passphrase };
            Assert.That(Program.CountValidPassphrase(lines), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("abcde fghij", 1)]
        [TestCase("abcde xyz ecdab", 0)]
        [TestCase("a ab abc abd abf abj", 1)]
        [TestCase("iiii oiii ooii oooi oooo", 1)]
        [TestCase("oiii ioii iioi iiio", 0)]
        public void ValidPassphrase2(string passphrase, int expected)
        {
            string[] lines = new string[] { passphrase };
            Assert.That(Program.CountValidPassphrase2(lines), Is.EqualTo(expected));
        }
    }
}
