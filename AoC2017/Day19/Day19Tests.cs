using NUnit.Framework;

namespace Day19
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(new string[] {
"     |          ",
"     |  +--+    ",
"     A  |  C    ",
" F---|----E|--+ ",
"     |  |  |  D ",
"     +B-+  +--+ "
        }, "ABCDEF", TestName = "FindRoute (ABCDEF)")]
        public void FindRoute(string[] map, string expected)
        {
            Program.Parse(map);
            Assert.That(Program.FindRoute(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(new string[] {
"     |          ",
"     |  +--+    ",
"     A  |  C    ",
" F---|----E|--+ ",
"     |  |  |  D ",
"     +B-+  +--+ "
        }, 38, TestName = "RouteLength 38")]
        public void RouteLength(string[] map, long expected)
        {
            Program.Parse(map);
            Assert.That(Program.RouteLength(), Is.EqualTo(expected));
        }
    }
}
