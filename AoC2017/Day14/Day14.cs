using System;

/*

--- Day 14: Disk Defragmentation ---

Suddenly, a scheduled job activates the system's disk defragmenter.
Were the situation different, you might sit and watch it for a while, but today, you just don't have that kind of time.
It's soaking up valuable system resources that are needed elsewhere, and so the only option is to help it finish its task as soon as possible.

The disk in question consists of a 128x128 grid; each square of the grid is either free or used.
On this disk, the state of the grid is tracked by the bits in a sequence of knot hashes.

A total of 128 knot hashes are calculated, each corresponding to a single row in the grid; each hash contains 128 bits which correspond to individual grid squares.
Each bit of a hash indicates whether that square is free (0) or used (1).

The hash inputs are a key string (your puzzle input), a dash, and a number from 0 to 127 corresponding to the row.
For example, if your key string were flqrgnkx, then the first row would be given by the bits of the knot hash of flqrgnkx-0, the second row from the bits of the knot hash of flqrgnkx-1, and so on until the last row, flqrgnkx-127.

The output of a knot hash is traditionally represented by 32 hexadecimal digits; each of these digits correspond to 4 bits, for a total of 4 * 32 = 128 bits.
To convert to bits, turn each hexadecimal digit to its equivalent binary value, high-bit first: 0 becomes 0000, 1 becomes 0001, e becomes 1110, f becomes 1111, and so on; a hash that begins with a0c2017... in hexadecimal would begin with 10100000110000100000000101110000... in binary.

Continuing this process, the first 8 rows and columns for key flqrgnkx appear as follows, using # to denote used squares, and . to denote free ones:

##.#.#..-->
.#.#.#.#   
....#.#.   
#.#.##.#   
.##.#...   
##..#..#   
.#...#..   
##.#.##.-->
|      |   
V      V   
In this example, 8108 squares are used across the entire 128x128 grid.

Given your actual key string, how many squares are used?

Your puzzle input is amgozmfv.

Your puzzle answer was 8222.

--- Part Two ---

Now, all the defragmenter needs to know is the number of regions.
A region is a group of used squares that are all adjacent, not including diagonals.
Every used square is in exactly one region: lone used squares form their own isolated regions, while several adjacent squares all count as a single region.

In the example above, the following nine regions are visible, each marked with a distinct digit:

11.2.3..-->
.1.2.3.4   
....5.6.   
7.8.55.9   
.88.5...   
88..5..8   
.8...8..   
88.8.88.-->
|      |   
V      V   
Of particular interest is the region marked 8; while it does not appear contiguous in this small view, all of the squares marked 8 are connected when considering the whole 128x128 grid.
In total, in this example, 1242 regions are present.

How many regions are present given your key string?

*/

namespace Day14
{
    class Program
    {
        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            if (lines.Length != 1)
            {
                throw new InvalidProgramException($"Invalid input expected a single line {lines.Length}");
            }
            var hash = lines[0].Trim();
            if (part1)
            {
                var result1 = UsedSquares(hash);
                Console.WriteLine($"Day14 : Result1 {result1}");
                var expected = 8214;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = CountRegions(hash);
                Console.WriteLine($"Day14 : Result2 {result2}");
                var expected = 1093;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        static byte[] KnotHash(string input)
        {
            var trimmed = input.Trim().ToCharArray();
            var trimmedLen = trimmed.Length;
            byte[] lengths = new byte[trimmedLen + 5];
            for (var c = 0; c < trimmedLen; ++c)
            {
                lengths[c] = (byte)trimmed[c];
            }
            lengths[trimmedLen + 0] = 17;
            lengths[trimmedLen + 1] = 31;
            lengths[trimmedLen + 2] = 73;
            lengths[trimmedLen + 3] = 47;
            lengths[trimmedLen + 4] = 23;

            const int SEQUENCE_SIZE = 256;
            byte[] values = new byte[SEQUENCE_SIZE];
            byte[] temp = new byte[SEQUENCE_SIZE];
            for (var i = 0; i < 256; ++i)
            {
                values[i] = (byte)i;
            }
            var skip = 0;
            var pc = 0;
            for (var r = 0; r < 64; ++r)
            {
                foreach (var length in lengths)
                {
                    pc %= SEQUENCE_SIZE;
                    for (var i = 0; i < length; ++i)
                    {
                        var index = (i + pc) % SEQUENCE_SIZE;
                        temp[index] = values[index];
                    }
                    for (var i = 0; i < length; ++i)
                    {
                        var index = (i + pc) % SEQUENCE_SIZE;
                        var reverseIndex = (pc + length - 1 - i) % SEQUENCE_SIZE;
                        values[index] = temp[reverseIndex];
                    }
                    pc += length;
                    pc += skip;
                    ++skip;
                }
            }

            byte[] sparseHash = new byte[16];
            for (var b = 0; b < 16; ++b)
            {
                byte v = 0;
                for (var j = 0; j < 16; ++j)
                {
                    v ^= values[j + b * 16];
                }
                sparseHash[b] = v;
            }

            byte[] bytes = new byte[32];
            for (var ic = 0; ic < 16; ++ic)
            {
                var v = sparseHash[ic];
                bytes[ic * 2 + 0] = (byte)((v >> 4) & 0xF);
                bytes[ic * 2 + 1] = (byte)((v >> 0) & 0xF);
            }
            return bytes;
        }

        static byte[] GetRowHashBytes(string hash, int row)
        {
            var rowHash = $"{hash}-{row}";
            var rowKnotHashBytes = KnotHash(rowHash);
            return rowKnotHashBytes;
        }

        public static long UsedSquares(string hash)
        {
            var usedBits = 0;
            for (var r = 0; r < 128; ++r)
            {
                var rowKnotHashBytes = GetRowHashBytes(hash, r);
                for (var b = 0; b < 32; ++b)
                {
                    var byteValue = (byte)rowKnotHashBytes[b];
                    while (byteValue != 0)
                    {
                        usedBits += byteValue & 0x1;
                        byteValue >>= 1;
                    }
                }
            }
            return usedBits;
        }

        public static long CountRegions(string hash)
        {
            var cells = new bool[128, 128];
            for (var y = 0; y < 128; ++y)
            {
                var rowKnotHashBytes = GetRowHashBytes(hash, y);
                for (var b = 0; b < 32; ++b)
                {
                    var x = b * 4 + 3;
                    var byteValue = (byte)rowKnotHashBytes[b];
                    while (byteValue != 0)
                    {
                        if ((byteValue & 0x1) == 0x1)
                        {
                            cells[x, y] = true;
                        }
                        byteValue >>= 1;
                        --x;
                    }
                }
            }
            var regionCount = 0;
            // Count regions
            for (var y = 0; y < 128; ++y)
            {
                for (var x = 0; x < 128; ++x)
                {
                    if (cells[x, y])
                    {
                        ++regionCount;
                        FillRegion(x, y, ref cells);
                    }
                }
            }
            return regionCount;
        }

        static void FillRegion(int x, int y, ref bool[,] cells)
        {
            if (x < 0)
            {
                return;
            }
            if (x >= 128)
            {
                return;
            }
            if (y < 0)
            {
                return;
            }
            if (y >= 128)
            {
                return;
            }

            if (cells[x, y] == false)
            {
                return;
            }
            cells[x, y] = false;
            FillRegion(x + 0, y - 1, ref cells);
            FillRegion(x - 1, y + 0, ref cells);
            FillRegion(x + 1, y + 0, ref cells);
            FillRegion(x + 0, y + 1, ref cells);
        }

        public static void Run()
        {
            Console.WriteLine("Day14 : Start");
            _ = new Program("Day14/input.txt", true);
            _ = new Program("Day14/input.txt", false);
            Console.WriteLine("Day14 : End");
        }
    }
}
