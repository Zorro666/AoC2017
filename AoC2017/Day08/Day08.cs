using System;

/*

--- Day 8: I Heard You Like Registers ---

You receive a signal directly from the CPU. 
Because of your recent assistance with jump instructions, it would like you to compute the result of a series of unusual register instructions.

Each instruction consists of several parts: the register to modify, whether to increase or decrease that register's value, the amount by which to increase or decrease it, and a condition. 
If the condition fails, skip the instruction without modifying the register. 
The registers all start at 0. The instructions look like this:

b inc 5 if a > 1
a inc 1 if b < 5
c dec -10 if a >= 1
c inc -20 if c == 10
These instructions would be processed as follows:

Because a starts at 0, it is not greater than 1, and so b is not modified.
a is increased by 1 (to 1) because b is less than 5 (it is 0).
c is decreased by -10 (to 10) because a is now greater than or equal to 1 (it is 1).
c is increased by -20 (to -10) because c is equal to 10.
After this process, the largest value in any register is 1.

You might also encounter <= (less than or equal to) or != (not equal to). 
However, the CPU doesn't have the bandwidth to tell you what all the registers are named, and leaves that to you to determine.

What is the largest value in any register after completing the instructions in your puzzle input?

*/

namespace Day08
{
    class Program
    {
        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);

            if (part1)
            {
                var result1 = LargestValue;
                Console.WriteLine($"Day08 : Result1 {result1}");
                var expected = 280;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = -123;
                Console.WriteLine($"Day08 : Result2 {result2}");
                var expected = 1797;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] program)
        {
            LargestValue = int.MinValue;
        }

        public static long LargestValue { get; private set; }

        public static void Run()
        {
            Console.WriteLine("Day08 : Start");
            _ = new Program("Day08/input.txt", true);
            _ = new Program("Day08/input.txt", false);
            Console.WriteLine("Day08 : End");
        }
    }
}
