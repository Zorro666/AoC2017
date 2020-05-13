using NUnit.Framework;

namespace Day17
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(3, 2018, 638)]
        public void SpinLock(int stepsPerInsert, int insertCount, int expected)
        {
            Assert.That(Program.SpinLock(stepsPerInsert, insertCount), Is.EqualTo(expected));
        }
    }
}
