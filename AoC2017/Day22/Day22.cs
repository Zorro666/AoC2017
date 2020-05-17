using System;

/*

--- Day 22: Sporifica Virus ---

Diagnostics indicate that the local grid computing cluster has been contaminated with the Sporifica Virus.
The grid computing cluster is a seemingly-infinite two-dimensional grid of compute nodes.
Each node is either clean or infected by the virus.

To prevent overloading the nodes (which would render them useless to the virus) or detection by system administrators, exactly one virus carrier moves through the network, infecting or cleaning nodes as it moves.
The virus carrier is always located on a single node in the network (the current node) and keeps track of the direction it is facing.

To avoid detection, the virus carrier works in bursts; in each burst, it wakes up, does some work, and goes back to sleep.
The following steps are all executed in order one time each burst:

If the current node is infected, it turns to its right.
Otherwise, it turns to its left.
(Turning is done in-place; the current node does not change.)
If the current node is clean, it becomes infected.
Otherwise, it becomes cleaned.
(This is done after the node is considered for the purposes of changing direction.)
The virus carrier moves forward one node in the direction it is facing.
Diagnostics have also provided a map of the node infection status (your puzzle input).
Clean nodes are shown as .; infected nodes are shown as #.
This map only shows the center of the grid; there are many more nodes beyond those shown, but none of them are currently infected.

The virus carrier begins in the middle of the map facing up.

For example, suppose you are given a map like this:

..#
#..
...
Then, the middle of the infinite grid looks like this, with the virus carrier's position marked with [ ]:

. . . . . . . . .
. . . . . . . . .
. . . . . . . . .
. . . . . # . . .
. . . #[.]. . . .
. . . . . . . . .
. . . . . . . . .
. . . . . . . . .
The virus carrier is on a clean node, so it turns left, infects the node, and moves left:

. . . . . . . . .
. . . . . . . . .
. . . . . . . . .
. . . . . # . . .
. . .[#]# . . . .
. . . . . . . . .
. . . . . . . . .
. . . . . . . . .
The virus carrier is on an infected node, so it turns right, cleans the node, and moves up:

. . . . . . . . .
. . . . . . . . .
. . . . . . . . .
. . .[.]. # . . .
. . . . # . . . .
. . . . . . . . .
. . . . . . . . .
. . . . . . . . .
Four times in a row, the virus carrier finds a clean, infects it, turns left, and moves forward, ending in the same place and still facing up:

. . . . . . . . .
. . . . . . . . .
. . . . . . . . .
. . #[#]. # . . .
. . # # # . . . .
. . . . . . . . .
. . . . . . . . .
. . . . . . . . .
Now on the same node as before, it sees an infection, which causes it to turn right, clean the node, and move forward:

. . . . . . . . .
. . . . . . . . .
. . . . . . . . .
. . # .[.]# . . .
. . # # # . . . .
. . . . . . . . .
. . . . . . . . .
. . . . . . . . .
After the above actions, a total of 7 bursts of activity had taken place.
Of them, 5 bursts of activity caused an infection.

After a total of 70, the grid looks like this, with the virus carrier facing up:

. . . . . # # . .
. . . . # . . # .
. . . # . . . . #
. . # . #[.]. . #
. . # . # . . # .
. . . . . # # . .
. . . . . . . . .
. . . . . . . . .
By this time, 41 bursts of activity caused an infection (though most of those nodes have since been cleaned).

After a total of 10000 bursts of activity, 5587 bursts will have caused an infection.

Given your actual map, after 10000 bursts of activity, how many bursts cause a node to become infected? (Do not count nodes that begin infected.)

Your puzzle answer was 5322.

--- Part Two ---

As you go to remove the virus from the infected nodes, it evolves to resist your attempt.

Now, before it infects a clean node, it will weaken it to disable your defenses.
If it encounters an infected node, it will instead flag the node to be cleaned in the future.
So:

Clean nodes become weakened.
Weakened nodes become infected.
Infected nodes become flagged.
Flagged nodes become clean.
Every node is always in exactly one of the above states.

The virus carrier still functions in a similar way, but now uses the following logic during its bursts of action:

Decide which way to turn based on the current node:
If it is clean, it turns left.
If it is weakened, it does not turn, and will continue moving in the same direction.
If it is infected, it turns right.
If it is flagged, it reverses direction, and will go back the way it came.
Modify the state of the current node, as described above.
The virus carrier moves forward one node in the direction it is facing.
Start with the same map (still using . for clean and # for infected) and still with the virus carrier starting in the middle and facing up.

Using the same initial state as the previous example, and drawing weakened as W and flagged as F, the middle of the infinite grid looks like this, with the virus carrier's position again marked with [ ]:

. . . . . . . . .
. . . . . . . . .
. . . . . . . . .
. . . . . # . . .
. . . #[.]. . . .
. . . . . . . . .
. . . . . . . . .
. . . . . . . . .
This is the same as before, since no initial nodes are weakened or flagged.
The virus carrier is on a clean node, so it still turns left, instead weakens the node, and moves left:

. . . . . . . . .
. . . . . . . . .
. . . . . . . . .
. . . . . # . . .
. . .[#]W . . . .
. . . . . . . . .
. . . . . . . . .
. . . . . . . . .
The virus carrier is on an infected node, so it still turns right, instead flags the node, and moves up:

. . . . . . . . .
. . . . . . . . .
. . . . . . . . .
. . .[.]. # . . .
. . . F W . . . .
. . . . . . . . .
. . . . . . . . .
. . . . . . . . .
This process repeats three more times, ending on the previously-flagged node and facing right:

. . . . . . . . .
. . . . . . . . .
. . . . . . . . .
. . W W . # . . .
. . W[F]W . . . .
. . . . . . . . .
. . . . . . . . .
. . . . . . . . .
Finding a flagged node, it reverses direction and cleans the node:

. . . . . . . . .
. . . . . . . . .
. . . . . . . . .
. . W W . # . . .
. .[W]. W . . . .
. . . . . . . . .
. . . . . . . . .
. . . . . . . . .
The weakened node becomes infected, and it continues in the same direction:

. . . . . . . . .
. . . . . . . . .
. . . . . . . . .
. . W W . # . . .
.[.]# . W . . . .
. . . . . . . . .
. . . . . . . . .
. . . . . . . . .

Of the first 100 bursts, 26 will result in infection.

Unfortunately, another feature of this evolved virus is speed; of the first 10000000 bursts, 2511944 will result in infection.

Given your actual map, after 10000000 bursts of activity, how many bursts cause a node to become infected? (Do not count nodes that begin infected.)

*/

namespace Day22
{
    class Program
    {
        readonly static int MAX_BOARD_SIZE = 512;
        readonly static int[,] sBoard = new int[MAX_BOARD_SIZE, MAX_BOARD_SIZE];
        static int sVirusX;
        static int sVirusY;
        static int sVirusDX;
        static int sVirusDY;

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);

            if (part1)
            {
                var result1 = InfectedBurstCount(10000);
                Console.WriteLine($"Day22 : Result1 {result1}");
                var expected = 5322;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = -123;
                Console.WriteLine($"Day22 : Result2 {result2}");
                var expected = 1797;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] lines)
        {
            if (lines.Length == 0)
            {
                throw new InvalidProgramException($"Invalid input need at least one line {lines.Length}");
            }

            var height = lines.Length;
            if ((height % 2) == 0)
            {
                throw new InvalidProgramException($"Invalid height {height} must be odd");
            }

            var width = lines[0].Trim().Length;
            if ((width % 2) == 0)
            {
                throw new InvalidProgramException($"Invalid width {width} must be odd");
            }

            for (var y = 0; y < MAX_BOARD_SIZE; ++y)
            {
                for (var x = 0; x < MAX_BOARD_SIZE; ++x)
                {
                    sBoard[x, y] = 0;
                }
            }

            var y0 = MAX_BOARD_SIZE / 2 - (height - 1) / 2;
            var x0 = MAX_BOARD_SIZE / 2 - (width - 1) / 2;

            for (var y = 0; y < height; ++y)
            {
                var line = lines[y].Trim();
                if (line.Length != width)
                {
                    throw new InvalidProgramException($"Invalid input row {y} different width expected {width} got {line.Length}");
                }
                for (var x = 0; x < width; ++x)
                {
                    sBoard[x0 + x, y0 + y] = line[x] switch
                    {
                        '.' => 0,
                        '#' => 1,
                        _ => throw new InvalidProgramException($"Invalid input at {x},{y} '{line[x]}'")
                    };
                }
            }
        }

        static void OutputBoard()
        {
            Console.WriteLine("--------------------------------");
            for (var y = 0; y < MAX_BOARD_SIZE; ++y)
            {
                for (var x = 0; x < MAX_BOARD_SIZE; ++x)
                {
                    var c = sBoard[x, y] == 1 ? '#' : '.';
                    if ((x == sVirusX) && (y == sVirusY))
                    {
                        if ((sVirusDY == 1) && (sVirusDX == 0))
                        {
                            c = 'V';
                        }
                        else if ((sVirusDY == -1) && (sVirusDX == 0))
                        {
                            c = '^';
                        }
                        else if ((sVirusDY == 0) && (sVirusDX == 1))
                        {
                            c = '>';
                        }
                        else if ((sVirusDY == 0) && (sVirusDX == -1))
                        {
                            c = '<';
                        }
                        else
                        {
                            throw new InvalidProgramException($"Invalid dx,dy {sVirusDX},{sVirusDY}");
                        }
                    }
                    Console.Write($"{c}");
                }
                Console.WriteLine();
            }
            Console.WriteLine("--------------------------------");
        }

        static int SingleStep()
        {
            var wasClean = sBoard[sVirusX, sVirusY] == 0;

            int newDX;
            int newDY;
            if (wasClean)
            {
                // turn left = (+dy,-dx)
                newDX = +sVirusDY;
                newDY = -sVirusDX;
                sBoard[sVirusX, sVirusY] = 1;
            }
            else
            {
                // turn right = (-dy,+dx)
                newDX = -sVirusDY;
                newDY = +sVirusDX;
                sBoard[sVirusX, sVirusY] = 0;
            }
            sVirusDX = newDX;
            sVirusDY = newDY;
            sVirusX += sVirusDX;
            sVirusY += sVirusDY;

            if ((sVirusX <= 0) || (sVirusX >= MAX_BOARD_SIZE))
            {
                throw new IndexOutOfRangeException($"Virus X out of range 0->{MAX_BOARD_SIZE}");
            }

            if ((sVirusY <= 0) || (sVirusY >= MAX_BOARD_SIZE))
            {
                throw new IndexOutOfRangeException($"Virus Y out of range 0->{MAX_BOARD_SIZE}");
            }

            return wasClean ? 1 : 0;
        }

        public static long InfectedBurstCount(int burstCount)
        {
            sVirusX = MAX_BOARD_SIZE / 2;
            sVirusY = MAX_BOARD_SIZE / 2;
            sVirusDX = 0;
            sVirusDY = -1;

            //OutputBoard();
            var totalInfectedBurstCount = 0L;
            for (var b = 0; b < burstCount; ++b)
            {
                totalInfectedBurstCount += SingleStep();
                //OutputBoard();
            }

            return totalInfectedBurstCount;
        }

        public static void Run()
        {
            Console.WriteLine("Day22 : Start");
            _ = new Program("Day22/input.txt", true);
            _ = new Program("Day22/input.txt", false);
            Console.WriteLine("Day22 : End");
        }
    }
}
