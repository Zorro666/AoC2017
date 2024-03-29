﻿using NUnit.Framework;

namespace Day07
{
    [TestFixture]
    public class Tests
    {
        [Test]
        [TestCase(new string[] {
"pbga (66)",
"xhth (57)",
"ebii (61)",
"havc (66)",
"ktlj (57)",
"fwft (72) -> ktlj, cntj, xhth",
"qoyq (66)",
"padx (45) -> pbga, havc, qoyq",
"tknk (41) -> ugml, padx, fwft",
"jptl (61)",
"ugml (68) -> gyxo, ebii, jptl",
"gyxo (61)",
"cntj (57)"
        }, "tknk", TestName = "BottomProgram tknk")]
        public void BottomProgram(string[] program, string expected)
        {
            Program.Parse(program);
            Assert.That(Program.BottomProgram, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(new string[] {
"pbga (66)",
"xhth (57)",
"ebii (61)",
"havc (66)",
"ktlj (57)",
"fwft (72) -> ktlj, cntj, xhth",
"qoyq (66)",
"padx (45) -> pbga, havc, qoyq",
"tknk (41) -> ugml, padx, fwft",
"jptl (61)",
"ugml (68) -> gyxo, ebii, jptl",
"gyxo (61)",
"cntj (57)"
        }, 60, TestName = "BalanceTower 60")]
        public void BalanceTower(string[] program, int expected)
        {
            Program.Parse(program);
            Program.ComputeBalanceTower();
            Assert.That(Program.BalanceTower, Is.EqualTo(expected));
        }
    }
}
