using System;
using System.Collections.Generic;

/*

--- Day 6: Memory Reallocation ---

A debugger program here is having an issue: it is trying to repair a memory reallocation routine, but it keeps getting stuck in an infinite loop.

In this area, there are sixteen memory banks; each memory bank can hold any number of blocks.
The goal of the reallocation routine is to balance the blocks between the memory banks.

The reallocation routine operates in cycles.
In each cycle, it finds the memory bank with the most blocks (ties won by the lowest-numbered memory bank) and redistributes those blocks among the banks.
To do this, it removes all of the blocks from the selected bank, then moves to the next (by index) memory bank and inserts one of the blocks.
It continues doing this until it runs out of blocks; if it reaches the last memory bank, it wraps around to the first one.

The debugger would like to know how many redistributions can be done before a blocks-in-banks configuration is produced that has been seen before.

For example, imagine a scenario with only four memory banks:

The banks start with 0, 2, 7, and 0 blocks.
The third bank has the most blocks, so it is chosen for redistribution.
Starting with the next bank (the fourth bank) and then continuing to the first bank, the second bank, and so on, the 7 blocks are spread out over the memory banks.
The fourth, first, and second banks get two blocks each, and the third bank gets one back.
The final result looks like this: 2 4 1 2.
Next, the second bank is chosen because it contains the most blocks (four).
Because there are four memory banks, each gets one block.
The result is: 3 1 2 3.
Now, there is a tie between the first and fourth memory banks, both of which have three blocks.
The first bank wins the tie, and its three blocks are distributed evenly over the other three banks, leaving it with none: 0 2 3 4.
The fourth bank is chosen, and its four blocks are distributed such that each of the four banks receives one: 1 3 4 1.
The third bank is chosen, and the same thing happens: 2 4 1 2.
At this point, we've reached a state we've seen before: 2 4 1 2 was already seen.
The infinite loop is detected after the fifth block redistribution cycle, and so the answer in this example is 5.

Given the initial block counts in your puzzle input, how many redistribution cycles must be completed before a configuration is produced that has been seen before?

Your puzzle answer was 7864.

--- Part Two ---

Out of curiosity, the debugger would also like to know the size of the loop: 
starting from a state that has already been seen, 
how many block redistribution cycles must be performed before that same state is seen again?

In the example above, 2 4 1 2 is seen again after four cycles, and so the answer in that example would be 4.

How many cycles are in the infinite loop that arises from the configuration in your puzzle input?

*/

namespace Day06
{
    class Program
    {
        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            if (lines.Length != 1)
            {
                throw new InvalidProgramException($"Invalid input expected a single line got {lines.Length}");
            }
            var start = lines[0];

            if (part1)
            {
                var result1 = InfiniteLoop(start).stepCount;
                Console.WriteLine($"Day06 : Result1 {result1}");
                var expected = 11137;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = InfiniteLoop(start).loop;
                Console.WriteLine($"Day06 : Result2 {result2}");
                var expected = 1037;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static (int stepCount, int loop) InfiniteLoop(string start)
        {
            var tokens = start.Trim().Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);
            var binCount = tokens.Length;
            var values = new int[binCount];
            for (var i = 0; i < binCount; ++i)
            {
                var value = byte.Parse(tokens[i]);
                values[i] = value;
            }

            var stepCount = 0;
            var previousValues = new List<int[]>(10000);
            do
            {
                for (var i = 0; i < previousValues.Count; ++i)
                {
                    var previousValue = previousValues[i];
                    bool theSame = true;
                    for (var j = 0; j < binCount; ++j)
                    {
                        if (values[j] != previousValue[j])
                        {
                            theSame = false;
                            break;
                        }
                    }
                    if (theSame)
                    {
                        return (stepCount, stepCount - i);
                    }
                }

                var newValues = new int[binCount];
                var maxValue = int.MinValue;
                var startIndex = -1;
                for (var i = 0; i < binCount; ++i)
                {
                    var value = values[i];
                    newValues[i] = value;
                    if (value > maxValue)
                    {
                        startIndex = i;
                        maxValue = value;
                    }
                }
                previousValues.Add(newValues);

                values[startIndex] = 0;
                var giveOut = maxValue;
                var index = startIndex;
                while (giveOut > 0)
                {
                    index = (index + 1) % binCount;
                    ++values[index];
                    --giveOut;
                }
                ++stepCount;
            } while (stepCount < 50000);

            return (-1, -1);
        }

        public static void Run()
        {
            Console.WriteLine("Day06 : Start");
            _ = new Program("Day06/input.txt", true);
            _ = new Program("Day06/input.txt", false);
            Console.WriteLine("Day06 : End");
        }
    }
}
