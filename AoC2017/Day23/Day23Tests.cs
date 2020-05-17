using NUnit.Framework;

namespace Day23
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(new string[] {
"set a 4",
"sub a 1",
"mul b b",
"jnz a -2"
        }, 4, TestName = "MulCount 4")]
        public void MulCount(string[] program, int expected)
        {
            Program.Parse(program);
            Assert.That(Program.MulCount(), Is.EqualTo(expected));
        }
    }
}
