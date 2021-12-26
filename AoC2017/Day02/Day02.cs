using System;

/*

--- Day 2: Corruption Checksum ---

As you walk through the door, a glowing humanoid shape yells in your direction.
"You there! Your state appears to be idle.
Come help us repair the corruption in this spreadsheet - if we take another millisecond, we'll have to display an hourglass cursor!"

The spreadsheet consists of rows of apparently-random numbers.
To make sure the recovery process is on the right track, they need you to calculate the spreadsheet's checksum.
For each row, determine the difference between the largest value and the smallest value; the checksum is the sum of all of these differences.

For example, given the following spreadsheet:

5 1 9 5
7 5 3
2 4 6 8
The first row's largest and smallest values are 9 and 1, and their difference is 8.
The second row's largest and smallest values are 7 and 3, and their difference is 4.
The third row's difference is 6.
In this example, the spreadsheet's checksum would be 8 + 4 + 6 = 18.

What is the checksum for the spreadsheet in your puzzle input?

Your puzzle answer was 32121.

--- Part Two ---

"Great work; looks like we're on the right track after all.
Here's a star for your effort."
However, the program seems a little worried.
Can programs be worried?

"Based on what we're seeing, it looks like all the User wanted is some information about the evenly divisible values in the spreadsheet.
Unfortunately, none of us are equipped for that kind of calculation - most of us specialize in bitwise operations."

It sounds like the goal is to find the only two numbers in each row where one evenly divides the other - that is, where the result of the division operation is a whole number.
They would like you to find those numbers on each line, divide them, and add up each line's result.

For example, given the following spreadsheet:

5 9 2 8
9 4 7 3
3 8 6 5
In the first row, the only two numbers that evenly divide are 8 and 2; the result of this division is 4.
In the second row, the two numbers are 9 and 3; the result is 3.
In the third row, the result is 2.
In this example, the sum of the results would be 4 + 3 + 2 = 9.

What is the sum of each row's result in your puzzle input?

*/

namespace Day02
{
    class Program
    {
        readonly static long MAX_NUM_ITEMS = 128;
        readonly static long[,] sCells = new long[MAX_NUM_ITEMS, MAX_NUM_ITEMS];
        readonly static long[] sColumnsCounts = new long[MAX_NUM_ITEMS];
        static long sRowsCount;

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);

            if (part1)
            {
                var result1 = Checksum;
                Console.WriteLine($"Day02 : Result1 {result1}");
                var expected = 37923;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = Checksum2;
                Console.WriteLine($"Day02 : Result2 {result2}");
                var expected = 263;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] lines)
        {
            sRowsCount = 0;

            if (lines.Length > MAX_NUM_ITEMS)
            {
                throw new InvalidProgramException($"Too many rows {lines.Length} Max:{MAX_NUM_ITEMS}");
            }
            var row = 0;
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                var tokens = trimmed.Split(" \t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length > MAX_NUM_ITEMS)
                {
                    throw new InvalidProgramException($"Too many columns {tokens.Length} Max:{MAX_NUM_ITEMS}");
                }

                sColumnsCounts[row] = tokens.Length;
                for (var col = 0; col < tokens.Length; ++col)
                {
                    sCells[row, col] = long.Parse(tokens[col]);
                }
                ++row;
            }
            sRowsCount = row;
        }

        public static long Checksum
        {
            get
            {
                var sum = 0L;
                for (var row = 0; row < sRowsCount; ++row)
                {
                    var max = long.MinValue;
                    var min = long.MaxValue;
                    for (var col = 0; col < sColumnsCounts[row]; ++col)
                    {
                        max = Math.Max(max, sCells[row, col]);
                        min = Math.Min(min, sCells[row, col]);
                    }
                    sum += (max - min);
                }
                return sum;
            }
        }

        public static long Checksum2
        {
            get
            {
                var sum = 0L;
                for (var row = 0; row < sRowsCount; ++row)
                {
                    var result = 0L;
                    for (var col = 0; col < sColumnsCounts[row] - 1; ++col)
                    {
                        var val1 = sCells[row, col];
                        for (var col2 = col + 1; col2 < sColumnsCounts[row]; ++col2)
                        {
                            var val2 = sCells[row, col2];
                            if (val1 > val2)
                            {
                                if ((val1 % val2) == 0)
                                {
                                    if (result != 0)
                                    {
                                        throw new InvalidProgramException($"Error multiple divisors found for row {row}");
                                    }
                                    result = val1 / val2;
                                }
                            }
                            else if (val2 > val1)
                            {
                                if ((val2 % val1) == 0)
                                {
                                    if (result != 0)
                                    {
                                        throw new InvalidProgramException($"Error multiple divisors found for row {row}");
                                    }
                                    result = val2 / val1;
                                }
                            }
                        }
                    }
                    if (result == 0)
                    {
                        throw new InvalidProgramException($"Error no divisor found for row {row}");
                    }
                    sum += result;
                }
                return sum;
            }
        }

        public static void Run()
        {
            Console.WriteLine("Day02 : Start");
            _ = new Program("Day02/input.txt", true);
            _ = new Program("Day02/input.txt", false);
            Console.WriteLine("Day02 : End");
        }
    }
}
