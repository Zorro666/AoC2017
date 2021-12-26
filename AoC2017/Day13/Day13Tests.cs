using NUnit.Framework;

namespace Day13
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(new string[] {
"0: 3",
"1: 2",
"4: 4",
"6: 4"
        }, 24, TestName = "Escape(0) 24")]
        public void Escape(string[] layers, int expected)
        {
            Program.Parse(layers);
            Assert.That(Program.Escape(0), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(new string[] {
"0: 3",
"1: 2",
"4: 4",
"6: 4"
        }, 10, TestName = "SmallestDelay 10")]
        public void SmallestDelay(string[] layers, int expected)
        {
            Program.Parse(layers);
            Assert.That(Program.SmallestDelay(), Is.EqualTo(expected));
        }
    }
}
