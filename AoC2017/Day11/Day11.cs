using System;

/*

--- Day 11: Hex Ed ---

Crossing the bridge, you've barely reached the other side of the stream when a program comes up to you, clearly in distress.
"It's my child process," she says, "he's gotten lost in an infinite grid!"

Fortunately for her, you have plenty of experience with infinite grids.

Unfortunately for you, it's a hex grid.

The hexagons ("hexes") in this grid are aligned such that adjacent hexes can be found to the north, northeast, southeast, south, southwest, and northwest:

  \ n  /
nw +--+ ne
  /    \
-+      +-
  \    /
sw +--+ se
  / s  \

You have the path the child process took.
Starting where he started, you need to determine the fewest number of steps required to reach him.
(A "step" means to move from the hex you are in to any adjacent hex.)

For example:

ne,ne,ne is 3 steps away.
ne,ne,sw,sw is 0 steps away (back where you started).
ne,ne,s,s is 2 steps away (se,se).
se,sw,se,sw,sw is 3 steps away (s,s,sw).

  \ n  /
nw +--+ ne
  /    \
-+      +-
  \    /
sw +--+ se
  / s  \

// n : dx = 0 : dy = 2
// nw : dx = -1 : dy = 1
// ne : dx = +1 : dy = 1
// s : dx = 0 : dy = -2
// sw : dx = -1 : dy = -1
// se : dx = +1 : dy = -1

// ne + sw : 0
// nw + se : 0
// n + s : 0
// se + sw : s
// ne + nw : n
// ne + s : se
// nw + s : sw
// se + n : ne
// sw + n : nw

Your puzzle answer was 696.

--- Part Two ---

How many steps away is the furthest he ever got from his starting position?

*/

namespace Day11
{
    class Program
    {
        readonly static int[] sCounts = new int[6];
        public enum Direction { N = 0, S = 1, NW = 2, NE = 3, SW = 4, SE = 5 };

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            if (lines.Length != 1)
            {
                throw new InvalidProgramException($"Invalid input expected a single line {lines.Length}");
            }
            var moves = lines[0];

            if (part1)
            {
                var result1 = HexSteps(moves).end;
                Console.WriteLine($"Day11 : Result1 {result1}");
                var expected = 696;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = HexSteps(moves).max;
                Console.WriteLine($"Day11 : Result2 {result2}");
                var expected = 1461;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static (int max, int end) HexSteps(string moves)
        {
            for (var i = 0; i < sCounts.Length; ++i)
            {
                sCounts[i] = 0;
            }

            var tokens = moves.Trim().Split(',');
            var steps = 0;
            var maxSteps = 0;
            foreach (var token in tokens)
            {
                var move = token switch
                {
                    "n" => Direction.N,
                    "s" => Direction.S,
                    "nw" => Direction.NW,
                    "ne" => Direction.NE,
                    "sw" => Direction.SW,
                    "se" => Direction.SE,
                    _ => throw new InvalidProgramException($"Unknown token {token}")
                };
                ++sCounts[(int)move];
                steps = TakeStep();
                maxSteps = Math.Max(steps, maxSteps);
            }
            return (maxSteps, steps);
        }

        static int TakeStep()
        {
            int lastSteps;
            int steps = int.MaxValue;
            do
            {
                lastSteps = steps;
                int delta;
                // n + s = 0
                if (sCounts[(int)Direction.N] > sCounts[(int)Direction.S])
                {
                    delta = sCounts[(int)Direction.S];
                }
                else
                {
                    delta = sCounts[(int)Direction.N];
                }
                sCounts[(int)Direction.N] -= delta;
                sCounts[(int)Direction.S] -= delta;

                // ne + sw = 0
                if (sCounts[(int)Direction.NE] > sCounts[(int)Direction.SW])
                {
                    delta = sCounts[(int)Direction.SW];
                }
                else
                {
                    delta = sCounts[(int)Direction.NE];
                }
                sCounts[(int)Direction.NE] -= delta;
                sCounts[(int)Direction.SW] -= delta;

                // nw + se = 0
                if (sCounts[(int)Direction.NW] > sCounts[(int)Direction.SE])
                {
                    delta = sCounts[(int)Direction.SE];
                }
                else
                {
                    delta = sCounts[(int)Direction.NW];
                }
                sCounts[(int)Direction.NW] -= delta;
                sCounts[(int)Direction.SE] -= delta;

                // se + sw = s
                if (sCounts[(int)Direction.SE] > sCounts[(int)Direction.SW])
                {
                    delta = sCounts[(int)Direction.SW];
                }
                else
                {
                    delta = sCounts[(int)Direction.SE];
                }
                sCounts[(int)Direction.SE] -= delta;
                sCounts[(int)Direction.SW] -= delta;
                sCounts[(int)Direction.S] += delta;

                // ne + nw = n
                if (sCounts[(int)Direction.NE] > sCounts[(int)Direction.NW])
                {
                    delta = sCounts[(int)Direction.NW];
                }
                else
                {
                    delta = sCounts[(int)Direction.NE];
                }
                sCounts[(int)Direction.NE] -= delta;
                sCounts[(int)Direction.NW] -= delta;
                sCounts[(int)Direction.N] += delta;

                // ne + s = se
                if (sCounts[(int)Direction.NE] > sCounts[(int)Direction.S])
                {
                    delta = sCounts[(int)Direction.S];
                }
                else
                {
                    delta = sCounts[(int)Direction.NE];
                }
                sCounts[(int)Direction.NE] -= delta;
                sCounts[(int)Direction.S] -= delta;
                sCounts[(int)Direction.SE] += delta;

                // nw + s = sw
                if (sCounts[(int)Direction.NW] > sCounts[(int)Direction.S])
                {
                    delta = sCounts[(int)Direction.S];
                }
                else
                {
                    delta = sCounts[(int)Direction.NW];
                }
                sCounts[(int)Direction.NW] -= delta;
                sCounts[(int)Direction.S] -= delta;
                sCounts[(int)Direction.SW] += delta;

                // se + n = ne
                if (sCounts[(int)Direction.SE] > sCounts[(int)Direction.N])
                {
                    delta = sCounts[(int)Direction.N];
                }
                else
                {
                    delta = sCounts[(int)Direction.SE];
                }
                sCounts[(int)Direction.SE] -= delta;
                sCounts[(int)Direction.N] -= delta;
                sCounts[(int)Direction.NE] += delta;

                // sw + n = nw
                if (sCounts[(int)Direction.SW] > sCounts[(int)Direction.N])
                {
                    delta = sCounts[(int)Direction.N];
                }
                else
                {
                    delta = sCounts[(int)Direction.SW];
                }
                sCounts[(int)Direction.SW] -= delta;
                sCounts[(int)Direction.N] -= delta;
                sCounts[(int)Direction.NW] += delta;

                steps = 0;
                for (var i = 0; i < sCounts.Length; ++i)
                {
                    steps += sCounts[i];
                }
            } while (steps != lastSteps);

            return steps;
        }

        public static void Run()
        {
            Console.WriteLine("Day11 : Start");
            _ = new Program("Day11/input.txt", true);
            _ = new Program("Day11/input.txt", false);
            Console.WriteLine("Day11 : End");
        }
    }
}
