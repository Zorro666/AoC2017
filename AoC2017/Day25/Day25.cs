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

*/

namespace Day25
{
    class Program
    {
        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);

            if (part1)
            {
                var result1 = RunMachine();
                Console.WriteLine($"Day25 : Result1 {result1}");
                var expected = 280;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = -123;
                Console.WriteLine($"Day25 : Result2 {result2}");
                var expected = 1797;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static long RunMachine()
        {
            return long.MinValue;
        }

        public static void Parse(string[] lines)
        {
            if (lines.Length < 3)
            {
                throw new InvalidProgramException($"Invalid input need at least 3 lines got {lines.Length}");
            }

            long stepsCount = long.MinValue;
            string startingState = null;


            bool expectingStartLine = true;
            bool expectingStepsLine = false;
            bool expectingStartStateLine = false;
            bool expectingStartIfLine = false;
            bool expectingWriteValueLine = false;
            bool expectingMoveLine = false;
            bool expectingContinueLine = false;

            foreach (var line in lines)
            {
                var trimLine = line.Trim();
                if (trimLine.Length == 0)
                {
                    continue;
                }
                // 'Begin in state A.'
                // 'Perform a diagnostic checksum after 6 steps.'
                // 'In state A:'
                // '  If the current value is 0:'
                // '    - Write the value 1.'
                // '    - Move one slot to the right.'
                // '    - Continue with state B.'
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
                    startingState = startTokens[3].TrimEnd('.');
                    expectingStartLine = false;
                    expectingStepsLine = true;
                }
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
                    stepsCount = long.Parse(stepsTokens[5]);
                    expectingStepsLine = false;
                    expectingStartStateLine = true;
                }
                else if (trimLine.StartsWith("In state "))
                {
                    if (!expectingStartStateLine)
                    {
                        throw new InvalidProgramException($"Invalid line '{trimLine}' not expecting the start of a state block");
                    }
                    expectingStartStateLine = false;
                    expectingStartIfLine = true;
                }
                else if (trimLine.StartsWith("If the current value is "))
                {
                    if (!expectingStartIfLine)
                    {
                        throw new InvalidProgramException($"Invalid line '{trimLine}' not expecting the start of a if block");
                    }
                    expectingStartIfLine = false;
                    expectingWriteValueLine = true;
                }
                else if (trimLine.StartsWith("- Write the value"))
                {
                    if (!expectingWriteValueLine)
                    {
                        throw new InvalidProgramException($"Invalid line '{trimLine}' not expecting the start of a if block");
                    }
                    expectingWriteValueLine = false;
                    expectingMoveLine = true;
                }
                else if (trimLine.StartsWith("- Move one slot to the "))
                {
                    expectingMoveLine = false;
                    expectingContinueLine = true;
                }
                else if (trimLine.StartsWith("- Continue with state "))
                {
                    expectingContinueLine = false;
                    expectingStartIfLine = true;
                }
                else
                {
                    throw new InvalidProgramException($"Unhandled line '{trimLine}'");
                }
            }
            Console.WriteLine($"Starting State '{startingState}' StepsCount {stepsCount}");
        }

        public static void Run()
        {
            Console.WriteLine("Day25 : Start");
            _ = new Program("Day25/input.txt", true);
            //_ = new Program("Day25/input.txt", false);
            Console.WriteLine("Day25 : End");
        }
    }
}
