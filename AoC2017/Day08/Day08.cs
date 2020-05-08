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

Your puzzle answer was 6012.

--- Part Two ---

To be safe, the CPU also needs to know the highest value held in any register during this process so that it can decide how much memory to allocate to these operations.
For example, in the above instructions, the highest value ever held was 10 (in register c after the third instruction was evaluated).

*/

namespace Day08
{
    class Program
    {
        public struct Command
        {
            public enum Test { UNKNOWN, LESS, LESSEQUAL, GREATER, GREATEREQUAL, EQUAL, NOTEQAUL };

            public int register;
            public long value;

            public int testRegister;
            public Test test;
            public long testValue;
        }

        readonly static int MAX_NUM_COMMANDS = 1024;
        readonly static int MAX_NUM_REGISTERS = 1024;
        readonly static Command[] sCommands = new Command[MAX_NUM_COMMANDS];
        readonly static string[] sRegisterNames = new string[MAX_NUM_REGISTERS];
        readonly static long[] sRegisterValues = new long[MAX_NUM_REGISTERS];
        static int sCommandCount = 0;
        static int sRegisterCount = 0;

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);
            Execute();

            if (part1)
            {
                var result1 = LargestValue;
                Console.WriteLine($"Day08 : Result1 {result1}");
                var expected = 6012;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = LargestEverValue;
                Console.WriteLine($"Day08 : Result2 {result2}");
                var expected = 6369;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] program)
        {
            LargestValue = int.MinValue;
            sCommandCount = 0;
            sRegisterCount = 0;
            foreach (var line in program)
            {
                var tokens = line.Trim().Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);
                // c inc -20 if c == 10
                if (tokens.Length != 7)
                {
                    throw new InvalidProgramException($"Invalid line '{line}' expected 7 tokens got {tokens.Length}");
                }
                long multiplier = tokens[1] switch
                {
                    "inc" => 1,
                    "dec" => -1,
                    _ => throw new InvalidProgramException($"Invalid line '{line}' unknown operation '{tokens[1]}' expected 'inc' or 'dec'")
                };
                if (tokens[3] != "if")
                {
                    throw new InvalidProgramException($"Invalid line '{line}' expected 'if' got {tokens[3]}");
                }
                Command.Test test = tokens[5] switch
                {
                    ">" => Command.Test.GREATER,
                    ">=" => Command.Test.GREATEREQUAL,
                    "<" => Command.Test.LESS,
                    "<=" => Command.Test.LESSEQUAL,
                    "==" => Command.Test.EQUAL,
                    "!=" => Command.Test.NOTEQAUL,
                    _ => throw new InvalidProgramException($"Invalid line '{line}' unknown test operater {tokens[5]}")
                };

                ref var command = ref sCommands[sCommandCount];
                command.register = FindRegisterIndex(tokens[0]);
                command.value = long.Parse(tokens[2]) * multiplier;
                command.testRegister = FindRegisterIndex(tokens[4]);
                command.test = test;
                command.testValue = long.Parse(tokens[6]);
                ++sCommandCount;
            }
        }

        static int FindRegisterIndex(string name)
        {
            for (var r = 0; r < sRegisterCount; ++r)
            {
                if (sRegisterNames[r] == name)
                {
                    return r;
                }
            }
            var reg = sRegisterCount;
            sRegisterNames[reg] = name;
            sRegisterValues[reg] = 0;
            ++sRegisterCount;
            return reg;
        }

        public static void Execute()
        {
            var largestValue = long.MinValue;
            for (var pc = 0; pc < sCommandCount; ++pc)
            {
                ref var command = ref sCommands[pc];
                var testRegisterValue = sRegisterValues[command.testRegister];
                var testValue = command.testValue;
                long delta = command.test switch
                {
                    Command.Test.GREATER => (testRegisterValue > testValue) ? 1 : 0,
                    Command.Test.GREATEREQUAL => (testRegisterValue >= testValue) ? 1 : 0,
                    Command.Test.LESS => (testRegisterValue < testValue) ? 1 : 0,
                    Command.Test.LESSEQUAL => (testRegisterValue <= testValue) ? 1 : 0,
                    Command.Test.EQUAL => (testRegisterValue == testValue) ? 1 : 0,
                    Command.Test.NOTEQAUL => (testRegisterValue != testValue) ? 1 : 0,
                    _ => throw new InvalidProgramException($"Unknown test operation {command.test}")
                };
                delta *= command.value;
                sRegisterValues[command.register] += delta;
                largestValue = Math.Max(sRegisterValues[command.register], largestValue);
            }
            LargestEverValue = largestValue;

            largestValue = long.MinValue;
            for (var r = 0; r < sRegisterCount; ++r)
            {
                largestValue = Math.Max(sRegisterValues[r], largestValue);
            }
            LargestValue = largestValue;
        }

        public static long LargestValue { get; private set; }
        public static long LargestEverValue { get; private set; }

        public static void Run()
        {
            Console.WriteLine("Day08 : Start");
            _ = new Program("Day08/input.txt", true);
            _ = new Program("Day08/input.txt", false);
            Console.WriteLine("Day08 : End");
        }
    }
}
