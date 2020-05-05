using NUnit.Framework;

namespace Day05
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(new string[] {
"0",
"3",
"0",
"1",
"-3"
            }, 5, TestName = "Steps = 5")]
        public void Steps(string[] input, int expected)
        {
            Assert.That(Program.Steps(input), Is.EqualTo(expected));
        }
        [TestCase(new string[] {
"0",
"3",
"0",
"1",
"-3"
            }, 10, TestName = "Steps2 = 10")]
        public void Steps2(string[] input, int expected)
        {
            Assert.That(Program.Steps2(input), Is.EqualTo(expected));
        }
    }
}
