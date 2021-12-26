using System;

/*

--- Day 19: A Series of Tubes ---

Somehow, a network packet got lost and ended up here.
It's trying to follow a routing diagram (your puzzle input), but it's confused about where to go.

Its starting point is just off the top of the diagram.
Lines (drawn with |, -, and +) show the path it needs to take, starting by going down onto the only line connected to the top of the diagram.
It needs to follow this path until it reaches the end (located somewhere within the diagram) and stop there.

Sometimes, the lines cross over each other; in these cases, it needs to continue going the same direction, and only turn left or right when there's no other option.
In addition, someone has left letters on the line; these also don't change its direction, but it can use them to keep track of where it's been.
For example:

     |          
     |  +--+    
     A  |  C    
 F---|----E|--+ 
     |  |  |  D 
     +B-+  +--+ 

Given this diagram, the packet needs to take the following path:

Starting at the only line touching the top of the diagram, it must go down, pass through A, and continue onward to the first +.
Travel right, up, and right, passing through B in the process.
Continue down (collecting C), right, and up (collecting D).
Finally, go all the way left through E and stopping at F.
Following the path to the end, the letters it sees on its path are ABCDEF.

The little packet looks up at you, hoping you can help it find the way.
What letters will it see (in the order it would see them) if it follows the path? (The routing diagram is very wide; make sure you view it without line wrapping.)

Your puzzle answer was AYRPVMEGQ.

--- Part Two ---

The packet is curious how many steps it needs to go.

For example, using the same routing diagram from the example above...

     |          
     |  +--+    
     A  |  C    
 F---|--|-E---+ 
     |  |  |  D 
     +B-+  +--+ 

...the packet would go:

6 steps down (including the first line at the top of the diagram).
3 steps right.
4 steps up.
3 steps right.
4 steps down.
3 steps right.
2 steps up.
13 steps left (including the F it stops on).
This would result in a total of 38 steps.

How many steps does the packet need to go?
*/

namespace Day19
{
    class Program
    {
        readonly static int MAP_EMPTY = 0;
        readonly static int MAP_PATH_NS = 1;
        readonly static int MAP_PATH_EW = 2;
        readonly static int MAP_TURN = 3;
        readonly static int MAP_LETTER_START = 100;

        readonly static int MAX_MAP_SIZE = 1024;
        readonly static int[,] sMap = new int[MAX_MAP_SIZE, MAX_MAP_SIZE];
        static int sWidth;
        static int sHeight;

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);

            if (part1)
            {
                var result1 = FindRoute();
                Console.WriteLine($"Day19 : Result1 {result1}");
                var expected = "MKXOIHZNBL";
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = RouteLength();
                Console.WriteLine($"Day19 : Result2 {result2}");
                var expected = 17872;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] lines)
        {
            if (lines.Length < 3)
            {
                throw new InvalidProgramException($"Invalid map input need at least 3 lines {lines.Length}");
            }
            sWidth = lines[0].Length;
            sHeight = lines.Length;

            for (var y = 0; y < sHeight; ++y)
            {
                var line = lines[y];
                var width = line.Length;
                sWidth = Math.Max(sWidth, width);
                for (var x = 0; x < width; ++x)
                {
                    var c = line[x];
                    var cell = c switch
                    {
                        ' ' => MAP_EMPTY,
                        '+' => MAP_TURN,
                        '|' => MAP_PATH_NS,
                        '-' => MAP_PATH_EW,
                        _ => MAP_LETTER_START
                    };
                    if (cell == MAP_LETTER_START)
                    {
                        if ((c >= 'A') && (c <= 'Z'))
                        {
                            cell = c - 'A' + MAP_LETTER_START;
                        }
                        else
                        {
                            throw new IndexOutOfRangeException($"Invalid character '{c}' at {x},{y}");
                        }
                    }
                    sMap[x, y] = cell;
                }
            }
        }

        public static long RouteLength()
        {
            return WalkRoute(out string route);
        }

        public static string FindRoute()
        {
            WalkRoute(out string route);
            return route;
        }

        static long WalkRoute(out string route)
        {
            var stepCount = 0;
            var startX = -100;
            var startY = 0;
            for (var i = 0; i < sWidth; ++i)
            {
                if (sMap[i, startY] == MAP_PATH_NS)
                {
                    startX = i;
                    break;
                }
            }
            if (startX == -100)
            {
                throw new InvalidProgramException($"Failed to find start position");
            }

            route = "";
            var x = startX;
            var y = startY;
            var currentPath = sMap[startX, startY];

            // Start heading down
            var dx = 0;
            var dy = 1;

            x += dx;
            y += dy;
            ++stepCount;

            while ((x >= 0) && (x < sWidth) && (y >= 0) && (y < sHeight))
            {
                var cell = sMap[x, y];
                if ((cell == MAP_PATH_NS) || (cell == MAP_PATH_EW))
                {
                }
                else if (cell == MAP_TURN)
                {
                    // left : dy = -dx, dx = +dy
                    var leftDX = +dy;
                    var leftDY = -dx;

                    var leftX = x + leftDX;
                    var leftY = y + leftDY;

                    var leftCell = MAP_EMPTY;
                    if ((leftX >= 0) && (leftX < sWidth) && (leftY >= 0) && (leftY < sHeight))
                    {
                        leftCell = sMap[leftX, leftY];
                    }

                    // right : dy = +dx, dx = -dy
                    var rightDX = -dy;
                    var rightDY = +dx;

                    var rightX = x + rightDX;
                    var rightY = y + rightDY;

                    var rightCell = MAP_EMPTY;
                    if ((rightX >= 0) && (rightX < sWidth) && (rightY >= 0) && (rightY < sHeight))
                    {
                        rightCell = sMap[rightX, rightY];
                    }

                    // Looking for opposite direction to the current one 
                    var nextPath = (currentPath == MAP_PATH_NS) ? MAP_PATH_EW : MAP_PATH_NS;
                    bool chooseLeft = false;
                    bool chooseRight = false;
                    if ((leftCell == nextPath) || (leftCell >= MAP_LETTER_START))
                    {
                        dx = leftDX;
                        dy = leftDY;
                        chooseLeft = true;
                    }
                    if ((rightCell == nextPath) || (rightCell >= MAP_LETTER_START))
                    {
                        dx = rightDX;
                        dy = rightDY;
                        chooseRight = true;
                    }

                    if (chooseLeft && chooseRight)
                    {
                        throw new InvalidProgramException($"Left and right are valid {x},{y}");
                    }

                    if (!chooseLeft && !chooseRight)
                    {
                        throw new InvalidProgramException($"Failed to choose left or right at {x},{y}");
                    }
                    currentPath = nextPath;
                }
                else if (cell >= MAP_LETTER_START)
                {
                    char c = (char)(cell - MAP_LETTER_START + 'A');
                    route += c;
                }
                else
                {
                    return stepCount;
                }
                x += dx;
                y += dy;
                ++stepCount;
            }

            return stepCount;
        }

        public static void Run()
        {
            Console.WriteLine("Day19 : Start");
            _ = new Program("Day19/input.txt", true);
            _ = new Program("Day19/input.txt", false);
            Console.WriteLine("Day19 : End");
        }
    }
}
