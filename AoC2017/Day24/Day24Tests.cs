using NUnit.Framework;

namespace Day24
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(new string[] {
"0/2",
"2/2",
"2/3",
"3/4",
"3/5",
"0/1",
"10/1",
"9/10"
        }, 31, TestName = "StrongestBridge 31")]
        public void StrongestBridge(string[] input, int expected)
        {
            Program.Parse(input);
            Assert.That(Program.StrongestBridge(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(new string[] {
"0/2",
"2/2",
"2/3",
"3/4",
"3/5",
"0/1",
"10/1",
"9/10"
        }, 19, TestName = "StrongestLongestBridge 19")]
        public void StrongestLongestBridge(string[] input, int expected)
        {
            Program.Parse(input);
            Assert.That(Program.StrongestLongestBridge(), Is.EqualTo(expected));
        }
    }
}
