using System;

/*

--- Day 23: Coprocessor Conflagration ---

You decide to head directly to the CPU and fix the printer from there.
As you get close, you find an experimental coprocessor doing so much work that the local programs are afraid it will halt and catch fire.
This would cause serious issues for the rest of the computer, so you head in and see what you can do.

The code it's running seems to be a variant of the kind you saw recently on that tablet.
The general functionality seems very similar, but some of the instructions are different:

set X Y sets register X to the value of Y.
sub X Y decreases register X by the value of Y.
mul X Y sets register X to the result of multiplying the value contained in register X by the value of Y.
jnz X Y jumps with an offset of the value of Y, but only if the value of X is not zero.
(An offset of 2 skips the next instruction, an offset of -1 jumps to the previous instruction, and so on.)
Only the instructions listed above are used.
The eight registers here, named a through h, all start at 0.

The coprocessor is currently set to some kind of debug mode, which allows for testing, but prevents it from doing any meaningful work.

If you run the program (your puzzle input), how many times is the mul instruction invoked?

Your puzzle answer was 4225.

--- Part Two ---

Now, it's time to fix the problem.

The debug mode switch is wired directly to register a.
You flip the switch, which makes register a now start at 1 when the program is executed.

Immediately, the coprocessor begins to overheat.
Whoever wrote this program obviously didn't choose a very efficient implementation.
You'll need to optimize the program if it has any hope of completing before Santa needs that printer working.

The coprocessor's ultimate goal is to determine the final value left in register h once the program completes.
Technically, if it had that... it wouldn't even need to run the program.

After setting register a to 1, if the program were to run to completion, what value would be left in register h?

*/

namespace Day23
{
    class Program
    {
        public enum Operation { ILLEGAL = 0, SET, SUB, MUL, JNZ };

        readonly static int MAX_NUM_INSTRUCTIONS = 1024;
        readonly static int MAX_NUM_REGISTERS = 256;
        readonly static int MAX_NUM_PROGRAMS = 2;
        readonly static int MAX_NUM_INPUTS = 1024;

        public struct Prog
        {
            public Operation[] Operations;
            public int[] DestinationRegisters;
            public long[] DestinationValues;

            public int[] SourceRegisters;
            public long[] SourceValues;

            public long[] Inputs;
            public int InstructionCount;

            public long[] RegisterValues;
            public long InputsCount;

            public int Id;
            public long Pc;

            public Prog(int inId, string[] lines)
            {
                Operations = new Operation[MAX_NUM_INSTRUCTIONS];
                DestinationRegisters = new int[MAX_NUM_INSTRUCTIONS];
                DestinationValues = new long[MAX_NUM_INSTRUCTIONS];

                SourceRegisters = new int[MAX_NUM_INSTRUCTIONS];
                SourceValues = new long[MAX_NUM_INSTRUCTIONS];

                RegisterValues = new long[MAX_NUM_REGISTERS];
                InstructionCount = 0;

                Inputs = new long[MAX_NUM_INPUTS];
                InputsCount = 0;
                Id = inId;
                Pc = 0;

                Parse(lines);
            }

            public Operation ExecuteInstruction()
            {
                //set X Y sets register X to the value of Y.
                //sub X Y decreases register X by the value of Y.
                //mul X Y sets register X to the result of multiplying the value contained in register X by the value of Y.
                //jnz X Y jumps with an offset of the value of Y, but only if the value of X is not zero.
                var operation = Operations[Pc];
                var sourceValue = SourceValues[Pc];
                var sourceRegister = SourceRegisters[Pc];
                if (sourceRegister != 'V')
                {
                    sourceValue = RegisterValues[sourceRegister];
                }
                var destinationValue = DestinationValues[Pc];
                var destinationRegister = DestinationRegisters[Pc];
                if (destinationRegister != 'V')
                {
                    destinationValue = RegisterValues[destinationRegister];
                }
                var resultValue = operation switch
                {
                    Operation.SET => sourceValue,
                    Operation.SUB => destinationValue - sourceValue,
                    Operation.MUL => destinationValue * sourceValue,
                    Operation.JNZ => (destinationValue != 0) ? sourceValue : 1,
                    _ => throw new InvalidProgramException($"Invalid opcode code")
                };
                if (operation == Operation.JNZ)
                {
                    Pc += resultValue;
                    return operation;
                }
                else
                {
                    RegisterValues[destinationRegister] = resultValue;
                }
                ++Pc;
                return operation;
            }

            public long RunProgram()
            {
                long mulCount = 0L;
                while ((Pc >= 0) && (Pc < InstructionCount))
                {
                    var operation = ExecuteInstruction();
                    if (operation == Operation.MUL)
                    {
                        ++mulCount;
                    }
                }
                return mulCount;
            }

            void Parse(string[] lines)
            {
                InstructionCount = 0;
                foreach (var line in lines)
                {
                    var tokens = line.Trim().Split();
                    if (tokens.Length < 2)
                    {
                        throw new InvalidProgramException($"Invalid line '{line}' need at least 2 tokens got {tokens.Length}");
                    }
                    var opToken = tokens[0].Trim();
                    var op = opToken switch
                    {
                        "set" => Operation.SET,
                        "sub" => Operation.SUB,
                        "mul" => Operation.MUL,
                        "jnz" => Operation.JNZ,
                        _ => Operation.ILLEGAL,
                    };
                    Operations[InstructionCount] = op;

                    var destRegToken = tokens[1].Trim();
                    if ((op == Operation.SET) || (op == Operation.SUB) || (op == Operation.MUL) || (op == Operation.JNZ))
                    {
                        var dstValueToken = destRegToken;
                        int dstValueRegister = 'V';
                        if (dstValueToken.Length == 1)
                        {
                            if ((dstValueToken[0] >= 'a') && (dstValueToken[0] <= 'z'))
                            {
                                DestinationValues[InstructionCount] = int.MinValue;
                                dstValueRegister = dstValueToken[0];
                            }
                        }
                        if (dstValueRegister == 'V')
                        {
                            DestinationValues[InstructionCount] = int.Parse(dstValueToken);
                        }
                        DestinationRegisters[InstructionCount] = dstValueRegister;

                        var srcValueToken = tokens[2].Trim();
                        int srcValueRegister = 'V';
                        if (srcValueToken.Length == 1)
                        {
                            if ((srcValueToken[0] >= 'a') && (srcValueToken[0] <= 'z'))
                            {
                                SourceValues[InstructionCount] = int.MinValue;
                                srcValueRegister = srcValueToken[0];
                            }
                        }
                        if (srcValueRegister == 'V')
                        {
                            SourceValues[InstructionCount] = int.Parse(srcValueToken);
                        }
                        SourceRegisters[InstructionCount] = srcValueRegister;
                    }
                    else
                    {
                        throw new InvalidProgramException($"Unknown operation '{op}' Line '{line}'");
                    }
                    ++InstructionCount;
                }
            }

            public void AddInput(long input)
            {
                Inputs[InputsCount] = input;
                ++InputsCount;
            }
        };

        static readonly Prog[] sPrograms = new Prog[MAX_NUM_PROGRAMS];

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);

            if (part1)
            {
                var result1 = MulCount();
                Console.WriteLine($"Day23 : Result1 {result1}");
                var expected = 4225;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = -123;
                Console.WriteLine($"Day23 : Result2 {result2}");
                var expected = 1797;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] lines)
        {
            sPrograms[0] = new Prog(0, lines);
        }

        public static long MulCount()
        {
            return sPrograms[0].RunProgram();
        }

        public static void Run()
        {
            Console.WriteLine("Day23 : Start");
            _ = new Program("Day23/input.txt", true);
            _ = new Program("Day23/input.txt", false);
            Console.WriteLine("Day23 : End");
        }
    }
}
