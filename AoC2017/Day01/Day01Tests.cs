using NUnit.Framework;

namespace Day01
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase("1122", 3)]
        [TestCase("1111", 4)]
        [TestCase("1234", 0)]
        [TestCase("91212129", 9)]
        public void CaptchaSum(string source, int expectedSum)
        {
            Program.Parse(new string[] { source });
            Assert.That(Program.CaptchaSum, Is.EqualTo(expectedSum));
        }

        [Test]
        [TestCase("1212", 6)]
        [TestCase("1221", 0)]
        [TestCase("123425", 4)]
        [TestCase("123123", 12)]
        [TestCase("12131415", 4)]
        public void CaptchaSum2(string source, int expectedSum)
        {
            Program.Parse(new string[] { source });
            Assert.That(Program.CaptchaSum2, Is.EqualTo(expectedSum));
        }
    }
}
