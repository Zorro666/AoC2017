using System;

/*

--- Day 13: Packet Scanners ---

You need to cross a vast firewall.
The firewall consists of several layers, each with a security scanner that moves back and forth across the layer.
To succeed, you must not be detected by a scanner.

By studying the firewall briefly, you are able to record (in your puzzle input) the depth of each layer and the range of the scanning area for the scanner within it, written as depth: range.
Each layer has a thickness of exactly 1.
A layer at depth 0 begins immediately inside the firewall; a layer at depth 1 would start immediately after that.

For example, suppose you've recorded the following:

0: 3
1: 2
4: 4
6: 4
This means that there is a layer immediately inside the firewall (with range 3), a second layer immediately after that (with range 2), a third layer which begins at depth 4 (with range 4), and a fourth layer which begins at depth 6 (also with range 4).
Visually, it might look like this:

 0   1   2   3   4   5   6
[ ] [ ] ... ... [ ] ... [ ]
[ ] [ ]         [ ]     [ ]
[ ]             [ ]     [ ]
                [ ]     [ ]
Within each layer, a security scanner moves back and forth within its range.
Each security scanner starts at the top and moves down until it reaches the bottom, then moves up until it reaches the top, and repeats.
A security scanner takes one picosecond to move one step.
Drawing scanners as S, the first few picoseconds look like this:


Picosecond 0:
 0   1   2   3   4   5   6
[S] [S] ... ... [S] ... [S]
[ ] [ ]         [ ]     [ ]
[ ]             [ ]     [ ]
                [ ]     [ ]

Picosecond 1:
 0   1   2   3   4   5   6
[ ] [ ] ... ... [ ] ... [ ]
[S] [S]         [S]     [S]
[ ]             [ ]     [ ]
                [ ]     [ ]

Picosecond 2:
 0   1   2   3   4   5   6
[ ] [S] ... ... [ ] ... [ ]
[ ] [ ]         [ ]     [ ]
[S]             [S]     [S]
                [ ]     [ ]

Picosecond 3:
 0   1   2   3   4   5   6
[ ] [ ] ... ... [ ] ... [ ]
[S] [S]         [ ]     [ ]
[ ]             [ ]     [ ]
                [S]     [S]
Your plan is to hitch a ride on a packet about to move through the firewall.
The packet will travel along the top of each layer, and it moves at one layer per picosecond.
Each picosecond, the packet moves one layer forward (its first move takes it into layer 0), and then the scanners move one step.
If there is a scanner at the top of the layer as your packet enters it, you are caught.
(If a scanner moves into the top of its layer while you are there, you are not caught: it doesn't have time to notice you before you leave.) If you were to do this in the configuration above, marking your current position with parentheses, your passage through the firewall would look like this:

Initial state:
 0   1   2   3   4   5   6
[S] [S] ... ... [S] ... [S]
[ ] [ ]         [ ]     [ ]
[ ]             [ ]     [ ]
                [ ]     [ ]

Picosecond 0:
 0   1   2   3   4   5   6
(S) [S] ... ... [S] ... [S]
[ ] [ ]         [ ]     [ ]
[ ]             [ ]     [ ]
                [ ]     [ ]

 0   1   2   3   4   5   6
( ) [ ] ... ... [ ] ... [ ]
[S] [S]         [S]     [S]
[ ]             [ ]     [ ]
                [ ]     [ ]


Picosecond 1:
 0   1   2   3   4   5   6
[ ] ( ) ... ... [ ] ... [ ]
[S] [S]         [S]     [S]
[ ]             [ ]     [ ]
                [ ]     [ ]

 0   1   2   3   4   5   6
[ ] (S) ... ... [ ] ... [ ]
[ ] [ ]         [ ]     [ ]
[S]             [S]     [S]
                [ ]     [ ]


Picosecond 2:
 0   1   2   3   4   5   6
[ ] [S] (.) ... [ ] ... [ ]
[ ] [ ]         [ ]     [ ]
[S]             [S]     [S]
                [ ]     [ ]

 0   1   2   3   4   5   6
[ ] [ ] (.) ... [ ] ... [ ]
[S] [S]         [ ]     [ ]
[ ]             [ ]     [ ]
                [S]     [S]


Picosecond 3:
 0   1   2   3   4   5   6
[ ] [ ] ... (.) [ ] ... [ ]
[S] [S]         [ ]     [ ]
[ ]             [ ]     [ ]
                [S]     [S]

 0   1   2   3   4   5   6
[S] [S] ... (.) [ ] ... [ ]
[ ] [ ]         [ ]     [ ]
[ ]             [S]     [S]
                [ ]     [ ]


Picosecond 4:
 0   1   2   3   4   5   6
[S] [S] ... ... ( ) ... [ ]
[ ] [ ]         [ ]     [ ]
[ ]             [S]     [S]
                [ ]     [ ]

 0   1   2   3   4   5   6
[ ] [ ] ... ... ( ) ... [ ]
[S] [S]         [S]     [S]
[ ]             [ ]     [ ]
                [ ]     [ ]


Picosecond 5:
 0   1   2   3   4   5   6
[ ] [ ] ... ... [ ] (.) [ ]
[S] [S]         [S]     [S]
[ ]             [ ]     [ ]
                [ ]     [ ]

 0   1   2   3   4   5   6
[ ] [S] ... ... [S] (.) [S]
[ ] [ ]         [ ]     [ ]
[S]             [ ]     [ ]
                [ ]     [ ]


Picosecond 6:
 0   1   2   3   4   5   6
[ ] [S] ... ... [S] ... (S)
[ ] [ ]         [ ]     [ ]
[S]             [ ]     [ ]
                [ ]     [ ]

 0   1   2   3   4   5   6
[ ] [ ] ... ... [ ] ... ( )
[S] [S]         [S]     [S]
[ ]             [ ]     [ ]
                [ ]     [ ]
In this situation, you are caught in layers 0 and 6, because your packet entered the layer when its scanner was at the top when you entered it.
You are not caught in layer 1, since the scanner moved into the top of the layer once you were already there.

The severity of getting caught on a layer is equal to its depth multiplied by its range.
(Ignore layers in which you do not get caught.) 
The severity of the whole trip is the sum of these values.
In the example above, the trip severity is 0*3 + 6*4 = 24.

Given the details of the firewall you've recorded, if you leave immediately, what is the severity of your whole trip?

*/

namespace Day13
{
    class Program
    {
        readonly static int MAX_NUM_LAYERS = 1024;
        readonly static int[] sLayerPositions = new int[MAX_NUM_LAYERS];
        readonly static int[] sLayerIDs = new int[MAX_NUM_LAYERS];
        readonly static int[] sLayerDepths = new int[MAX_NUM_LAYERS];
        readonly static int[] sLayerDirections = new int[MAX_NUM_LAYERS];
        static int sLayerCount;

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);
            Escape();

            if (part1)
            {
                var result1 = Severity;
                Console.WriteLine($"Day13 : Result1 {result1}");
                var expected = 1528;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = -123;
                Console.WriteLine($"Day13 : Result2 {result2}");
                var expected = 1797;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] layers)
        {
            sLayerCount = 0;
            if (layers.Length > MAX_NUM_LAYERS)
            {
                throw new InvalidProgramException($"Invalid input too many layers {layers.Length} MAX:{MAX_NUM_LAYERS}");
            }
            foreach (var layer in layers)
            {
                var tokens = layer.Trim().Split();
                if (tokens.Length != 2)
                {
                    throw new InvalidProgramException($"Invalid layer '{layer}' must have 2 tokens {tokens.Length}");
                }
                var idToken = tokens[0].Trim().Trim(':');
                var id = int.Parse(idToken);

                var depthToken = tokens[1];
                var depth = int.Parse(depthToken);

                sLayerIDs[sLayerCount] = id;
                sLayerDepths[sLayerCount] = depth;
                ++sLayerCount;
            }
        }

        static int FindLayerIndex(int id)
        {
            for (var i = 0; i < sLayerCount; ++i)
            {
                if (sLayerIDs[i] == id)
                {
                    return i;
                }
            }
            return -1;
        }

        public static void Escape()
        {
            var exitLayer = int.MinValue;
            for (var i = 0; i < sLayerCount; ++i)
            {
                sLayerPositions[i] = 0;
                sLayerDirections[i] = +1;
                exitLayer = Math.Max(sLayerIDs[i], exitLayer);
            }

            var severity = 0;
            var myLayerID = 0;
            while (myLayerID <= exitLayer)
            {
                var myIndex = FindLayerIndex(myLayerID);
                if (myIndex >= 0)
                {
                    if (sLayerPositions[myIndex] == 0)
                    {
                        severity += myLayerID * sLayerDepths[myIndex];
                    }
                }
                for (var i = 0; i < sLayerCount; ++i)
                {
                    sLayerPositions[i] = sLayerPositions[i] + sLayerDirections[i];
                    if (sLayerPositions[i] == 0)
                    {
                        sLayerDirections[i] = +1;
                    }
                    else if (sLayerPositions[i] == sLayerDepths[i] - 1)
                    {
                        sLayerDirections[i] = -1;
                    }
                }
                ++myLayerID;
            }
            Severity = severity;
        }

        public static int Severity { get; private set; }

        public static void Run()
        {
            Console.WriteLine("Day13 : Start");
            _ = new Program("Day13/input.txt", true);
            _ = new Program("Day13/input.txt", false);
            Console.WriteLine("Day13 : End");
        }
    }
}
