using NUnit.Framework;

namespace Day13
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(new string[] {
"0: 3",
"1: 2",
"4: 4",
"6: 4"
        }, 24)]
        public void Severity(string[] layers, int expected)
        {
            Program.Parse(layers);
            Program.Escape();
            Assert.That(Program.Severity, Is.EqualTo(expected));
        }
    }
}
