using NUnit.Framework;

namespace Day08
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(new string[] {
"b inc 5 if a > 1",
"a inc 1 if b < 5",
"c dec -10 if a >= 1",
"c inc -20 if c == 10"
        },
            1, TestName = "LargestValue 1")]
        public void LargestValue(string[] program, int expected)
        {
            Program.Parse(program);
            Assert.That(Program.LargestValue, Is.EqualTo(expected));
        }
    }
}
