using System;

/*

--- Day 15: Dueling Generators ---

Here, you encounter a pair of dueling generators.
The generators, called generator A and generator B, are trying to agree on a sequence of numbers.
However, one of them is malfunctioning, and so the sequences don't always match.

As they do this, a judge waits for each of them to generate its next value, compares the lowest 16 bits of both values, and keeps track of the number of times those parts of the values match.

The generators both work on the same principle.
To create its next value, a generator will take the previous value it produced, multiply it by a factor (generator A uses 16807; generator B uses 48271), and then keep the remainder of dividing that resulting product by 2147483647.
That final remainder is the value it produces next.

To calculate each generator's first value, it instead uses a specific starting value as its "previous value" (as listed in your puzzle input).

For example, suppose that for starting values, generator A uses 65, while generator B uses 8921.
Then, the first five pairs of generated values are:

--Gen. A--  --Gen. B--
   1092455   430625591
1181022009  1233683848
 245556042  1431495498
1744312007   137874439
1352636452   285222916
In binary, these pairs are (with generator A's value first in each pair):

00000000000100001010101101100111
00011001101010101101001100110111

01000110011001001111011100111001
01001001100010001000010110001000

00001110101000101110001101001010
01010101010100101110001101001010

01100111111110000001011011000111
00001000001101111100110000000111

01010000100111111001100000100100
00010001000000000010100000000100
Here, you can see that the lowest (here, rightmost) 16 bits of the third value match: 1110001101001010.
Because of this one match, after processing these five pairs, the judge would have added only 1 to its total.

To get a significant sample, the judge would like to consider 40 million pairs.
(In the example above, the judge would eventually find a total of 588 pairs that match in their lowest 16 bits.)

After 40 million pairs, what is the judge's final count?

*/

namespace Day15
{
    class Program
    {
        static long aStart;
        static long bStart;

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);

            if (part1)
            {
                long result1 = CountMatches(aStart, bStart);
                Console.WriteLine($"Day15 : Result1 {result1}");
                long expected = 280;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                long result2 = -123;
                Console.WriteLine($"Day15 : Result2 {result2}");
                long expected = 1797;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        static void Parse(string[] lines)
        {
            if (lines.Length != 2)
            {
                throw new InvalidProgramException($"Invalid input expected 2 lines got {lines.Length}");
            }
            foreach (var line in lines)
            {
                var tokens = line.Split();
                // Generator A starts with 634
                // Generator B starts with 301
                bool goodLine = true;
                if (tokens.Length != 5)
                {
                    throw new InvalidProgramException($"Invalid line '{line}' expected 5 tokens got {tokens.Length}");
                }
                if (tokens[0] != "Generator")
                {
                    goodLine = false;
                }
                var value = long.Parse(tokens[4]);
                if (tokens[1] == "A")
                {
                    aStart = value;
                }
                else if (tokens[1] == "B")
                {
                    bStart = value;
                }
                else
                {
                    goodLine = false;
                }
                if (tokens[2] != "starts")
                {
                    goodLine = false;
                }
                if (tokens[3] != "with")
                {
                    goodLine = false;
                }

                if (goodLine == false)
                {
                    throw new InvalidProgramException($"Invalid line '{line}' expected format 'Generator <A/B> starts with <NUMBER>'");
                }
            }
        }

        public static long CountMatches(long aStart, long bStart)
        {
            return long.MinValue;
        }

        public static void Run()
        {
            Console.WriteLine("Day15 : Start");
            _ = new Program("Day15/input.txt", true);
            _ = new Program("Day15/input.txt", false);
            Console.WriteLine("Day15 : End");
        }
    }
}
