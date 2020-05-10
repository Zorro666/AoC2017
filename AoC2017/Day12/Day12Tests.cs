using NUnit.Framework;

namespace Day12
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(new string[] {
"0 <-> 2",
"1 <-> 1",
"2 <-> 0, 3, 4",
"3 <-> 2, 4",
"4 <-> 2, 3, 6",
"5 <-> 6",
"6 <-> 4, 5"
        }, 0, 6, TestName = "ProgramCount(0) = 6")]
        public void ProgramCount(string[] connections, int programID, int expected)
        {
            Program.Parse(connections);
            Assert.That(Program.ProgramCount(programID), Is.EqualTo(expected));
        }
    }
}
