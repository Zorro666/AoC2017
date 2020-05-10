using NUnit.Framework;

namespace Day11
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("ne,ne,ne", 3)]
        [TestCase("ne,ne,sw,sw", 0)]
        [TestCase("ne,ne,s,s", 2)]
        [TestCase("se,sw,se,sw,sw", 3)]
        public void HexSteps(string moves, int expected)
        {
            Assert.That(Program.HexSteps(moves).end, Is.EqualTo(expected));
        }
    }
}
