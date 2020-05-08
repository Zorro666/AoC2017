using System;

/*

--- Day 7: Recursive Circus ---

Wandering further through the circuits of the computer, you come upon a tower of programs that have gotten themselves into a bit of trouble.
A recursive algorithm has gotten out of hand, and now they're balanced precariously in a large tower.

One program at the bottom supports the entire tower.
It's holding a large disc, and on the disc are balanced several more sub-towers.
At the bottom of these sub-towers, standing on the bottom disc, are other programs, each holding their own disc, and so on.
At the very tops of these sub-sub-sub-...-towers, many programs stand simply keeping the disc below them balanced but with no disc of their own.

You offer to help, but first you need to understand the structure of these towers.
You ask each program to yell out their name, their weight, and (if they're holding a disc) the names of the programs immediately above them balancing on that disc.
You write this information down (your puzzle input).
Unfortunately, in their panic, they don't do this in an orderly fashion; by the time you're done, you're not sure which program gave which information.

For example, if your list is the following:

pbga (66)
xhth (57)
ebii (61)
havc (66)
ktlj (57)
fwft (72) -> ktlj, cntj, xhth
qoyq (66)
padx (45) -> pbga, havc, qoyq
tknk (41) -> ugml, padx, fwft
jptl (61)
ugml (68) -> gyxo, ebii, jptl
gyxo (61)
cntj (57)

...then you would be able to recreate the structure of the towers that looks like this:

                gyxo
              /     
         ugml - ebii
       /      \     
      |         jptl
      |        
      |         pbga
     /        /
tknk --- padx - havc
     \        \
      |         qoyq
      |             
      |         ktlj
       \      /     
         fwft - cntj
              \     
                xhth
In this example, tknk is at the bottom of the tower (the bottom program), and is holding up ugml, padx, and fwft.
Those programs are, in turn, holding up other programs; in this example, none of those programs are holding up any other programs, and are all the tops of their own towers.
(The actual tower balancing in front of you is much larger.)

Before you're ready to help them, you need to make sure your information is correct.

What is the name of the bottom program?

Your puzzle answer was bpvhwhh.

--- Part Two ---

The programs explain the situation: they can't get down. 
Rather, they could get down, if they weren't expending all of their energy trying to keep the tower balanced. 
Apparently, one program has the wrong weight, and until it's fixed, they're stuck here.

For any program holding a disc, each program standing on that disc forms a sub-tower. 
Each of those sub-towers are supposed to be the same weight, or the disc itself isn't balanced. 
The weight of a tower is the sum of the weights of the programs in that tower.

In the example above, this means that for ugml's disc to be balanced, gyxo, ebii, and jptl must all have the same weight, and they do: 61.

However, for tknk to be balanced, each of the programs standing on its disc and all programs above it must each match. 
This means that the following sums must all be the same:

ugml + (gyxo + ebii + jptl) = 68 + (61 + 61 + 61) = 251
padx + (pbga + havc + qoyq) = 45 + (66 + 66 + 66) = 243
fwft + (ktlj + cntj + xhth) = 72 + (57 + 57 + 57) = 243

As you can see, tknk's disc is unbalanced: ugml's stack is heavier than the other two. 
Even though the nodes above ugml are balanced, ugml itself is too heavy: 
it needs to be 8 units lighter for its stack to weigh 243 and keep the towers balanced. 
If this change were made, its weight would be 60.

Given that exactly one program is the wrong weight, what would its weight need to be to balance the entire tower?

*/

namespace Day07
{
    class Program
    {
        readonly static int MAX_NUM_PROGRAMS = 2048;
        readonly static int MAX_NUM_CHILDREN = 128;
        readonly static string[] sNames = new string[MAX_NUM_PROGRAMS];
        readonly static int[] sWeights = new int[MAX_NUM_PROGRAMS];
        readonly static int[] sTotalWeights = new int[MAX_NUM_PROGRAMS];
        readonly static string[,] sChildren = new string[MAX_NUM_PROGRAMS, MAX_NUM_CHILDREN];
        readonly static int[] sChildrenCounts = new int[MAX_NUM_PROGRAMS];
        readonly static string[] sParents = new string[MAX_NUM_PROGRAMS];
        readonly static int[] sDepths = new int[MAX_NUM_PROGRAMS];
        static int sProgramCount = 0;
        static int sMaxDepth = 0;

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);
            ComputeBalanceTower();

            if (part1)
            {
                var result1 = BottomProgram;
                Console.WriteLine($"Day07 : Result1 {result1}");
                var expected = "bpvhwhh";
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = BalanceTower;
                Console.WriteLine($"Day07 : Result2 {result2}");
                var expected = 256;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] programs)
        {
            BottomProgram = null;
            BalanceTower = 0;
            sProgramCount = 0;
            sMaxDepth = int.MinValue;
            if (programs.Length > MAX_NUM_PROGRAMS)
            {
                throw new InvalidProgramException($"Invalid input too many programs {programs.Length} MAX:{MAX_NUM_PROGRAMS}");
            }

            foreach (var line in programs)
            {
                var tokens = line.Trim().Split();
                if (tokens.Length < 2)
                {
                    throw new InvalidProgramException($"Invalid line need at least 2 tokens found {tokens.Length}");
                }

                var name = tokens[0];

                var weightToken = tokens[1];
                if (weightToken[0] != '(')
                {
                    throw new InvalidProgramException($"Invalid line incorrect weight token '{weightToken}' expected '('");
                }
                if (weightToken[^1] != ')')
                {
                    throw new InvalidProgramException($"Invalid line incorrect weight token '{weightToken}' expected ')'");
                }

                var weight = int.Parse(weightToken.Substring(1, weightToken.Length - 2));

                var childCount = tokens.Length - 3;
                if (childCount > MAX_NUM_CHILDREN)
                {
                    throw new InvalidProgramException($"Invalid line too many children {childCount} MAX:{MAX_NUM_CHILDREN}");
                }
                if (childCount > 0)
                {
                    if (tokens[2] != "->")
                    {
                        throw new InvalidProgramException($"Invalid line incorrect depends on token '{tokens[2]}' expected '->'");
                    }
                    for (var c = 0; c < childCount; ++c)
                    {
                        sChildren[sProgramCount, c] = tokens[c + 3].TrimEnd(',');
                    }
                }
                sChildrenCounts[sProgramCount] = childCount;

                // Parents are computed in pass 2
                sParents[sProgramCount] = null;

                sNames[sProgramCount] = name;
                sWeights[sProgramCount] = weight;
                sTotalWeights[sProgramCount] = 0;
                ++sProgramCount;
            }

            // Pass2 : compute Parents
            for (var i = 0; i < sProgramCount; ++i)
            {
                var childCount = sChildrenCounts[i];
                for (var c = 0; c < childCount; ++c)
                {
                    var child = sChildren[i, c];
                    var childIndex = FindProgram(child);
                    if (sParents[childIndex] != null)
                    {
                        throw new InvalidProgramException($"Parent already set for child '{child}' for program[{i}] '{sNames[i]}'");
                    }
                    sParents[childIndex] = sNames[i];
                }
            }

            // Pass3 : compute depths
            for (var i = 0; i < sProgramCount; ++i)
            {
                var depth = 0;
                var node = i;
                while (sParents[node] != null)
                {
                    ++depth;
                    node = FindProgram(sParents[node]);
                }
                sDepths[i] = depth;
                if (depth == 0)
                {
                    if (BottomProgram != null)
                    {
                        throw new InvalidProgramException($"BottomProgram already set '{BottomProgram}' at program[{i}] '{sNames[i]}");
                    }
                    BottomProgram = sNames[i];
                }
                sMaxDepth = Math.Max(sMaxDepth, depth);
            }
        }

        public static void ComputeBalanceTower()
        {
            var brokenNode = int.MinValue;
            var unbalancedAmount = 0;
            for (var d = sMaxDepth; d >= 0; --d)
            {
                for (var i = 0; i < sProgramCount; ++i)
                {
                    if (sDepths[i] == d)
                    {
                        sTotalWeights[i] = sWeights[i];
                        var expectedChildWeight = int.MinValue;
                        for (var c = 0; c < sChildrenCounts[i]; ++c)
                        {
                            var childName = sChildren[i, c];
                            var childIndex = FindProgram(childName);
                            var childWeight = sTotalWeights[childIndex];
                            sTotalWeights[i] += sTotalWeights[childIndex];
                            if (expectedChildWeight == int.MinValue)
                            {
                                expectedChildWeight = childWeight;
                            }
                            if (expectedChildWeight != childWeight)
                            {
                                if (brokenNode == int.MinValue)
                                {
                                    brokenNode = i;
                                    unbalancedAmount = expectedChildWeight - childWeight;
                                }
                                Console.WriteLine($"Unbalanced Node '{sNames[i]}' W:{sWeights[i]} TW:{sTotalWeights[i]} Expected:{expectedChildWeight} Got:{childWeight}");
                                if (unbalancedAmount != (expectedChildWeight - childWeight))
                                {
                                    throw new InvalidProgramException($"Unbalanced amount does not match '{sNames[i]}' Expected:{unbalancedAmount} Got:{expectedChildWeight - childWeight}");
                                }
                            }
                        }
                    }
                }
            }

            Console.WriteLine($"Broken Node '{sNames[brokenNode]}' W:{sWeights[brokenNode]} TW:{sTotalWeights[brokenNode]}");
            var badChildIndex = int.MinValue;
            for (var c1 = 0; c1 < sChildrenCounts[brokenNode]; ++c1)
            {
                var child1Name = sChildren[brokenNode, c1];
                var child1Index = FindProgram(child1Name);
                var child1Weight = sTotalWeights[child1Index];
                var countMatches = 0;
                for (var c2 = 0; c2 < sChildrenCounts[brokenNode]; ++c2)
                {
                    if (c2 == c1)
                    {
                        continue;
                    }
                    var child2Name = sChildren[brokenNode, c2];
                    var child2Index = FindProgram(child2Name);
                    var child2Weight = sTotalWeights[child2Index];
                    if (child1Weight == child2Weight)
                    {
                        ++countMatches;
                    }
                }
                if (countMatches == 0)
                {
                    if (badChildIndex == int.MinValue)
                    {
                        badChildIndex = c1;
                    }
                    else
                    {
                        throw new InvalidProgramException($"Found multiple bad children existing {badChildIndex} new {c1}");
                    }
                }
            }

            var badChildName = sChildren[brokenNode, badChildIndex];
            var badChildNode = FindProgram(badChildName);
            var goodExpectedChildWeight = int.MaxValue;

            for (var c = 0; c < sChildrenCounts[brokenNode]; ++c)
            {
                var childName = sChildren[brokenNode, c];
                var childIndex = FindProgram(childName);
                Console.WriteLine($"Child[{c}] '{childName}' W:{sWeights[childIndex]} TW:{sTotalWeights[childIndex]}");
                if (c != badChildIndex)
                {
                    if (goodExpectedChildWeight == int.MaxValue)
                    {
                        goodExpectedChildWeight = sTotalWeights[childIndex];
                    }
                    if (goodExpectedChildWeight != sTotalWeights[childIndex])
                    {
                        throw new InvalidProgramException($"Good Child nodes are unbalanced Expected:{goodExpectedChildWeight} Got:{sTotalWeights[childIndex]}");
                    }
                }
            }

            var newChildWeight = sWeights[badChildNode] + (goodExpectedChildWeight - sTotalWeights[badChildNode]);
            BalanceTower = newChildWeight;
            Console.WriteLine($"BAD Child[{badChildIndex}] '{badChildName}' NewWeight:{newChildWeight} W:{sWeights[badChildNode]} TW:{sTotalWeights[badChildNode]} Expected:{goodExpectedChildWeight}");
        }

        static int FindProgram(string name)
        {
            for (var i = 0; i < sProgramCount; ++i)
            {
                if (sNames[i] == name)
                {
                    return i;
                }
            }
            throw new InvalidProgramException($"Program '{name}' not found");
        }

        public static string BottomProgram { get; private set; }
        public static int BalanceTower { get; private set; }

        public static void Run()
        {
            Console.WriteLine("Day07 : Start");
            _ = new Program("Day07/input.txt", true);
            _ = new Program("Day07/input.txt", false);
            Console.WriteLine("Day07 : End");
        }
    }
}
