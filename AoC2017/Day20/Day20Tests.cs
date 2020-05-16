using NUnit.Framework;

namespace Day20
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(new string[] {
"p=< 3,0,0>, v=< 2,0,0>, a=<-1,0,0>",
"p=< 4,0,0>, v=< 0,0,0>, a=<-2,0,0>"
        }, 0, TestName = "ClosestParticle 0")]
        public void ClosestParticle(string[] inputs, int expected)
        {
            Program.Parse(inputs);
            Assert.That(Program.ClosestParticle(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(new string[] {
"p=< -6,0,0>, v=< 3,0,0>, a=< 0,0,0>",
"p=< -4,0,0>, v=< 2,0,0>, a=< 0,0,0>",
"p=< -2,0,0>, v=< 1,0,0>, a=< 0,0,0>",
"p=< 3,0,0>, v=<-1,0,0>, a=< 0,0,0>"
        }, 1, TestName = "RemainingParticles 1")]
        public void RemainingParticles(string[] inputs, int expected)
        {
            Program.Parse(inputs);
            Assert.That(Program.RemainingParticles(), Is.EqualTo(expected));
        }
    }
}
