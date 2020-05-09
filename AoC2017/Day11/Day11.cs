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
                var result1 = HexSteps(moves);
                Console.WriteLine($"Day11 : Result1 {result1}");
                var expected = 696;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = -123;
                Console.WriteLine($"Day11 : Result2 {result2}");
                var expected = 1797;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static int HexSteps(string moves)
        {
            var counts = new int[6];
            var tokens = moves.Trim().Split(',');
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
                ++counts[(int)move];
            }

            int minSteps;
            int steps = int.MaxValue;
            do
            {
                minSteps = steps;
                int delta;
                // n + s : 0
                if (counts[(int)Direction.N] > counts[(int)Direction.S])
                {
                    delta = counts[(int)Direction.S];
                }
                else
                {
                    delta = counts[(int)Direction.N];
                }
                counts[(int)Direction.N] -= delta;
                counts[(int)Direction.S] -= delta;

                // ne + sw : 0
                if (counts[(int)Direction.NE] > counts[(int)Direction.SW])
                {
                    delta = counts[(int)Direction.SW];
                }
                else
                {
                    delta = counts[(int)Direction.NE];
                }
                counts[(int)Direction.NE] -= delta;
                counts[(int)Direction.SW] -= delta;

                // nw + se : 0
                if (counts[(int)Direction.NW] > counts[(int)Direction.SE])
                {
                    delta = counts[(int)Direction.SE];
                }
                else
                {
                    delta = counts[(int)Direction.NW];
                }
                counts[(int)Direction.NW] -= delta;
                counts[(int)Direction.SE] -= delta;

                // se + sw : s
                if (counts[(int)Direction.SE] > counts[(int)Direction.SW])
                {
                    delta = counts[(int)Direction.SW];
                }
                else
                {
                    delta = counts[(int)Direction.SE];
                }
                counts[(int)Direction.SE] -= delta;
                counts[(int)Direction.SW] -= delta;
                counts[(int)Direction.S] += delta;

                // ne + nw : n
                if (counts[(int)Direction.NE] > counts[(int)Direction.NW])
                {
                    delta = counts[(int)Direction.NW];
                }
                else
                {
                    delta = counts[(int)Direction.NE];
                }
                counts[(int)Direction.NE] -= delta;
                counts[(int)Direction.NW] -= delta;
                counts[(int)Direction.N] += delta;

                // ne + s : se
                if (counts[(int)Direction.NE] > counts[(int)Direction.S])
                {
                    delta = counts[(int)Direction.S];
                }
                else
                {
                    delta = counts[(int)Direction.NE];
                }
                counts[(int)Direction.NE] -= delta;
                counts[(int)Direction.S] -= delta;
                counts[(int)Direction.SE] += delta;

                // nw + s : sw
                if (counts[(int)Direction.NW] > counts[(int)Direction.S])
                {
                    delta = counts[(int)Direction.S];
                }
                else
                {
                    delta = counts[(int)Direction.NW];
                }
                counts[(int)Direction.NW] -= delta;
                counts[(int)Direction.S] -= delta;
                counts[(int)Direction.SW] += delta;

                // se + n : ne
                if (counts[(int)Direction.SE] > counts[(int)Direction.N])
                {
                    delta = counts[(int)Direction.N];
                }
                else
                {
                    delta = counts[(int)Direction.SE];
                }
                counts[(int)Direction.SE] -= delta;
                counts[(int)Direction.N] -= delta;
                counts[(int)Direction.NE] += delta;

                // sw + n : nw
                if (counts[(int)Direction.SW] > counts[(int)Direction.N])
                {
                    delta = counts[(int)Direction.N];
                }
                else
                {
                    delta = counts[(int)Direction.SW];
                }
                counts[(int)Direction.SW] -= delta;
                counts[(int)Direction.N] -= delta;
                counts[(int)Direction.NW] += delta;

                // n + s : 0
                if (counts[(int)Direction.N] > counts[(int)Direction.S])
                {
                    delta = counts[(int)Direction.S];
                }
                else
                {
                    delta = counts[(int)Direction.N];
                }
                counts[(int)Direction.N] -= delta;
                counts[(int)Direction.S] -= delta;

                steps = 0;
                steps += counts[0];
                steps += counts[1];
                steps += counts[2];
                steps += counts[3];
                steps += counts[4];
                steps += counts[5];
            } while (steps < minSteps);

            return minSteps;
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
