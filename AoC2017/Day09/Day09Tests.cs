using NUnit.Framework;

namespace Day09
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("{}", 1)]
        [TestCase("{{{}}}", 6)]
        [TestCase("{{},{}}", 5)]
        [TestCase("{{{},{},{{}}}}", 16)]
        [TestCase("{<a>,<a>,<a>,<a>}", 1)]
        [TestCase("{{<ab>},{<ab>},{<ab>},{<ab>}}", 9)]
        [TestCase("{{<!!>},{<!!>},{<!!>},{<!!>}}", 9)]
        [TestCase("{{<a!>},{<a!>},{<a!>},{<ab>}}", 3)]
        public void Score(string group, int expected)
        {
            Assert.That(Program.Score(group), Is.EqualTo(expected));
        }

        [TestCase("<>", 0)]
        [TestCase("<random characters>", 17)]
        [TestCase("<<<<>", 3)]
        [TestCase("<{!>}>", 2)]
        [TestCase("<!!>", 0)]
        [TestCase("<!!!>>", 0)]
        [TestCase("<{o\"i!a,<{i<a>", 10)]
        public void CountGarbage(string group, int expected)
        {
            Assert.That(Program.CountGarbage(group), Is.EqualTo(expected));
        }
    }
}
