using NUnit.Framework;

namespace Day18
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(new string[] {
"set a 1",
"add a 2",
"mul a a",
"mod a 5",
"snd a",
"set a 0",
"rcv a",
"jgz a -1",
"set a 1",
"jgz a -2"
        }, 4, TestName = "FindValidRcvFrequency 4")]
        public void FirstValidRcvFrequency(string[] program, int expected)
        {
            Program.Parse(program);
            Assert.That(Program.FirstValidRcvFrequency(), Is.EqualTo(expected));
        }
    }
}
