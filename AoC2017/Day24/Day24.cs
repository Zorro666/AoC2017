using System;

/*

--- Day 24: Electromagnetic Moat ---

The CPU itself is a large, black building surrounded by a bottomless pit.
Enormous metal tubes extend outward from the side of the building at regular intervals and descend down into the void.
There's no way to cross, but you need to get inside.

No way, of course, other than building a bridge out of the magnetic components strewn about nearby.

Each component has two ports, one on each end.
The ports come in all different types, and only matching types can be connected.
You take an inventory of the components by their port types (your puzzle input).
Each port is identified by the number of pins it uses; more pins mean a stronger connection for your bridge.
A 3/7 component, for example, has a type-3 port on one side, and a type-7 port on the other.

Your side of the pit is metallic; a perfect surface to connect a magnetic, zero-pin port.
Because of this, the first port you use must be of type 0.
It doesn't matter what type of port you end with; your goal is just to make the bridge as strong as possible.

The strength of a bridge is the sum of the port types in each component.
For example, if your bridge is made of components 0/3, 3/7, and 7/4, your bridge has a strength of 0+3 + 3+7 + 7+4 = 24.

For example, suppose you had the following components:

0/2
2/2
2/3
3/4
3/5
0/1
10/1
9/10
With them, you could make the following valid bridges:

0/1
0/1--10/1
0/1--10/1--9/10
0/2
0/2--2/3
0/2--2/3--3/4
0/2--2/3--3/5
0/2--2/2
0/2--2/2--2/3
0/2--2/2--2/3--3/4
0/2--2/2--2/3--3/5
(Note how, as shown by 10/1, order of ports within a component doesn't matter.
However, you may only use each port on a component once.)

Of these bridges, the strongest one is 0/1--10/1--9/10; it has a strength of 0+1 + 1+10 + 10+9 = 31.

What is the strength of the strongest bridge you can make with the components you have available?

Your puzzle answer was 1511.

--- Part Two ---

The bridge you've built isn't long enough; you can't jump the rest of the way.

In the example above, there are two longest bridges:

0/2--2/2--2/3--3/4
0/2--2/2--2/3--3/5

Of them, the one which uses the 3/5 component is stronger; its strength is 0+2 + 2+2 + 2+3 + 3+5 = 19.

What is the strength of the longest bridge you can make? 
If you can make multiple bridges of the longest length, pick the strongest one.

*/

namespace Day24
{
    class Program
    {
        const int MAX_NUM_PIECES = 1024;
        static int sNumPieces;
        readonly static int[] sPiecesEndA = new int[MAX_NUM_PIECES];
        readonly static int[] sPiecesEndB = new int[MAX_NUM_PIECES];
        readonly static int[] sPiecesStrength = new int[MAX_NUM_PIECES];
        readonly static bool[] sUsedPieces = new bool[MAX_NUM_PIECES];
        static int sMaxBridgeStrength;
        static int sMaxBridgeLength;

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);

            if (part1)
            {
                var result1 = StrongestBridge();
                Console.WriteLine($"Day24 : Result1 {result1}");
                var expected = 1511;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = StrongestLongestBridge();
                Console.WriteLine($"Day24 : Result2 {result2}");
                var expected = 1471;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] lines)
        {
            if (lines.Length < 1)
            {
                throw new InvalidProgramException($"Invalid input need at least one piece");
            }
            sNumPieces = 0;
            foreach (var line in lines)
            {
                var tokens = line.Trim().Split('/');
                if (tokens.Length != 2)
                {
                    throw new InvalidProgramException($"Invalid line '{line}' must have only one '/'");
                }
                var link1 = int.Parse(tokens[0]);
                var link2 = int.Parse(tokens[1]);
                if (link1 < link2)
                {
                    sPiecesEndA[sNumPieces] = link1;
                    sPiecesEndB[sNumPieces] = link2;
                }
                else
                {
                    sPiecesEndA[sNumPieces] = link2;
                    sPiecesEndB[sNumPieces] = link1;
                }
                sPiecesStrength[sNumPieces] = link1 + link2;
                ++sNumPieces;
            }
            for (var i = 0; i < sNumPieces - 1; ++i)
            {
                for (var j = i + 1; j < sNumPieces - 1; ++j)
                {
                    var pieceEndAI = sPiecesEndA[i];
                    var pieceEndAJ = sPiecesEndA[j];
                    if (pieceEndAJ < pieceEndAI)
                    {
                        sPiecesEndA[i] = pieceEndAJ;
                        sPiecesEndA[j] = pieceEndAI;
                        var temp = sPiecesEndB[i];
                        sPiecesEndB[i] = sPiecesEndB[j];
                        sPiecesEndB[j] = temp;

                        temp = sPiecesStrength[i];
                        sPiecesStrength[i] = sPiecesStrength[j];
                        sPiecesStrength[j] = temp;
                    }
                }
            }
        }

        static void FindBridge(int wantedEdge, int bridgeStrength, int bridgeLength, bool findLongestBridge)
        {
            for (int i = 0; i < sNumPieces; ++i)
            {
                if (sUsedPieces[i])
                {
                    continue;
                }
                int nextPiece = -1;
                if (sPiecesEndA[i] == wantedEdge)
                {
                    nextPiece = i;
                }
                if (sPiecesEndB[i] == wantedEdge)
                {
                    nextPiece = i;
                }
                if (nextPiece == -1)
                {
                    continue;
                }
                var pieceStrength = sPiecesStrength[nextPiece];
                var oldWantedEdge = wantedEdge;
                bridgeLength += 1;
                bridgeStrength += pieceStrength;
                sUsedPieces[nextPiece] = true;
                //Console.WriteLine($"{i} {nextPiece} {sPiecesEndA[nextPiece]}/{sPiecesEndB[nextPiece]} {bridgeStrength}");
                if (!findLongestBridge)
                {
                    if (bridgeStrength > sMaxBridgeStrength)
                    {
                        //Console.WriteLine($"New Maximum {bridgeStrength} {sMaxBridgeStrength}");
                        sMaxBridgeStrength = bridgeStrength;
                    }
                }
                if (bridgeLength > sMaxBridgeLength)
                {
                    if (findLongestBridge)
                    {
                        sMaxBridgeStrength = int.MinValue;
                    }
                    sMaxBridgeLength = bridgeLength;
                }
                if (findLongestBridge)
                {
                    if (bridgeLength >= sMaxBridgeLength)
                    {
                        if (bridgeStrength > sMaxBridgeStrength)
                        {
                            //Console.WriteLine($"New Maximum {bridgeStrength} {sMaxBridgeStrength}");
                            sMaxBridgeStrength = bridgeStrength;
                        }
                    }
                }

                if (sPiecesEndA[nextPiece] == wantedEdge)
                {
                    wantedEdge = sPiecesEndB[nextPiece];
                }
                else if (sPiecesEndB[nextPiece] == wantedEdge)
                {
                    wantedEdge = sPiecesEndA[nextPiece];
                }
                else
                {
                    throw new InvalidProgramException($"Neither edge matches from nextPiece {nextPiece} wanted:{wantedEdge} A:{sPiecesEndA[nextPiece]} B:{sPiecesEndB[nextPiece]}");
                }

                FindBridge(wantedEdge, bridgeStrength, bridgeLength, findLongestBridge);
                bridgeLength -= 1;
                bridgeStrength -= pieceStrength;
                sUsedPieces[nextPiece] = false;
                wantedEdge = oldWantedEdge;
            }
        }

        public static int StrongestLongestBridge()
        {
            sMaxBridgeStrength = int.MinValue;
            for (var i = 0; i < sNumPieces; ++i)
            {
                sUsedPieces[i] = false;
            }
            FindBridge(0, 0, 0, true);
            return sMaxBridgeStrength;
        }

        public static int StrongestBridge()
        {
            sMaxBridgeStrength = int.MinValue;
            for (var i = 0; i < sNumPieces; ++i)
            {
                sUsedPieces[i] = false;
            }
            FindBridge(0, 0, 0, false);
            return sMaxBridgeStrength;
        }

        public static void Run()
        {
            Console.WriteLine("Day24 : Start");
            _ = new Program("Day24/input.txt", true);
            _ = new Program("Day24/input.txt", false);
            Console.WriteLine("Day24 : End");
        }
    }
}
