using System;

/*

--- Day 17: Spinlock ---

Suddenly, whirling in the distance, you notice what looks like a massive, pixelated hurricane: a deadly spinlock.
This spinlock isn't just consuming computing power, but memory, too; vast, digital mountains are being ripped from the ground and consumed by the vortex.

If you don't move quickly, fixing that printer will be the least of your problems.

This spinlock's algorithm is simple but efficient, quickly consuming everything in its path.
It starts with a circular buffer containing only the value 0, which it marks as the current position.
It then steps forward through the circular buffer some number of steps (your puzzle input) before inserting the first new value, 1, after the value it stopped on.
The inserted value becomes the current position.
Then, it steps forward from there the same number of steps, and wherever it stops, inserts after it the second new value, 2, and uses that as the new current position again.

It repeats this process of stepping forward, inserting a new value, and using the location of the inserted value as the new current position a total of 2017 times, inserting 2017 as its final operation, and ending with a total of 2018 values (including 0) in the circular buffer.

For example, if the spinlock were to step 3 times per insert, the circular buffer would begin to evolve like this (using parentheses to mark the current position after each iteration of the algorithm):

(0), the initial state before any insertions.
0 (1): the spinlock steps forward three times (0, 0, 0), and then inserts the first value, 1, after it.
1 becomes the current position.
0 (2) 1: the spinlock steps forward three times (0, 1, 0), and then inserts the second value, 2, after it.
2 becomes the current position.
0  2 (3) 1: the spinlock steps forward three times (1, 0, 2), and then inserts the third value, 3, after it.
3 becomes the current position.
And so on:

0  2 (4) 3  1
0 (5) 2  4  3  1
0  5  2  4  3 (6) 1
0  5 (7) 2  4  3  6  1
0  5  7  2  4  3 (8) 6  1
0 (9) 5  7  2  4  3  8  6  1
Eventually, after 2017 insertions, the section of the circular buffer near the last insertion looks like this:

1512  1134  151 (2017) 638  1513  851
Perhaps, if you can identify the value that will ultimately be after the last value written (2017), you can short-circuit the spinlock.
In this example, that would be 638.

What is the value after 2017 in your completed circular buffer?

Your puzzle input is 370.

Your puzzle answer was 1244.

--- Part Two ---

The spinlock does not short-circuit. Instead, it gets more angry. At least, you assume that's what happened; it's spinning significantly faster than it was a moment ago.

You have good news and bad news.

The good news is that you have improved calculations for how to stop the spinlock. They indicate that you actually need to identify the value after 0 in the current state of the circular buffer.

The bad news is that while you were determining this, the spinlock has just finished inserting its fifty millionth value (50000000).

What is the value after 0 the moment 50000000 is inserted?

*/

namespace Day17
{
    class Program
    {
        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            if (lines.Length != 1)
            {
                throw new InvalidProgramException($"Bad input expected a single line got {lines.Length}");
            }
            var stepsPerInsert = int.Parse(lines[0]);

            if (part1)
            {
                var result1 = SpinLock(stepsPerInsert, 2018);
                Console.WriteLine($"Day17 : Result1 {result1}");
                var expected = 136;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = SpinLock2(stepsPerInsert, 50 * 1000 * 1000);
                Console.WriteLine($"Day17 : Result2 {result2}");
                var expected = 1080289;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static int SpinLock(int stepsPerInsert, int insertCount)
        {
            var bufferSize = 0;
            var values = new int[insertCount];
            var insertAt = 0;
            values[insertAt] = 0;
            ++bufferSize;
            for (var i = 1; i < insertCount; ++i)
            {
                insertAt += stepsPerInsert;
                insertAt %= bufferSize;
                ++insertAt;
                for (var j = bufferSize; j > insertAt; --j)
                {
                    values[j] = values[j - 1];
                }
                values[insertAt] = i;
                ++bufferSize;
            }
            return values[insertAt + 1];
        }

        public static int SpinLock2(int stepsPerInsert, int insertCount)
        {
            var bufferSize = 0;
            var insertAt = 0;
            ++bufferSize;
            var valueNextToZero = -1;
            for (var i = 1; i < insertCount + 1; ++i)
            {
                insertAt += stepsPerInsert;
                insertAt %= bufferSize;
                if (insertAt == 0)
                {
                    valueNextToZero = i;
                }
                ++insertAt;
                ++bufferSize;
            }
            return valueNextToZero;
        }

        public static void Run()
        {
            Console.WriteLine("Day17 : Start");
            _ = new Program("Day17/input.txt", true);
            _ = new Program("Day17/input.txt", false);
            Console.WriteLine("Day17 : End");
        }
    }
}
