using System;

/*

--- Day 16: Permutation Promenade ---

You come upon a very unusual sight; a group of programs here appear to be dancing.

There are sixteen programs in total, named a through p.
They start by standing in a line: a stands in position 0, b stands in position 1, and so on until p, which stands in position 15.

The programs' dance consists of a sequence of dance moves:

Spin, written sX, makes X programs move from the end to the front, but maintain their order otherwise.
(For example, s3 on abcde produces cdeab).
Exchange, written xA/B, makes the programs at positions A and B swap places.
Partner, written pA/B, makes the programs named A and B swap places.
For example, with only five programs standing in a line (abcde), they could do the following dance:

s1, a spin of size 1: eabcd.
x3/4, swapping the last two programs: eabdc.
pe/b, swapping programs e and b: baedc.
After finishing their dance, the programs end up in order baedc.

You watch the dance for a while and record their dance moves (your puzzle input).
In what order are the programs standing after their dance?

Your puzzle answer was nlciboghjmfdapek.

The first half of this puzzle is complete! It provides one gold star: *

--- Part Two ---

Now that you're starting to get a feel for the dance moves, you turn your attention to the dance as a whole.

Keeping the positions they ended up in from their previous dance, the programs perform it again and again: including the first dance, a total of one billion (1000000000) times.

In the example above, their second dance would begin with the order baedc, and use the same dance moves:

s1, a spin of size 1: cbaed.
x3/4, swapping the last two programs: cbade.
pe/b, swapping programs e and b: ceadb.

In what order are the programs standing after their billion dances?

*/

namespace Day16
{
    class Program
    {
        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            if (lines.Length != 1)
            {
                throw new InvalidProgramException($"Invalid input expected a single line");
            }
            var moves = lines[0];

            if (part1)
            {
                var result1 = DanceMoves(moves, 16, 1, true);
                Console.WriteLine($"Day16 : Result1 {result1}");
                var expected = "glnacbhedpfjkiom";
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = DanceMoves(moves, 16, 1000 * 1000 * 1000, false);
                var expected = "fmpanloehgkdcbji";
                Console.WriteLine($"Day16 : Result2 {result2}");
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static string DanceMoves(string moves, int length, int repeat, bool bruteForce)
        {
            var pastMoves = new string[repeat];
            var programs = new char[length];
            var lastPrograms = new char[length];
            for (var i = 0; i < length; ++i)
            {
                programs[i] = (char)('a' + i);
            }
            var tokens = moves.Trim().Split(',');
            var moveCount = tokens.Length;
            var spinSizes = new int[moveCount];
            var swapIndexAs = new int[moveCount];
            var swapIndexBs = new int[moveCount];
            var swapProgramAs = new char[moveCount];
            var swapProgramBs = new char[moveCount];

            var moveIndex = 0;
            foreach (var move in tokens)
            {
                var type = move.Trim()[0];
                var buffer = move.Trim().Substring(1);
                var spinSize = 0;
                var swapIndexA = -1;
                var swapIndexB = -1;
                var programA = '0';
                var programB = '0';
                if (type == 's')
                {
                    spinSize = int.Parse(buffer);
                    if ((spinSize < 1) || (spinSize > length))
                    {
                        throw new InvalidProgramException($"Invalid spinSize {spinSize} Range:1-{length}");
                    }
                }
                else if (type == 'x')
                {
                    var indexes = buffer.Split('/');
                    swapIndexA = int.Parse(indexes[0]);
                    swapIndexB = int.Parse(indexes[1]);
                    if ((swapIndexA < 0) || (swapIndexA >= length))
                    {
                        throw new InvalidProgramException($"Invalid swapIndexA {swapIndexA} Range:0-{length - 1}");
                    }
                    if ((swapIndexB < 0) || (swapIndexB >= length))
                    {
                        throw new InvalidProgramException($"Invalid swapIndexB {swapIndexB} Range:0-{length - 1}");
                    }
                }
                else if (type == 'p')
                {
                    var indexes = buffer.Split('/');
                    programA = indexes[0][0];
                    programB = indexes[1][0];
                }
                else
                {
                    throw new InvalidProgramException($"Unknown move type {move}");
                }
                spinSizes[moveIndex] = spinSize;
                swapIndexAs[moveIndex] = swapIndexA;
                swapIndexBs[moveIndex] = swapIndexB;
                swapProgramAs[moveIndex] = programA;
                swapProgramBs[moveIndex] = programB;
                ++moveIndex;
            }
            if (moveIndex != moveCount)
            {
                throw new InvalidProgramException($"Invalid moveIndex {moveIndex} != {moveCount}");
            }

            var repeatCycleStart = -1;
            var repeatCycleEnd = -1;
            for (var r = 0; r < repeat; ++r)
            {
                for (var m = 0; m < moveCount; ++m)
                {
                    for (var i = 0; i < length; ++i)
                    {
                        lastPrograms[i] = programs[i];
                    }
                    var spinSize = spinSizes[m];
                    var swapIndexA = swapIndexAs[m];
                    var swapIndexB = swapIndexBs[m];
                    var swapProgramA = swapProgramAs[m];
                    var swapProgramB = swapProgramBs[m];

                    if (spinSize > 0)
                    {
                        for (var i = 0; i < spinSize; ++i)
                        {
                            programs[i] = lastPrograms[i + length - spinSize];
                        }
                        for (var i = spinSize; i < length; ++i)
                        {
                            programs[i] = lastPrograms[i - spinSize];
                        }
                    }
                    else if ((swapIndexA >= 0) && (swapIndexB >= 0))
                    {
                        programs[swapIndexA] = lastPrograms[swapIndexB];
                        programs[swapIndexB] = lastPrograms[swapIndexA];
                    }
                    else if ((swapProgramA != '0') && (swapProgramB >= '0'))
                    {
                        for (var i = 0; i < length; ++i)
                        {
                            if (lastPrograms[i] == swapProgramA)
                            {
                                swapIndexA = i;
                                break;
                            }
                        }
                        for (var i = 0; i < length; ++i)
                        {
                            if (lastPrograms[i] == swapProgramB)
                            {
                                swapIndexB = i;
                                break;
                            }
                        }
                        programs[swapIndexA] = lastPrograms[swapIndexB];
                        programs[swapIndexB] = lastPrograms[swapIndexA];
                    }
                    else
                    {
                        throw new InvalidProgramException($"Unknown move {m}");
                    }
                }
                var thisMove = new string(programs);
                if (!bruteForce)
                {
                    var pastRepeat = -1;
                    for (var r2 = 0; r2 < r; ++r2)
                    {
                        if (pastMoves[r2] == thisMove)
                        {
                            pastRepeat = r2;
                            break;
                        }
                    }
                    if (pastRepeat == -1)
                    {
                        pastMoves[r] = thisMove;
                        //Console.WriteLine($"Repeat[{r}] '{thisMove}'");
                    }
                    else
                    {
                        repeatCycleStart = pastRepeat;
                        repeatCycleEnd = r;
                        break;
                    }
                }
            }
            if (!bruteForce)
            {
                if ((repeatCycleStart != -1) && (repeatCycleEnd != -1))
                {
                    var repeatCycleLength = repeatCycleEnd - repeatCycleStart + 1;
                    Console.WriteLine($"Repeat Cycle Found {repeatCycleEnd} -> {repeatCycleStart} Length:{repeatCycleLength}");
                    var pastCycle = (repeat - 1 - repeatCycleStart) % repeatCycleEnd;
                    return pastMoves[pastCycle];
                }
                else
                {
                    throw new InvalidProgramException($"Repeat cycle not found after {repeat} iterations");
                }
            }
            return new string(programs);
        }

        public static void Run()
        {
            Console.WriteLine("Day16 : Start");
            _ = new Program("Day16/input.txt", true);
            _ = new Program("Day16/input.txt", false);
            Console.WriteLine("Day16 : End");
        }
    }
}
