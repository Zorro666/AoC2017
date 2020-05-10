using System;

/*

--- Day 12: Digital Plumber ---

Walking along the memory banks of the stream, you find a small village that is experiencing a little confusion: some programs can't communicate with each other.

Programs in this village communicate using a fixed system of pipes.
Messages are passed between programs using these pipes, but most programs aren't connected to each other directly.
Instead, programs pass messages between each other until the message reaches the intended recipient.

For some reason, though, some of these messages aren't ever reaching their intended recipient, and the programs suspect that some pipes are missing.
They would like you to investigate.

You walk through the village and record the ID of each program and the IDs with which it can communicate directly (your puzzle input).
Each program has one or more programs with which it can communicate, and these pipes are bidirectional; if 8 says it can communicate with 11, then 11 will say it can communicate with 8.

You need to figure out how many programs are in the group that contains program ID 0.

For example, suppose you go door-to-door like a travelling salesman and record the following list:

0 <-> 2
1 <-> 1
2 <-> 0, 3, 4
3 <-> 2, 4
4 <-> 2, 3, 6
5 <-> 6
6 <-> 4, 5
In this example, the following programs are in the group that contains program ID 0:

Program 0 by definition.
Program 2, directly connected to program 0.
Program 3 via program 2.
Program 4 via program 2.
Program 5 via programs 6, then 4, then 2.
Program 6 via programs 4, then 2.
Therefore, a total of 6 programs are in this group; all but program 1, which has a pipe that connects it to itself.

How many programs are in the group that contains program ID 0?

Your puzzle answer was 169.

--- Part Two ---

There are more programs than just the ones in the group containing program ID 0. 
The rest of them have no way of reaching that group, and still might have no way of reaching each other.

A group is a collection of programs that can all communicate via pipes either directly or indirectly. 
The programs you identified just a moment ago are all part of the same group. 
Now, they would like you to determine the total number of groups.

In the example above, there were 2 groups: one consisting of programs 0,2,3,4,5,6, and the other consisting solely of program 1.

How many groups are there in total?

*/

namespace Day12
{
    class Program
    {
        readonly static int MAX_NUM_PROGRAMS = 2048;
        readonly static int MAX_NUM_CONNECTIONS = 128;
        readonly static int[,] sConnections = new int[MAX_NUM_PROGRAMS, MAX_NUM_CONNECTIONS];
        readonly static int[] sConnectionCount = new int[MAX_NUM_PROGRAMS];

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);

            if (part1)
            {
                var result1 = ProgramCount(0);
                Console.WriteLine($"Day12 : Result1 {result1}");
                var expected = 169;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = GroupCount();
                Console.WriteLine($"Day12 : Result2 {result2}");
                var expected = 179;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] connections)
        {
            if (connections.Length > MAX_NUM_PROGRAMS)
            {
                throw new InvalidProgramException($"Invalid input too many programs {connections.Length} MAX:{MAX_NUM_PROGRAMS}");
            }

            for (var c = 0; c < MAX_NUM_PROGRAMS; ++c)
            {
                sConnectionCount[c] = 0;
            }

            foreach (var line in connections)
            {
                var tokens = line.Trim().Split();
                if (tokens.Length < 3)
                {
                    throw new InvalidProgramException($"Invalid line '{line}' expected at least 3 tokens got {tokens.Length}");
                }

                var id = int.Parse(tokens[0].Trim());
                if (id >= MAX_NUM_PROGRAMS)
                {
                    throw new InvalidProgramException($"Invalid line '{line}' invalid ID {id} bigger than MAX {MAX_NUM_PROGRAMS - 1}");
                }

                if (tokens[1].Trim() != "<->")
                {
                    throw new InvalidProgramException($"Invalid line '{line}' invalid token {tokens[1]} expected '<->'");
                }

                if (sConnectionCount[id] != 0)
                {
                    throw new InvalidProgramException($"Invalid line '{line}' ID:{id} already has non-zero connections {sConnectionCount[id]}");
                }
                var connectionCount = tokens.Length - 2;
                if (connectionCount > MAX_NUM_CONNECTIONS)
                {
                    throw new InvalidProgramException($"Invalid line '{line}' ID:{id} too many connections {connectionCount} MAX:{MAX_NUM_CONNECTIONS}");
                }
                for (var i = 0; i < connectionCount; ++i)
                {
                    var otherEndToken = tokens[i + 2].Trim().Trim(',');
                    var otherEndID = int.Parse(otherEndToken);
                    if ((otherEndID < 0) || (otherEndID >= MAX_NUM_PROGRAMS))
                    {
                        throw new InvalidProgramException($"Invalid line '{line}' ID:{id} out of range connection {otherEndID}");
                    }
                    sConnections[id, i] = otherEndID;
                }
                sConnectionCount[id] = connectionCount;
            }
        }

        public static int GroupCount()
        {
            bool[] visited = new bool[MAX_NUM_PROGRAMS];
            var count = 0;
            for (var p = 0; p < MAX_NUM_PROGRAMS; ++p)
            {
                if (sConnectionCount[p] > 0)
                {
                    if (visited[p] == false)
                    {
                        ++count;
                        VisitChildren(p, ref visited);
                    }
                }
            }
            return count;
        }

        public static int ProgramCount(int programID)
        {
            bool[] visited = new bool[MAX_NUM_PROGRAMS];
            var count = 1 + VisitChildren(programID, ref visited);
            return count;
        }

        static int VisitChildren(int id, ref bool[] visited)
        {
            visited[id] = true;
            var count = 0;
            for (var c = 0; c < sConnectionCount[id]; ++c)
            {
                var child = sConnections[id, c];
                if (visited[child] == false)
                {
                    ++count;
                    count += VisitChildren(child, ref visited);
                }
            }
            return count;
        }

        public static void Run()
        {
            Console.WriteLine("Day12 : Start");
            _ = new Program("Day12/input.txt", true);
            _ = new Program("Day12/input.txt", false);
            Console.WriteLine("Day12 : End");
        }
    }
}
