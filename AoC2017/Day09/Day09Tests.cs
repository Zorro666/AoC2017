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
    }
}
