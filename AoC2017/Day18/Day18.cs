using System;

/*
--- Day 18: Duet ---

You discover a tablet containing some strange assembly code labeled simply "Duet".
Rather than bother the sound card with it, you decide to run the code yourself.
Unfortunately, you don't see any documentation, so you're left to figure out what the instructions mean on your own.

It seems like the assembly is meant to operate on a set of registers that are each named with a single letter and that can each hold a single integer.
You suppose each register should start with a value of 0.

There aren't that many instructions, so it shouldn't be hard to figure out what they do.
Here's what you determine:

snd X plays a sound with a frequency equal to the value of X.
set X Y sets register X to the value of Y.
add X Y increases register X by the value of Y.
mul X Y sets register X to the result of multiplying the value contained in register X by the value of Y.
mod X Y sets register X to the remainder of dividing the value contained in register X by the value of Y (that is, it sets X to the result of X modulo Y).
rcv X recovers the frequency of the last sound played, but only when the value of X is not zero.(If it is zero, the command does nothing.)
jgz X Y jumps with an offset of the value of Y, but only if the value of X is greater than zero.(An offset of 2 skips the next instruction, an offset of -1 jumps to the previous instruction, and so on.)
Many of the instructions can take either a register (a single letter) or a number.
The value of a register is the integer it contains; the value of a number is that number.

After each jump instruction, the program continues with the instruction to which the jump jumped.
After any other instruction, the program continues with the next instruction.
Continuing (or jumping) off either end of the program terminates it.

For example:

set a 1
add a 2
mul a a
mod a 5
snd a
set a 0
rcv a
jgz a -1
set a 1
jgz a -2

The first four instructions set a to 1, add 2 to it, square it, and then set it to itself modulo 5, resulting in a value of 4.
Then, a sound with frequency 4 (the value of a) is played.
After that, a is set to 0, causing the subsequent rcv and jgz instructions to both be skipped (rcv because a is 0, and jgz because a is not greater than 0).
Finally, a is set to 1, causing the next jgz instruction to activate, jumping back two instructions to another jump, which jumps again to the rcv, which ultimately triggers the recover operation.
At the time the recover operation is executed, the frequency of the last sound played is 4.

What is the value of the recovered frequency (the value of the most recently played sound) the first time a rcv instruction is executed with a non-zero value?

*/

namespace Day18
{
    class Program
    {
        readonly static int SND_REGISTER = 'S';
        readonly static int RCV_REGISTER = 'R';
        public enum Operation { ILLEGAL = 0, SND, SET, ADD, MUL, MOD, RCV, JGZ };

        readonly static int MAX_NUM_INSTRUCTIONS = 1024;
        readonly static int MAX_NUM_REGISTERS = 256;

        readonly static Operation[] sOperations = new Operation[MAX_NUM_INSTRUCTIONS];
        readonly static int[] sDestinationRegisters = new int[MAX_NUM_INSTRUCTIONS];
        readonly static int[] sSourceRegisters = new int[MAX_NUM_INSTRUCTIONS];
        readonly static int[] sSourceValues = new int[MAX_NUM_INSTRUCTIONS];

        readonly static int[] sRegisterValues = new int[MAX_NUM_REGISTERS];
        static int sInstructionCount;

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);

            if (part1)
            {
                var result1 = FirstValidRcvFrequency();
                Console.WriteLine($"Day18 : Result1 {result1}");
                var expected = 280;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                long result2 = -123;
                Console.WriteLine($"Day18 : Result2 {result2}");
                long expected = 1797;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] lines)
        {
            sInstructionCount = 0;
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
                    "snd" => Operation.SND,
                    "set" => Operation.SET,
                    "add" => Operation.ADD,
                    "mul" => Operation.MUL,
                    "mod" => Operation.MOD,
                    "rcv" => Operation.RCV,
                    "jgz" => Operation.JGZ,
                    _ => Operation.ILLEGAL,
                };
                sOperations[sInstructionCount] = op;

                var destRegToken = tokens[1].Trim();
                if (destRegToken.Length != 1)
                {
                    throw new InvalidProgramException($"Invalid line '{line}' destination register must be a single character '{destRegToken}'");
                }
                if ((destRegToken[0] < 'a') || (destRegToken[0] > 'z'))
                {
                    throw new InvalidProgramException($"Invalid line '{line}' destination register must be a-z '{destRegToken}'");
                }

                var destinationRegister = destRegToken[0];
                if ((op == Operation.SET) || (op == Operation.ADD) || (op == Operation.MUL) || (op == Operation.MOD) || (op == Operation.JGZ))
                {
                    sDestinationRegisters[sInstructionCount] = destinationRegister;

                    var srcValueToken = tokens[2].Trim();
                    int srcValueRegister = 'V';
                    if (srcValueToken.Length == 1)
                    {
                        if ((srcValueToken[0] >= 'a') && (srcValueToken[0] <= 'z'))
                        {
                            sSourceValues[sInstructionCount] = int.MinValue;
                            srcValueRegister = srcValueToken[0];
                        }
                    }
                    if (srcValueRegister == 'V')
                    {
                        sSourceValues[sInstructionCount] = int.Parse(srcValueToken);
                    }
                    sSourceRegisters[sInstructionCount] = srcValueRegister;
                }
                else if (op == Operation.SND)
                {
                    sSourceValues[sInstructionCount] = 0;
                    sSourceRegisters[sInstructionCount] = destinationRegister;
                    sDestinationRegisters[sInstructionCount] = SND_REGISTER;
                }
                else if (op == Operation.RCV)
                {
                    sSourceRegisters[sInstructionCount] = SND_REGISTER;
                    sDestinationRegisters[sInstructionCount] = destinationRegister;
                }
                else
                {
                    throw new InvalidProgramException($"Unknown operation '{op}' Line '{line}'");
                }
                ++sInstructionCount;
            }
        }

        public static int FirstValidRcvFrequency()
        {
            var pc = 0;
            while (pc >= 0)
            {
                pc = ExecuteInstruction(pc);
                if (sRegisterValues[RCV_REGISTER] != 0)
                {
                    return sRegisterValues[RCV_REGISTER];
                }
                if (pc >= sInstructionCount)
                {
                    throw new IndexOutOfRangeException($"Illegal pc {pc} range 0-{sInstructionCount - 1}");
                }
            }
            throw new NotImplementedException();
        }

        static int ExecuteInstruction(int pc)
        {
            //snd X plays a sound with a frequency equal to the value of X.
            //set X Y sets register X to the value of Y.
            //add X Y increases register X by the value of Y.
            //mul X Y sets register X to the result of multiplying the value contained in register X by the value of Y.
            //mod X Y sets register X to the remainder of dividing the value contained in register X by the value of Y (that is, it sets X to the result of X modulo Y).
            //rcv X recovers the frequency of the last sound played, but only when the value of X is not zero.(If it is zero, the command does nothing.)
            //jgz X Y jumps with an offset of the value of Y, but only if the value of X is greater than zero.(An offset of 2 skips the next instruction, an offset of -1 jumps to the previous instruction, and so on.)
            var sourceValue = sSourceValues[pc];
            var sourceRegister = sSourceRegisters[pc];
            if (sourceRegister != 'V')
            {
                sourceValue = sRegisterValues[sourceRegister];
            }
            var destinationRegister = sDestinationRegisters[pc];
            var destinationRegisterValue = sRegisterValues[destinationRegister];
            var resultValue = sOperations[pc] switch
            {
                Operation.SND => sourceValue,
                Operation.SET => sourceValue,
                Operation.ADD => destinationRegisterValue + sourceValue,
                Operation.MUL => destinationRegisterValue * sourceValue,
                Operation.MOD => destinationRegisterValue % sourceValue,
                Operation.RCV => (sourceValue != 0) ? destinationRegisterValue : 0,
                Operation.JGZ => (destinationRegisterValue > 0) ? sourceValue : 1,
                _ => throw new InvalidProgramException($"Invalid opcode code")
            };
            if (sOperations[pc] == Operation.JGZ)
            {
                return pc + resultValue;
            }
            else if (sOperations[pc] == Operation.RCV)
            {
                if (resultValue != 0)
                {
                    sRegisterValues[RCV_REGISTER] = sourceValue;
                }
            }
            else
            {
                sRegisterValues[destinationRegister] = resultValue;
            }
            return pc + 1;
        }

        public static void Run()
        {
            Console.WriteLine("Day18 : Start");
            _ = new Program("Day18/input.txt", true);
            _ = new Program("Day18/input.txt", false);
            Console.WriteLine("Day18 : End");
        }
    }
}
