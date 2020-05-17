using NUnit.Framework;

namespace Day22
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(new string[] {
"..#",
"#..",
"..."
        }, 7, 5, TestName = "InfectedBurstCount(7) = 5")]
        [TestCase(new string[] {
"..#",
"#..",
"..."
        }, 70, 41, TestName = "InfectedBurstCount(70) = 41")]
        [TestCase(new string[] {
"..#",
"#..",
"..."
        }, 10000, 5587, TestName = "InfectedBurstCount(10000) = 5587")]
        public void Day22(string[] start, int bursts, int expected)
        {
            Program.Parse(start);
            Assert.That(Program.InfectedBurstCount(bursts), Is.EqualTo(expected));
        }
    }
}
