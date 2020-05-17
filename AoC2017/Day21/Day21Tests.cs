using NUnit.Framework;

namespace Day21
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(new string[] {
"../.# => ##./#../...",
".#./..#/### => #..#/..../..../#..#"
            }, 2, 12, TestName = "PixelsOn(2) = 12")]
        public void PixelsOn(string[] rules, int iterations, int expected)
        {
            Program.Parse(rules);
            Assert.That(Program.PixelsOn(2), Is.EqualTo(12));
        }
    }
}
