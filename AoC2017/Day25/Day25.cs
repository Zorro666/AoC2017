using System;

/*

--- Day 25: The Halting Problem ---

Following the twisty passageways deeper and deeper into the CPU, you finally reach the core of the computer.
Here, in the expansive central chamber, you find a grand apparatus that fills the entire room, suspended nanometers above your head.

You had always imagined CPUs to be noisy, chaotic places, bustling with activity.
Instead, the room is quiet, motionless, and dark.

Suddenly, you and the CPU's garbage collector startle each other.
"It's not often we get many visitors here!", he says.
You inquire about the stopped machinery.

"It stopped milliseconds ago; not sure why.
I'm a garbage collector, not a doctor." You ask what the machine is for.

"Programs these days, don't know their origins.
That's the Turing machine! It's what makes the whole computer work." You try to explain that Turing machines are merely models of computation, but he cuts you off.
"No, see, that's just what they want you to think.
Ultimately, inside every CPU, there's a Turing machine driving the whole thing! Too bad this one's broken.
We're doomed!"

You ask how you can help.
"Well, unfortunately, the only way to get the computer running again would be to create a whole new Turing machine from scratch, but there's no way you can-" He notices the look on your face, gives you a curious glance, shrugs, and goes back to sweeping the floor.

You find the Turing machine blueprints (your puzzle input) on a tablet in a nearby pile of debris.
Looking back up at the broken Turing machine above, you can start to identify its parts:

A tape which contains 0 repeated infinitely to the left and right.
A cursor, which can move left or right along the tape and read or write values at its current position.
A set of states, each containing rules about what to do based on the current value under the cursor.
Each slot on the tape has two possible values: 0 (the starting value for all slots) and 1.
Based on whether the cursor is pointing at a 0 or a 1, the current state says what value to write at the current position of the cursor, whether to move the cursor left or right one slot, and which state to use next.

For example, suppose you found the following blueprint:

Begin in state A.
Perform a diagnostic checksum after 6 steps.

In state A:
  If the current value is 0:
    - Write the value 1.
    - Move one slot to the right.
    - Continue with state B.
  If the current value is 1:
    - Write the value 0.
    - Move one slot to the left.
    - Continue with state B.

In state B:
  If the current value is 0:
    - Write the value 1.
    - Move one slot to the left.
    - Continue with state A.
  If the current value is 1:
    - Write the value 1.
    - Move one slot to the right.
    - Continue with state A.
Running it until the number of steps required to take the listed diagnostic checksum would result in the following tape configurations (with the cursor marked in square brackets):

... 0  0  0 [0] 0  0 ... (before any steps; about to run state A)
... 0  0  0  1 [0] 0 ... (after 1 step;     about to run state B)
... 0  0  0 [1] 1  0 ... (after 2 steps;    about to run state A)
... 0  0 [0] 0  1  0 ... (after 3 steps;    about to run state B)
... 0 [0] 1  0  1  0 ... (after 4 steps;    about to run state A)
... 0  1 [1] 0  1  0 ... (after 5 steps;    about to run state B)
... 0  1  1 [0] 1  0 ... (after 6 steps;    about to run state A)

The CPU can confirm that the Turing machine is working by taking a diagnostic checksum after a specific number of steps (given in the blueprint).
Once the specified number of steps have been executed, the Turing machine should pause; once it does, count the number of times 1 appears on the tape.
In the above example, the diagnostic checksum is 3.

Recreate the Turing machine and save the computer! What is the diagnostic checksum it produces once it's working again?

Your puzzle answer was 633.

--- Part Two ---

The Turing machine, and soon the entire computer, springs back to life. A console glows dimly nearby, awaiting your command.

> reboot printer
Error: That command requires priority 50. You currently have priority 0.
You must deposit 50 stars to increase your priority to the required level.
The console flickers for a moment, and then prints another message:

Star accepted.
You must deposit 49 stars to increase your priority to the required level.
The garbage collector winks at you, then continues sweeping.

*/

namespace Day25
{
    class Program
    {
        const long MAX_NUM_TAPE_SLOTS = 1L * 1024 * 16;
        readonly static byte[] sTapeValues = new byte[MAX_NUM_TAPE_SLOTS];

        const int MAX_NUM_STATES = 1024;
        static int sNumStates;
        readonly static string[] sStateNames = new string[MAX_NUM_STATES];
        readonly static byte[,] sStateWriteValues = new byte[MAX_NUM_STATES, 2];
        readonly static int[,] sStateMoveDelta = new int[MAX_NUM_STATES, 2];
        readonly static string[,] sStateTargetStates = new string[MAX_NUM_STATES, 2];
        readonly static int[,] sStateTargetStateIndexes = new int[MAX_NUM_STATES, 2];

        static string sStartingState = null;
        static long sTotalStepsCount = int.MinValue;

        private Program(string inputFile)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);

            var result1 = RunMachine();
            Console.WriteLine($"Day25 : Result1 {result1}");
            var expected = 2474;
            if (result1 != expected)
            {
                throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
            }
        }

        public static void Parse(string[] lines)
        {
            if (lines.Length < 3)
            {
                throw new InvalidProgramException($"Invalid input need at least 3 lines got {lines.Length}");
            }

            sNumStates = 0;
            sStartingState = null;
            sTotalStepsCount = long.MinValue;

            bool expectingStartLine = true;
            bool expectingStepsLine = false;
            bool expectingStartStateLine = false;
            bool expectingStartIfLine = false;
            bool expectingWriteValueLine = false;
            bool expectingMoveLine = false;
            bool expectingContinueLine = false;

            long currentIfValue = long.MinValue;
            bool foundIfOne = false;
            bool foundIfZero = false;

            foreach (var line in lines)
            {
                var trimLine = line.Trim();
                if (trimLine.Length == 0)
                {
                    continue;
                }
                // 'Begin in state A.'
                if (trimLine.StartsWith("Begin in state "))
                {
                    if (!expectingStartLine)
                    {
                        throw new InvalidProgramException($"Invalid line '{trimLine}' not expecting a start line");
                    }
                    var startTokens = trimLine.Split();
                    if (startTokens.Length != 4)
                    {
                        throw new InvalidProgramException($"Invalid start line '{trimLine}' expected 4 tokens got {startTokens.Length}");
                    }

                    sStartingState = startTokens[3].TrimEnd('.');
                    expectingStartLine = false;
                    expectingStepsLine = true;
                }
                // 'Perform a diagnostic checksum after 6 steps.'
                else if (trimLine.StartsWith("Perform a diagnostic checksum after "))
                {
                    if (!expectingStepsLine)
                    {
                        throw new InvalidProgramException($"Invalid line '{trimLine}' not expecting a steps line");
                    }
                    var stepsTokens = trimLine.Split();
                    if (stepsTokens.Length != 7)
                    {
                        throw new InvalidProgramException($"Invalid steps line '{trimLine}' expected 7 tokens got {stepsTokens.Length}");
                    }
                    if (stepsTokens[6] != "steps.")
                    {
                        throw new InvalidProgramException($"Invalid steps line '{trimLine}' expected 'steps.' got {stepsTokens[6]}");
                    }

                    sTotalStepsCount = long.Parse(stepsTokens[5]);
                    expectingStepsLine = false;
                    expectingStartStateLine = true;
                }
                // 'In state A:'
                else if (trimLine.StartsWith("In state "))
                {
                    if (!expectingStartStateLine)
                    {
                        throw new InvalidProgramException($"Invalid line '{trimLine}' not expecting the start of a state block");
                    }
                    var stateTokens = trimLine.Split();
                    if (stateTokens.Length != 3)
                    {
                        throw new InvalidProgramException($"Invalid state line '{trimLine}' expected 3 tokens got {stateTokens.Length}");
                    }

                    var stateName = stateTokens[2].TrimEnd(':');
                    for (var s = 0; s < sNumStates; ++s)
                    {
                        if (sStateNames[s] == stateName)
                        {
                            throw new InvalidProgramException($"Invalid state line '{trimLine}' state '{stateName}' already exists");
                        }
                    }
                    sStateNames[sNumStates] = stateName;
                    expectingStartStateLine = false;
                    expectingStartIfLine = true;
                    foundIfOne = false;
                    foundIfZero = false;
                }
                // '  If the current value is 0:'
                else if (trimLine.StartsWith("If the current value is "))
                {
                    if (!expectingStartIfLine)
                    {
                        throw new InvalidProgramException($"Invalid line '{trimLine}' not expecting the start of a if block");
                    }
                    var valueTokens = trimLine.Split();
                    if (valueTokens.Length != 6)
                    {
                        throw new InvalidProgramException($"Invalid if value line '{trimLine}' expected 6 tokens got {valueTokens.Length}");
                    }
                    var valueToken = valueTokens[5].TrimEnd(':');
                    currentIfValue = long.Parse(valueToken);
                    if (currentIfValue == 0)
                    {
                        if (foundIfZero)
                        {
                            throw new InvalidProgramException($"Invalid if value line '{trimLine}' already found if 0 line");
                        }
                        foundIfZero = true;
                    }
                    else if (currentIfValue == 1)
                    {
                        if (foundIfOne)
                        {
                            throw new InvalidProgramException($"Invalid if value line '{trimLine}' already found if 1 line");
                        }
                        foundIfOne = true;
                    }
                    else
                    {
                        throw new InvalidProgramException($"Invalid if value line '{trimLine}' test value must be 0 or 1 got {currentIfValue}");
                    }

                    expectingStartIfLine = false;
                    expectingWriteValueLine = true;
                }
                // '    - Write the value 1.'
                else if (trimLine.StartsWith("- Write the value"))
                {
                    if (!expectingWriteValueLine)
                    {
                        throw new InvalidProgramException($"Invalid line '{trimLine}' not expecting the start of a if block");
                    }
                    var valueTokens = trimLine.Split();
                    if (valueTokens.Length != 5)
                    {
                        throw new InvalidProgramException($"Invalid write value line '{trimLine}' expected 5 tokens got {valueTokens.Length}");
                    }
                    var valueToken = valueTokens[4].TrimEnd('.');
                    var writeValue = byte.Parse(valueToken);
                    if ((writeValue != 0) && (writeValue != 1))
                    {
                        throw new InvalidProgramException($"Invalid write value line '{trimLine}' write value must be 0 or 1 got {writeValue}");
                    }

                    sStateWriteValues[sNumStates, currentIfValue] = writeValue;
                    expectingWriteValueLine = false;
                    expectingMoveLine = true;
                }
                // '    - Move one slot to the right.'
                else if (trimLine.StartsWith("- Move one slot to the "))
                {
                    if (!expectingMoveLine)
                    {
                        throw new InvalidProgramException($"Invalid line '{trimLine}' not expecting a move slot line");
                    }
                    var slotTokens = trimLine.Split();
                    if (slotTokens.Length != 7)
                    {
                        throw new InvalidProgramException($"Invalid move line '{trimLine}' expected 7 tokens got {slotTokens.Length}");
                    }
                    var move = slotTokens[6].TrimEnd('.');
                    if ((move != "left") && (move != "right"))
                    {
                        throw new InvalidProgramException($"Invalid move line '{trimLine}' move value must be 'left' or 'right' {move}");
                    }

                    sStateMoveDelta[sNumStates, currentIfValue] = (move == "left") ? -1 : +1;
                    expectingMoveLine = false;
                    expectingContinueLine = true;
                }
                // '    - Continue with state B.'
                else if (trimLine.StartsWith("- Continue with state "))
                {
                    if (!expectingContinueLine)
                    {
                        throw new InvalidProgramException($"Invalid line '{trimLine}' not expecting a continue line");
                    }
                    var stateTokens = trimLine.Split();
                    if (stateTokens.Length != 5)
                    {
                        throw new InvalidProgramException($"Invalid continue state line '{trimLine}' expected 7 tokens got {stateTokens.Length}");
                    }

                    sStateTargetStates[sNumStates, currentIfValue] = stateTokens[4].TrimEnd('.');
                    expectingContinueLine = false;

                    if (foundIfOne && foundIfZero)
                    {
                        expectingStartIfLine = false;
                        expectingStartStateLine = true;
                        ++sNumStates;
                    }
                    else
                    {
                        expectingStartIfLine = true;
                    }
                }
                else
                {
                    throw new InvalidProgramException($"Unhandled line '{trimLine}'");
                }
            }

            for (var s = 0; s < sNumStates; ++s)
            {
                for (var i = 0; i < 2; ++i)
                {
                    var targetState = sStateTargetStates[s, i];
                    var targetIndex = int.MinValue;
                    for (var s2 = 0; s2 < sNumStates; ++s2)
                    {
                        if (sStateNames[s2] == targetState)
                        {
                            targetIndex = s2;
                            break;
                        }
                    }
                    if (targetIndex == int.MinValue)
                    {
                        throw new InvalidProgramException($"Failed to find target state '{targetState}");
                    }
                    sStateTargetStateIndexes[s, i] = targetIndex;
                }
            }

            /*
            Console.WriteLine($"Starting State '{sStartingState}' StepsCount {sTotalStepsCount}");
            for (var s = 0; s < sNumStates; ++s)
            {
                Console.WriteLine($"State '{sStateNames[s]};");
                for (var i = 0; i < 2; ++i)
                {
                    Console.WriteLine($"  If value is {i}");
                    Console.WriteLine($"   - Write {sStateWriteValues[s, i]}");
                    Console.WriteLine($"   - Move {sStateMoveDelta[s, i]} '{((sStateMoveDelta[s, i] == -1) ? "left" : "right")}'");
                    Console.WriteLine($"   - Goto {sStateTargetStateIndexes[s, i]} '{sStateTargetStates[s, i]}'");
                }
            }
            */
        }

        public static long RunMachine()
        {
            var currentTapeIndex = MAX_NUM_TAPE_SLOTS / 2;

            var currentState = int.MinValue;
            for (var s = 0; s < sNumStates; ++s)
            {
                if (sStateNames[s] == sStartingState)
                {
                    currentState = s;
                    break;
                }
            }
            for (var i = 0; i < sTotalStepsCount; ++i)
            {
                if ((currentState < 0) || (currentState >= sNumStates))
                {
                    throw new InvalidProgramException($"Invalid currentState {currentState} range is 0-{sNumStates}");
                }
                if ((currentTapeIndex < 0) || (currentTapeIndex >= MAX_NUM_TAPE_SLOTS))
                {
                    throw new InvalidProgramException($"Invalid currentTapeIndex {currentTapeIndex} range is 0-{MAX_NUM_TAPE_SLOTS}");
                }
                var currentValue = sTapeValues[currentTapeIndex];
                if ((currentValue != 0) && (currentValue != 1))
                {
                    throw new InvalidProgramException($"Invalid currentValue {currentValue} not 0 or 1");
                }
                sTapeValues[currentTapeIndex] = sStateWriteValues[currentState, currentValue];
                currentTapeIndex += sStateMoveDelta[currentState, currentValue];
                currentState = sStateTargetStateIndexes[currentState, currentValue];
            }
            var crc = 0L;
            for (var i = 0; i < MAX_NUM_TAPE_SLOTS; ++i)
            {
                if (sTapeValues[i] == 1)
                {
                    ++crc;
                }
            }
            return crc;
        }


        public static void Run()
        {
            Console.WriteLine("Day25 : Start");
            _ = new Program("Day25/input.txt");
            Console.WriteLine("Day25 : End");
        }
    }
}
