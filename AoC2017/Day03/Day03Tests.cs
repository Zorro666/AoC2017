using NUnit.Framework;

namespace Day03
{
    [TestFixture]
    public class Tests
    {
        //  37  36  35  34  33  32  31
        //  38  17  16  15  14  13  30
        //  39  18   5   4   3  12  29
        //  40  19   6   1   2  11  28
        //  41  20   7   8   9  10  27
        //  42  21  22  23  24  25  26
        //  43  ->
        [Test]
        [TestCase(1, 0)]
        [TestCase(2, 1)]
        [TestCase(3, 2)]
        [TestCase(4, 1)]
        [TestCase(5, 2)]
        [TestCase(6, 1)]
        [TestCase(7, 2)]
        [TestCase(8, 1)]
        [TestCase(9, 2)]
        [TestCase(10, 3)]
        [TestCase(11, 2)]
        [TestCase(12, 3)]
        [TestCase(13, 4)]
        [TestCase(14, 3)]
        [TestCase(15, 2)]
        [TestCase(16, 3)]
        [TestCase(17, 4)]
        [TestCase(18, 3)]
        [TestCase(19, 2)]
        [TestCase(20, 3)]
        [TestCase(21, 4)]
        [TestCase(22, 3)]
        [TestCase(23, 2)]
        [TestCase(24, 3)]
        [TestCase(25, 4)]
        [TestCase(26, 5)]
        [TestCase(27, 4)]
        [TestCase(28, 3)]
        [TestCase(29, 4)]
        [TestCase(30, 5)]
        [TestCase(31, 6)]
        [TestCase(32, 5)]
        [TestCase(33, 4)]
        [TestCase(34, 3)]
        [TestCase(35, 4)]
        [TestCase(36, 5)]
        [TestCase(37, 6)]
        [TestCase(38, 5)]
        [TestCase(39, 4)]
        [TestCase(40, 3)]
        [TestCase(41, 4)]
        [TestCase(42, 5)]
        [TestCase(43, 6)]
        [TestCase(1024, 31)]
        public void SpiralSteps(int value, int expected)
        {
            Assert.That(Program.SpiralSteps(value), Is.EqualTo(expected));
        }

        // 147  142  133  122   59
        // 304    5    4    2   57
        // 330   10    1    1   54
        // 351   11   23   25   26
        // 362  747  806--->   ...
        [TestCase(1, 2)]
        [TestCase(2, 4)]
        [TestCase(3, 4)]
        [TestCase(4, 5)]
        [TestCase(5, 10)]
        [TestCase(10, 11)]
        [TestCase(11, 23)]
        [TestCase(20, 23)]
        [TestCase(23, 25)]
        [TestCase(24, 25)]
        [TestCase(25, 26)]
        [TestCase(26, 54)]
        [TestCase(54, 57)]
        [TestCase(57, 59)]
        [TestCase(59, 122)]
        [TestCase(122, 133)]
        [TestCase(133, 142)]
        [TestCase(142, 147)]
        [TestCase(147, 304)]
        [TestCase(304, 330)]
        [TestCase(330, 351)]
        [TestCase(351, 362)]
        [TestCase(362, 747)]
        [TestCase(747, 806)]
        public void SpiralSteps2(int value, int expected)
        {
            Assert.That(Program.SpiralSteps2(value), Is.EqualTo(expected));
        }
    }
}
