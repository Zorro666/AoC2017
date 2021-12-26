using System;

/*

--- Day 3: Spiral Memory ---

You come across an experimental new kind of memory stored on an infinite two-dimensional grid.

Each square on the grid is allocated in a spiral pattern starting at a location marked 1 and then counting up while spiraling outward. For example, the first few squares are allocated like this:

17  16  15  14  13
18   5   4   3  12
19   6   1   2  11
20   7   8   9  10
21  22  23---> ...
While this is very space-efficient (no squares are skipped), requested data must be carried back to square 1 (the location of the only access port for this memory system) by programs that can only move up, down, left, or right. They always take the shortest path: the Manhattan Distance between the location of the data and square 1.

For example:

Data from square 1 is carried 0 steps, since it's at the access port.
Data from square 12 is carried 3 steps, such as: down, left, left.
Data from square 23 is carried only 2 steps: up twice.
Data from square 1024 must be carried 31 steps.
How many steps are required to carry the data from the square identified in your puzzle input all the way to the access port?

Your puzzle input is 347991.

Your puzzle answer was 480.

--- Part Two ---

As a stress test on the system, the programs here clear the grid and then store the value 1 in square 1.
Then, in the same allocation order as shown above, they store the sum of the values in all adjacent squares, including diagonals.

So, the first few squares' values are chosen as follows:

Square 1 starts with the value 1.
Square 2 has only one adjacent filled square (with value 1), so it also stores 1.
Square 3 has both of the above squares as neighbors and stores the sum of their values, 2.
Square 4 has all three of the aforementioned squares as neighbors and stores the sum of their values, 4.
Square 5 only has the first and fourth squares as neighbors, so it gets the value 5.
Once a square is written, its value does not change. 
Therefore, the first few squares would receive the following values:

147  142  133  122   59
304    5    4    2   57
330   10    1    1   54
351   11   23   25   26
362  747  806--->   ...

What is the first value written that is larger than your puzzle input?

*/

namespace Day03
{
    class Program
    {
        const int MAX_DIMENSION = 512;
        const int MIN_X = -MAX_DIMENSION;
        const int MAX_X = MAX_DIMENSION;
        const int MIN_Y = -MAX_DIMENSION;
        const int MAX_Y = MAX_DIMENSION;
        const int ARRAY_SIZE_X = (MAX_X - MIN_X) + 1;
        const int ARRAY_SIZE_Y = (MAX_Y - MIN_Y) + 1;

        readonly static long[,] sCells = new long[ARRAY_SIZE_X, ARRAY_SIZE_Y];
        readonly static long[,] sTotalsGrid = new long[ARRAY_SIZE_X, ARRAY_SIZE_Y];
        readonly static long[] sTotals = new long[ARRAY_SIZE_X * ARRAY_SIZE_Y];

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            if (lines.Length != 1)
            {
                throw new InvalidProgramException($"Invalid input not 1 line long {lines.Length}");
            }
            var input = long.Parse(lines[0]);

            if (part1)
            {
                var result1 = SpiralSteps(input);
                Console.WriteLine($"Day03 : Result1 {result1}");
                var expected = 438;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = SpiralSteps2(input);
                Console.WriteLine($"Day03 : Result2 {result2}");
                var expected = 266330;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static long SpiralStepsRoot(long value)
        {
            // Find next odd number square larger than the vaue
            double floatSqRoot = Math.Sqrt(value);
            long intSqRoot = (long)floatSqRoot;
            if (intSqRoot * intSqRoot > value)
            {
                throw new InvalidProgramException($"Round-down Integer square root failed {intSqRoot * intSqRoot} > {value}");
            }
            if (intSqRoot * intSqRoot != value)
            {
                ++intSqRoot;
            }
            if (intSqRoot * intSqRoot < value)
            {
                throw new InvalidProgramException($"Next Integer square root failed {intSqRoot * intSqRoot} > {value}");
            }
            if ((intSqRoot % 2) == 0)
            {
                ++intSqRoot;
            }
            var startDx = (intSqRoot - 1) / 2;
            var startDy = (intSqRoot - 1) / 2;
            var dx = startDx;
            var dy = startDy;
            var spiralValue = intSqRoot * intSqRoot;
            while (spiralValue > value)
            {
                --spiralValue;
                // Go along the bottom edge
                if (dy == startDy)
                {
                    if ((dx > -startDx) && (dx <= startDx))
                    {
                        --dx;
                    }
                    else if (dx == -startDx)
                    {
                        // Start to go up the left edge
                        --dy;
                    }
                }
                // Go up the left edge
                else if (dx == -startDx)
                {
                    if ((dy > -startDy) && (dy <= startDy))
                    {
                        --dy;
                    }
                    else if (dy == -startDy)
                    {
                        // Start to go along the top edge
                        ++dx;
                    }
                }
                // Go along the top edge
                else if (dy == -startDy)
                {
                    if ((dx >= -startDx) && (dx < startDx))
                    {
                        ++dx;
                    }
                    else if (dx == startDx)
                    {
                        // Start to go down the right edge
                        ++dy;
                    }
                }
                // Go down the right edge
                else if (dx == startDx)
                {
                    if ((dy >= -startDy) && (dy < startDy))
                    {
                        ++dy;
                    }
                    else if (dy == startDy)
                    {
                        // Start to go along the bottom edge
                        --dx;
                    }
                }
                else
                {
                    throw new InvalidProgramException($"Unknown dx,dy position {dx} {dy} startDx {startDx} startDy {startDy}");
                }
            }
            return Math.Abs(dx) + Math.Abs(dy);
        }

        public static long SpiralSteps(long value)
        {
            for (var ix = 0; ix < ARRAY_SIZE_X; ++ix)
            {
                for (var iy = 0; iy < ARRAY_SIZE_Y; ++iy)
                {
                    sCells[ix, iy] = 0;
                    sTotalsGrid[ix, iy] = 0;
                }
            }
            for (var ix = 0; ix < sTotals.Length; ++ix)
            {
                sTotals[ix] = 0;
            }

            var x0 = MAX_DIMENSION;
            var y0 = MAX_DIMENSION;

            sCells[x0, y0] = 1;
            sTotalsGrid[x0, y0] = 1;
            sTotals[0] = 1;
            var dx = 0;
            var dy = 0;
            var loop = 0;
            var spiralValue = 1;
            var i = 1;
            while (spiralValue < value)
            {
                ++spiralValue;
                if ((dx == loop) && (dy == loop))
                {
                    ++loop;
                    ++dx;
                }
                else
                {
                    // Go up the right edge
                    if (dx == loop)
                    {
                        if ((dy > -loop) && (dy <= loop))
                        {
                            --dy;
                        }
                        else if (dy == -loop)
                        {
                            // Start to go left along the top edge
                            --dx;
                        }
                    }
                    // Go left along the top edge
                    else if (dy == -loop)
                    {
                        if ((dx > -loop) && (dx <= loop))
                        {
                            --dx;
                        }
                        else if (dx == -loop)
                        {
                            // Start to go down the left edge
                            ++dy;
                        }
                    }
                    // Go down the left edge
                    else if (dx == -loop)
                    {
                        if ((dy >= -loop) && (dy < loop))
                        {
                            ++dy;
                        }
                        else if (dy == loop)
                        {
                            // Start to go right along the bottom edge
                            ++dx;
                        }
                    }
                    // Go right along the bottom edge
                    else if (dy == loop)
                    {
                        if ((dx >= -loop) && (dx < loop))
                        {
                            ++dx;
                        }
                    }
                    else
                    {
                        throw new InvalidProgramException($"Unknown dx,dy position {dx} {dy} loop {loop}");
                    }
                }
                var x = x0 + dx;
                var y = y0 + dy;
                sCells[x, y] = spiralValue;
                //Console.WriteLine($"Spiral: {dx},{dy} = {spiralValue}");
                var totalValue = 0L;
                totalValue += sTotalsGrid[x - 1, y - 1];
                totalValue += sTotalsGrid[x + 0, y - 1];
                totalValue += sTotalsGrid[x + 1, y - 1];
                totalValue += sTotalsGrid[x - 1, y + 0];
                totalValue += sTotalsGrid[x + 1, y + 0];
                totalValue += sTotalsGrid[x - 1, y + 1];
                totalValue += sTotalsGrid[x + 0, y + 1];
                totalValue += sTotalsGrid[x + 1, y + 1];
                sTotalsGrid[x, y] = totalValue;
                sTotals[i] = totalValue;
                ++i;
                //Console.WriteLine($"Total: {dx},{dy} = {totalValue}");
            }
            return Math.Abs(dx) + Math.Abs(dy);
        }

        public static long SpiralSteps2(long value)
        {
            var maxSteps = MAX_DIMENSION / 8;
            for (var s = 32; s < maxSteps; ++s)
            {
                SpiralSteps(s);
                for (var i = 0; i < sTotals.Length; ++i)
                {
                    var total = sTotals[i];
                    if (total > value)
                    {
                        return total;
                    }
                }
            }
            throw new InvalidProgramException($"Failed to find total for {value}");
        }

        public static void Run()
        {
            Console.WriteLine("Day03 : Start");
            _ = new Program("Day03/input.txt", true);
            _ = new Program("Day03/input.txt", false);
            Console.WriteLine("Day03 : End");
        }
    }
}
