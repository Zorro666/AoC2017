using NUnit.Framework;

namespace Day02
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(new string[] {
"5  1 9 5",
"7 5    3",
"2 4    6 8" }, 18, TestName = "Checksum 18")]
        public void CheckSum(string[] input, int expected)
        {
            Program.Parse(input);
            Assert.That(Program.Checksum, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(new string[] {
"5 9 2 8",
"9 4 7 3",
"3 8 6 5" }, 9, TestName = "Checksum2 9")]
        public void CheckSum2(string[] input, int expected)
        {
            Program.Parse(input);
            Assert.That(Program.Checksum2, Is.EqualTo(expected));
        }
    }
}
