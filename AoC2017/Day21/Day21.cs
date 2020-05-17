using System;

/*

--- Day 21: Fractal Art ---

You find a program trying to generate some art.
It uses a strange process that involves repeatedly enhancing the detail of an image through a set of rules.

The image consists of a two-dimensional square grid of pixels that are either on (#) or off (.).
The program always begins with this pattern:

.#.
..#
###
Because the pattern is both 3 pixels wide and 3 pixels tall, it is said to have a size of 3.

Then, the program repeats the following process:

If the size is evenly divisible by 2, break the pixels up into 2x2 squares, and convert each 2x2 square into a 3x3 square by following the corresponding enhancement rule.
Otherwise, the size is evenly divisible by 3; break the pixels up into 3x3 squares, and convert each 3x3 square into a 4x4 square by following the corresponding enhancement rule.
Because each square of pixels is replaced by a larger one, the image gains pixels and so its size increases.

The artist's book of enhancement rules is nearby (your puzzle input); however, it seems to be missing rules.
The artist explains that sometimes, one must rotate or flip the input pattern to find a match.
(Never rotate or flip the output pattern, though.) Each pattern is written concisely: rows are listed as single units, ordered top-down, and separated by slashes.
For example, the following rules correspond to the adjacent patterns:

../.#  =  ..
          .#

                .#.
.#./..#/###  =  ..#
                ###

                        #..#
#..#/..../#..#/.##.  =  ....
                        #..#
                        .##.
When searching for a rule to use, rotate and flip the pattern as necessary.
For example, all of the following patterns match the same rule:

.#.   .#.   #..   ###
..#   #..   #.#   ..#
###   ###   ##.   .#.
Suppose the book contained the following two rules:

../.# => ##./#../...
.#./..#/### => #..#/..../..../#..#
As before, the program begins with this pattern:

.#.
..#
###
The size of the grid (3) is not divisible by 2, but it is divisible by 3.
It divides evenly into a single square; the square matches the second rule, which produces:

#..#
....
....
#..#
The size of this enhanced grid (4) is evenly divisible by 2, so that rule is used.
It divides evenly into four squares:

#.|.#
..|..
--+--
..|..
#.|.#
Each of these squares matches the same rule (../.# => ##./#../...), three of which require some flipping and rotation to line up with the rule.
The output for the rule is the same in all four cases:

##.|##.
#..|#..
...|...
---+---
##.|##.
#..|#..
...|...
Finally, the squares are joined into a new grid:

##.##.
#..#..
......
##.##.
#..#..
......
Thus, after 2 iterations, the grid contains 12 pixels that are on.

How many pixels stay on after 5 iterations?

Your puzzle answer was 190.

--- Part Two ---

How many pixels stay on after 18 iterations?

*/

namespace Day21
{
    class Program
    {
        readonly static int MAX_NUM_RULES = 1024;
        readonly static int MAX_BOARD_SIZE = 1024 * 4;
        readonly static string[] sRulesLHS = new string[MAX_NUM_RULES];
        readonly static string[] sRulesRHS = new string[MAX_NUM_RULES];
        static int sRuleCount;
        readonly static char[,] sBoard = new char[MAX_BOARD_SIZE, MAX_BOARD_SIZE];
        static int sBoardSize;

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);

            if (part1)
            {
                var result1 = PixelsOn(5);
                Console.WriteLine($"Day21 : Result1 {result1}");
                var expected = 190;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = PixelsOn(18);
                Console.WriteLine($"Day21 : Result2 {result2}");
                var expected = 2335049;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] rules)
        {
            sRuleCount = 0;
            foreach (var rule in rules)
            {
                var ruleTokens = rule.Trim().Split(" => ");
                if (ruleTokens.Length != 2)
                {
                    throw new InvalidProgramException($"Invalid rule '{rule}' must be two tokens separated by ' => '");
                }

                var lhsTokens = ruleTokens[0].Trim().Split('/');
                var lhsHeight = lhsTokens.Length;
                if ((lhsHeight != 2) && (lhsHeight != 3))
                {
                    throw new InvalidProgramException($"Invalid LHS rule '{rule}' height {lhsHeight} must 2 or 3");
                }
                var ruleLHS = new char[lhsHeight * lhsHeight];
                var i = 0;
                for (var y = 0; y < lhsHeight; ++y)
                {
                    var buffer = lhsTokens[y];
                    var lhsWidth = buffer.Length;
                    if (lhsWidth != lhsHeight)
                    {
                        throw new InvalidProgramException($"Invalid LHS rule '{rule}' width and height must be the same {lhsWidth} != {lhsHeight}");
                    }
                    for (var x = 0; x < lhsWidth; ++x)
                    {
                        ruleLHS[i] = buffer[x];
                        ++i;
                    }
                }

                var rhsTokens = ruleTokens[1].Trim().Split('/');
                var outputHeight = rhsTokens.Length;
                var rhsHeight = rhsTokens.Length;
                if (rhsHeight != lhsHeight + 1)
                {
                    throw new InvalidProgramException($"Invalid RHS rule '{rule}' height {rhsHeight} must be 1 more than LHS height {lhsHeight}");
                }
                var ruleRHS = new char[rhsHeight * rhsHeight];
                i = 0;
                for (var y = 0; y < rhsHeight; ++y)
                {
                    var buffer = rhsTokens[y];
                    var rhsWwidth = buffer.Length;
                    if (rhsWwidth != rhsHeight)
                    {
                        throw new InvalidProgramException($"Invalid rule '{rule}' width and height must be the same {rhsWwidth} != {rhsHeight}");
                    }
                    for (var x = 0; x < rhsWwidth; ++x)
                    {
                        ruleRHS[i] = buffer[x];
                        ++i;
                    }
                }
                AddRuleAndVariations(ruleLHS, ruleRHS);
            }
            //Console.WriteLine($"{sRuleCount} rules");
            //.#.
            //..#
            //###
            string startingState = ".#...####";
            if (startingState.Length != 9)
            {
                throw new InvalidProgramException($"Invalid startingState '{startingState}' Length must be 9 {startingState.Length}");
            }
            for (var y = 0; y < 3; ++y)
            {
                for (var x = 0; x < 3; ++x)
                {
                    var i = x + y * 3;
                    sBoard[x, y] = startingState[i];
                }
            }
            sBoardSize = 3;
        }

        static void AddRuleAndVariations(char[] ruleLHS, char[] ruleRHS)
        {
            AddRule(ruleLHS, ruleRHS);

            if (ruleLHS.Length == 4)
            {
                var variation = new char[4];
                // 2x2:   0123

                // R90:   2031
                variation[0] = ruleLHS[2];
                variation[1] = ruleLHS[0];
                variation[2] = ruleLHS[3];
                variation[3] = ruleLHS[1];
                AddRule(variation, ruleRHS);

                // R180:  3210
                variation[0] = ruleLHS[3];
                variation[1] = ruleLHS[2];
                variation[2] = ruleLHS[1];
                variation[3] = ruleLHS[0];
                AddRule(variation, ruleRHS);

                // R270:  1302
                variation[0] = ruleLHS[1];
                variation[1] = ruleLHS[3];
                variation[2] = ruleLHS[0];
                variation[3] = ruleLHS[2];
                AddRule(variation, ruleRHS);

                // R90 FlipX:   0213
                variation[0] = ruleLHS[0];
                variation[1] = ruleLHS[2];
                variation[2] = ruleLHS[1];
                variation[3] = ruleLHS[3];
                AddRule(variation, ruleRHS);

                // R180 FlipX:  2301 == FlipY

                // R270 FlipX:  3120
                variation[0] = ruleLHS[3];
                variation[1] = ruleLHS[1];
                variation[2] = ruleLHS[2];
                variation[3] = ruleLHS[0];
                AddRule(variation, ruleRHS);
            }
            else if (ruleLHS.Length == 9)
            {
                var variation = new char[9];
                //3x3 :   012345678

                //R90 :   630741852
                variation[0] = ruleLHS[6];
                variation[1] = ruleLHS[3];
                variation[2] = ruleLHS[0];
                variation[3] = ruleLHS[7];
                variation[4] = ruleLHS[4];
                variation[5] = ruleLHS[1];
                variation[6] = ruleLHS[8];
                variation[7] = ruleLHS[5];
                variation[8] = ruleLHS[2];
                AddRule(variation, ruleRHS);

                //R180 :  876543210
                variation[0] = ruleLHS[8];
                variation[1] = ruleLHS[7];
                variation[2] = ruleLHS[6];
                variation[3] = ruleLHS[5];
                variation[4] = ruleLHS[4];
                variation[5] = ruleLHS[3];
                variation[6] = ruleLHS[2];
                variation[7] = ruleLHS[1];
                variation[8] = ruleLHS[0];
                AddRule(variation, ruleRHS);

                //R270 :  258147036
                variation[0] = ruleLHS[2];
                variation[1] = ruleLHS[5];
                variation[2] = ruleLHS[8];
                variation[3] = ruleLHS[1];
                variation[4] = ruleLHS[4];
                variation[5] = ruleLHS[7];
                variation[6] = ruleLHS[0];
                variation[7] = ruleLHS[3];
                variation[8] = ruleLHS[6];
                AddRule(variation, ruleRHS);
            }
            else
            {
                throw new InvalidProgramException($"Invalid rule '{ruleLHS}' length must be 4 or 9 {ruleLHS.Length}");
            }
        }

        static void AddRule(char[] ruleLHS, char[] ruleRHS)
        {
            AddRuleImpl(ruleLHS, ruleRHS);
            if (ruleLHS.Length == 4)
            {
                var variation = new char[4];
                // FlipX: 1032
                variation[0] = ruleLHS[1];
                variation[1] = ruleLHS[0];
                variation[2] = ruleLHS[3];
                variation[3] = ruleLHS[2];
                AddRuleImpl(variation, ruleRHS);

                // FlipY: 2301
                variation[0] = ruleLHS[2];
                variation[1] = ruleLHS[3];
                variation[2] = ruleLHS[0];
                variation[3] = ruleLHS[1];
                AddRuleImpl(variation, ruleRHS);
            }
            else if (ruleLHS.Length == 9)
            {
                var variation = new char[9];

                //FlipX : 210543876
                variation[0] = ruleLHS[2];
                variation[1] = ruleLHS[1];
                variation[2] = ruleLHS[0];
                variation[3] = ruleLHS[5];
                variation[4] = ruleLHS[4];
                variation[5] = ruleLHS[3];
                variation[6] = ruleLHS[8];
                variation[7] = ruleLHS[7];
                variation[8] = ruleLHS[6];
                AddRuleImpl(variation, ruleRHS);

                //FlipY : 678345012
                variation[0] = ruleLHS[6];
                variation[1] = ruleLHS[7];
                variation[2] = ruleLHS[8];
                variation[3] = ruleLHS[3];
                variation[4] = ruleLHS[4];
                variation[5] = ruleLHS[5];
                variation[6] = ruleLHS[0];
                variation[7] = ruleLHS[1];
                variation[8] = ruleLHS[2];
                AddRuleImpl(variation, ruleRHS);
            }
            else
            {
                throw new InvalidProgramException($"Invalid rule '{ruleLHS}' length must be 4 or 9 {ruleLHS.Length}");
            }
        }

        static void AddRuleImpl(char[] ruleLHS, char[] ruleRHS)
        {
            if ((ruleLHS.Length != 4) && (ruleLHS.Length != 9))
            {
                throw new InvalidProgramException($"Invalid rule ruleLHS length must be 4 or 9 {ruleLHS.Length}");
            }
            if ((ruleRHS.Length != 9) && (ruleRHS.Length != 16))
            {
                throw new InvalidProgramException($"Invalid rule ruleRHS length must be 9 or 16 {ruleRHS.Length}");
            }
            if (sRuleCount >= MAX_NUM_RULES)
            {
                throw new IndexOutOfRangeException($"Too many rules MAX:{MAX_NUM_RULES}");
            }

            var newRuleLHS = new string(ruleLHS);
            var newRuleRHS = new string(ruleRHS);
            for (var r = 0; r < sRuleCount; ++r)
            {
                if (sRulesLHS[r] == newRuleLHS)
                {
                    if (sRulesRHS[r] == newRuleRHS)
                    {
                        return;
                    }
                    throw new InvalidProgramException($"Rule LHS {newRuleLHS} matched existing rule but the RHS did not match expected {sRulesRHS[r]} got {newRuleRHS}");
                }
            }
            sRulesLHS[sRuleCount] = newRuleLHS;
            sRulesRHS[sRuleCount] = newRuleRHS;
            ++sRuleCount;
        }

        static int FindMatchingRule(char[] board, int boardSize)
        {
            var totalLen = boardSize * boardSize;
            for (var r = 0; r < sRuleCount; ++r)
            {
                var rule = sRulesLHS[r];
                if (rule.Length != totalLen)
                {
                    continue;
                }
                bool match = true;
                for (var i = 0; i < totalLen; ++i)
                {
                    if (board[i] != rule[i])
                    {
                        match = false;
                        continue;
                    }
                }
                if (match)
                {
                    return r;
                }
            }
            throw new InvalidProgramException($"No matching rule found");
        }

        static char[] ExtractBoard(int subBoardX, int subBoardY, int subBoardSize)
        {
            var x0 = subBoardX * subBoardSize;
            var y0 = subBoardY * subBoardSize;

            var subBoard = new char[subBoardSize * subBoardSize];
            for (var y = 0; y < subBoardSize; ++y)
            {
                for (var x = 0; x < subBoardSize; ++x)
                {
                    var i = x + y * subBoardSize;
                    subBoard[i] = sBoard[x0 + x, y0 + y];
                }
            }
            return subBoard;
        }

        public static long PixelsOn(int iterationCount)
        {
            for (var i = 0; i < iterationCount; ++i)
            {
                //Console.WriteLine($"Start[{i}] {sBoardSize} x {sBoardSize}");
                //OutputBaord();
                SingleIteration();
                //Console.WriteLine($"End[{i}] {sBoardSize} x {sBoardSize}");
                //OutputBaord();
            }
            var pixelsOnCount = 0L;
            Console.WriteLine($"End {sBoardSize} x {sBoardSize}");
            for (var y = 0; y < sBoardSize; ++y)
            {
                for (var x = 0; x < sBoardSize; ++x)
                {
                    if (sBoard[x, y] == '#')
                    {
                        ++pixelsOnCount;
                    }
                }
            }
            return pixelsOnCount;
        }

        static void SingleIteration()
        {
            int subBoardSize;
            if ((sBoardSize % 2) == 0)
            {
                subBoardSize = 2;
            }
            else if ((sBoardSize % 3) == 0)
            {
                subBoardSize = 3;
            }
            else
            {
                throw new IndexOutOfRangeException($"Invalid boardsize {sBoardSize} not a multiple of 2 or 3");
            }

            var subBoardCount = sBoardSize / subBoardSize;
            //Console.WriteLine($"BoardSize {sBoardSize} processing {subBoardCount} x {subBoardCount} of size {subBoardSize}");

            var newBoard = new char[MAX_BOARD_SIZE, MAX_BOARD_SIZE];
            var outputBoardSize = int.MinValue;
            for (var y = 0; y < subBoardCount; ++y)
            {
                for (var x = 0; x < subBoardCount; ++x)
                {
                    char[] board = ExtractBoard(x, y, subBoardSize);
                    var rule = FindMatchingRule(board, subBoardSize);
                    int thisOutputBoardSize = sRulesRHS[rule].Length == 16 ? 4 : 3;
                    if (outputBoardSize == int.MinValue)
                    {
                        outputBoardSize = thisOutputBoardSize;
                    }
                    if (outputBoardSize != thisOutputBoardSize)
                    {
                        throw new IndexOutOfRangeException($"Inconsistent matched output boardsize was {outputBoardSize} got {thisOutputBoardSize}");
                    }
                    var ruleRHS = sRulesRHS[rule];
                    for (var newY = 0; newY < outputBoardSize; ++newY)
                    {
                        for (var newX = 0; newX < outputBoardSize; ++newX)
                        {
                            var i = newX + newY * outputBoardSize;
                            var c = ruleRHS[i];
                            var x0 = x * outputBoardSize;
                            var y0 = y * outputBoardSize;
                            var outputX = x0 + newX;
                            var outputY = y0 + newY;
                            newBoard[outputX, outputY] = c;
                        }
                    }
                    //Console.WriteLine($"SubBoard {x},{y} Rule[{rule}] '{sRulesLHS[rule]}' matched => {outputBoardSize} x {outputBoardSize}");
                }
            }
            sBoardSize = subBoardCount * outputBoardSize;
            for (var y = 0; y < sBoardSize; ++y)
            {
                for (var x = 0; x < sBoardSize; ++x)
                {
                    sBoard[x, y] = newBoard[x, y];
                }
            }
        }

        static void OutputBaord()
        {
            for (var y = 0; y < sBoardSize; ++y)
            {
                for (var x = 0; x < sBoardSize; ++x)
                {
                    Console.Write(sBoard[x, y]);
                }
                Console.WriteLine();
            }
        }

        public static void Run()
        {
            Console.WriteLine("Day21 : Start");
            _ = new Program("Day21/input.txt", true);
            _ = new Program("Day21/input.txt", false);
            Console.WriteLine("Day21 : End");
        }
    }
}

/*

3x3 = 1x1 * 4 = 4x4
4x4 = 2x2 * 3 = 6x6
6x6 = 3x3 * 3 = 9x9
9x9 = 3x3 * 4 = 12x12
12x12 = 6x6 * 3 = 18x18
*/
