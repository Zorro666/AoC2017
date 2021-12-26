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

Your puzzle answer was 8600.

--- Part Two ---

As you congratulate yourself for a job well done, you notice that the documentation has been on the back of the tablet this entire time.
While you actually got most of the instructions correct, there are a few key differences.
This assembly code isn't about sound at all - it's meant to be run twice at the same time.

Each running copy of the program has its own set of registers and follows the code independently - in fact, the programs don't even necessarily run at the same speed.
To coordinate, they use the send (snd) and receive (rcv) instructions:

snd X sends the value of X to the other program.
These values wait in a queue until that program is ready to receive them.
Each program has its own message queue, so a program can never receive a message it sent.
rcv X receives the next value and stores it in register X.
If no values are in the queue, the program waits for a value to be sent to it.
Programs do not continue to the next instruction until they have received a value.
Values are received in the order they are sent.
Each program also has its own program ID (one 0 and the other 1); the register p should begin with this value.

For example:

snd 1
snd 2
snd p
rcv a
rcv b
rcv c
rcv d

Both programs begin by sending three values to the other.
Program 0 sends 1, 2, 0; program 1 sends 1, 2, 1.
Then, each program receives a value (both 1) and stores it in a, receives another value (both 2) and stores it in b, and then each receives the program ID of the other program (program 0 receives 1; program 1 receives 0) and stores it in c.
Each program now sees a different value in its own copy of register c.

Finally, both programs try to rcv a fourth time, but no data is waiting for either of them, and they reach a deadlock.
When this happens, both programs terminate.

It should be noted that it would be equally valid for the programs to run at different speeds; for example, program 0 might have sent all three values and then stopped at the first rcv before program 1 executed even its first instruction.

Once both of your programs have terminated (regardless of what caused them to do so), how many times did program 1 send a value?

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
                RegisterValues['p'] = Id;
            }

            public Operation ExecuteInstruction(bool sendReceiveMode)
            {
                //snd X plays a sound with a frequency equal to the value of X.
                //set X Y sets register X to the value of Y.
                //add X Y increases register X by the value of Y.
                //mul X Y sets register X to the result of multiplying the value contained in register X by the value of Y.
                //mod X Y sets register X to the remainder of dividing the value contained in register X by the value of Y (that is, it sets X to the result of X modulo Y).
                //rcv X recovers the frequency of the last sound played, but only when the value of X is not zero.(If it is zero, the command does nothing.)
                //jgz X Y jumps with an offset of the value of Y, but only if the value of X is greater than zero.(An offset of 2 skips the next instruction, an offset of -1 jumps to the previous instruction, and so on.)
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
                    Operation.SND => sourceValue,
                    Operation.SET => sourceValue,
                    Operation.ADD => destinationValue + sourceValue,
                    Operation.MUL => destinationValue * sourceValue,
                    Operation.MOD => destinationValue % sourceValue,
                    Operation.RCV => (sourceValue != 0) ? destinationValue : 0,
                    Operation.JGZ => (destinationValue > 0) ? sourceValue : 1,
                    _ => throw new InvalidProgramException($"Invalid opcode code")
                };
                if (operation == Operation.JGZ)
                {
                    Pc += resultValue;
                    return operation;
                }
                else if (operation == Operation.RCV)
                {
                    if (sendReceiveMode)
                    {
                        if (InputsCount > 0)
                        {
                            RegisterValues[destinationRegister] = Inputs[0];
                            for (var i = 1; i < InputsCount; ++i)
                            {
                                Inputs[i - 1] = Inputs[i];
                            }
                            --InputsCount;
                        }
                        else
                        {
                            // Do not move the program counter : waiting for input
                            return operation;
                        }
                    }
                    else
                    {
                        if (resultValue != 0)
                        {
                            RegisterValues[RCV_REGISTER] = sourceValue;
                        }
                    }
                }
                else
                {
                    RegisterValues[destinationRegister] = resultValue;
                }
                ++Pc;
                return operation;
            }

            public void RunProgram(bool sendRecieve, out bool send, out bool receive)
            {
                send = false;
                receive = false;
                while ((Pc >= 0) && (Pc < InstructionCount))
                {
                    var operation = ExecuteInstruction(sendRecieve);
                    if (operation == Operation.RCV)
                    {
                        receive = true;
                        return;
                    }
                    if (operation == Operation.SND)
                    {
                        send = true;
                        return;
                    }
                }
                throw new IndexOutOfRangeException($"Illegal pc {Pc} range 0-{InstructionCount - 1}");
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
                        "snd" => Operation.SND,
                        "set" => Operation.SET,
                        "add" => Operation.ADD,
                        "mul" => Operation.MUL,
                        "mod" => Operation.MOD,
                        "rcv" => Operation.RCV,
                        "jgz" => Operation.JGZ,
                        _ => Operation.ILLEGAL,
                    };
                    Operations[InstructionCount] = op;

                    var destRegToken = tokens[1].Trim();
                    if ((op == Operation.SET) || (op == Operation.ADD) || (op == Operation.MUL) || (op == Operation.MOD) || (op == Operation.JGZ))
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
                    else if (op == Operation.SND)
                    {
                        int srcValueRegister = 'V';
                        if (destRegToken.Length == 1)
                        {
                            if ((destRegToken[0] >= 'a') && (destRegToken[0] <= 'z'))
                            {
                                SourceValues[InstructionCount] = 0;
                                srcValueRegister = destRegToken[0];
                            }
                        }
                        if (srcValueRegister == 'V')
                        {
                            SourceValues[InstructionCount] = int.Parse(destRegToken);
                        }
                        SourceRegisters[InstructionCount] = srcValueRegister;
                        DestinationRegisters[InstructionCount] = SND_REGISTER;
                    }
                    else if (op == Operation.RCV)
                    {
                        var destinationRegister = destRegToken[0];
                        if (destRegToken.Length == 1)
                        {
                            if ((destinationRegister >= 'a') && (destinationRegister <= 'z'))
                            {
                                SourceValues[InstructionCount] = 0;
                                SourceRegisters[InstructionCount] = SND_REGISTER;
                                DestinationRegisters[InstructionCount] = destinationRegister;
                            }
                            else
                            {
                                throw new IndexOutOfRangeException($"Invalid line '{line}' invalid register '{destinationRegister}'");
                            }
                        }
                        else
                        {
                            throw new IndexOutOfRangeException($"Invalid line '{line}' invalid register '{destRegToken}'");
                        }
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
                var result1 = FirstValidRcvFrequency(1024);
                Console.WriteLine($"Day18 : Result1 {result1}");
                var expected = 3423;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = FindDeadlock(1024 * 1024);
                Console.WriteLine($"Day18 : Result2 {result2}");
                var expected = 7493;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] lines)
        {
            sPrograms[0] = new Prog(0, lines);
            sPrograms[1] = new Prog(1, lines);
        }

        public static long FirstValidRcvFrequency(int maxCycleCount)
        {
            ref Prog prog = ref sPrograms[0];
            prog.Pc = 0;
            for (var c = 0; c < maxCycleCount; ++c)
            {
                prog.RunProgram(false, out _, out bool receive);
                if (receive)
                {
                    if (prog.RegisterValues[RCV_REGISTER] != 0)
                    {
                        return prog.RegisterValues[RCV_REGISTER];
                    }
                }
            }
            throw new InvalidProgramException($"No first valid rcv frequency found after {maxCycleCount} cycles");
        }

        public static long FindDeadlock(int maxCycleCount)
        {
            ref Prog prog0 = ref sPrograms[0];
            ref Prog prog1 = ref sPrograms[1];

            long output1Count = 0;

            for (var c = 0; c < maxCycleCount; ++c)
            {
                bool receive0;
                do
                {
                    prog0.RunProgram(true, out bool send0, out receive0);
                    if (send0)
                    {
                        var output = prog0.RegisterValues[SND_REGISTER];
                        prog1.AddInput(output);
                        //Console.WriteLine($"Prog0 Sends {output} {prog1.InputsCount}");
                    }
                }
                while (!receive0);

                bool receive1;
                do
                {
                    prog1.RunProgram(true, out bool send1, out receive1);
                    if (send1)
                    {
                        var output = prog1.RegisterValues[SND_REGISTER];
                        prog0.AddInput(output);
                        //Console.WriteLine($"Prog1 Sends {output} {prog0.InputsCount}");
                        ++output1Count;
                    }
                }
                while (!receive1);

                if (receive0 && receive1)
                {
                    if ((prog0.InputsCount == 0) && (prog1.InputsCount == 0))
                    {
                        return output1Count;
                    }
                }
            }
            throw new InvalidProgramException($"No deadlock found after {maxCycleCount} cycles");
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
