using System;

/*

--- Day 20: Particle Swarm ---

Suddenly, the GPU contacts you, asking for help. Someone has asked it to simulate too many particles, and it won't be able to finish them all in time to render the next frame at this rate.

It transmits to you a buffer (your puzzle input) listing each particle in order (starting with particle 0, then particle 1, particle 2, and so on). For each particle, it provides the X, Y, and Z coordinates for the particle's position (p), velocity (v), and acceleration (a), each in the format <X,Y,Z>.

Each tick, all particles are updated simultaneously. A particle's properties are updated in the following order:

Increase the X velocity by the X acceleration.
Increase the Y velocity by the Y acceleration.
Increase the Z velocity by the Z acceleration.
Increase the X position by the X velocity.
Increase the Y position by the Y velocity.
Increase the Z position by the Z velocity.
Because of seemingly tenuous rationale involving z-buffering, the GPU would like to know which particle will stay closest to position <0,0,0> in the long term. Measure this using the Manhattan distance, which in this situation is simply the sum of the absolute values of a particle's X, Y, and Z position.

For example, suppose you are only given two particles, both of which stay entirely on the X-axis (for simplicity). Drawing the current states of particles 0 and 1 (in that order) with an adjacent a number line and diagram of current X positions (marked in parentheses), the following would take place:

p=< 3,0,0>, v=< 2,0,0>, a=<-1,0,0>    -4 -3 -2 -1  0  1  2  3  4
p=< 4,0,0>, v=< 0,0,0>, a=<-2,0,0>                         (0)(1)

p=< 4,0,0>, v=< 1,0,0>, a=<-1,0,0>    -4 -3 -2 -1  0  1  2  3  4
p=< 2,0,0>, v=<-2,0,0>, a=<-2,0,0>                      (1)   (0)

p=< 4,0,0>, v=< 0,0,0>, a=<-1,0,0>    -4 -3 -2 -1  0  1  2  3  4
p=<-2,0,0>, v=<-4,0,0>, a=<-2,0,0>          (1)               (0)

p=< 3,0,0>, v=<-1,0,0>, a=<-1,0,0>    -4 -3 -2 -1  0  1  2  3  4
p=<-8,0,0>, v=<-6,0,0>, a=<-2,0,0>                         (0)   
At this point, particle 1 will never be closer to <0,0,0> than particle 0, and so, in the long run, particle 0 will stay closest.

Which particle will stay closest to position <0,0,0> in the long term?

Your puzzle answer was 344.

The first half of this puzzle is complete! It provides one gold star: *

--- Part Two ---

To simplify the problem further, the GPU would like to remove any particles that collide. 
Particles collide if their positions ever exactly match. 
Because particles are updated simultaneously, more than two particles can collide at the same time and place. 
Once particles collide, they are removed and cannot collide with anything else after that tick.

For example:

p=<-6,0,0>, v=< 3,0,0>, a=< 0,0,0>    
p=<-4,0,0>, v=< 2,0,0>, a=< 0,0,0>    -6 -5 -4 -3 -2 -1  0  1  2  3
p=<-2,0,0>, v=< 1,0,0>, a=< 0,0,0>    (0)   (1)   (2)            (3)
p=< 3,0,0>, v=<-1,0,0>, a=< 0,0,0>

p=<-3,0,0>, v=< 3,0,0>, a=< 0,0,0>    
p=<-2,0,0>, v=< 2,0,0>, a=< 0,0,0>    -6 -5 -4 -3 -2 -1  0  1  2  3
p=<-1,0,0>, v=< 1,0,0>, a=< 0,0,0>             (0)(1)(2)      (3)   
p=< 2,0,0>, v=<-1,0,0>, a=< 0,0,0>

p=< 0,0,0>, v=< 3,0,0>, a=< 0,0,0>    
p=< 0,0,0>, v=< 2,0,0>, a=< 0,0,0>    -6 -5 -4 -3 -2 -1  0  1  2  3
p=< 0,0,0>, v=< 1,0,0>, a=< 0,0,0>                       X (3)      
p=< 1,0,0>, v=<-1,0,0>, a=< 0,0,0>

------destroyed by collision------    
------destroyed by collision------    -6 -5 -4 -3 -2 -1  0  1  2  3
------destroyed by collision------                      (3)         
p=< 0,0,0>, v=<-1,0,0>, a=< 0,0,0>

In this example, particles 0, 1, and 2 are simultaneously destroyed at the time and place marked X. 
On the next tick, particle 3 passes through unharmed.

How many particles are left after all collisions are resolved?

*/

namespace Day20
{
    class Program
    {
        readonly static int MAX_NUM_PARTICLES = 1024 * 1024;
        readonly static long[] sPositionsX = new long[MAX_NUM_PARTICLES];
        readonly static long[] sPositionsY = new long[MAX_NUM_PARTICLES];
        readonly static long[] sPositionsZ = new long[MAX_NUM_PARTICLES];
        readonly static long[] sVelocitysX = new long[MAX_NUM_PARTICLES];
        readonly static long[] sVelocitysY = new long[MAX_NUM_PARTICLES];
        readonly static long[] sVelocitysZ = new long[MAX_NUM_PARTICLES];
        readonly static long[] sAccelerationsX = new long[MAX_NUM_PARTICLES];
        readonly static long[] sAccelerationsY = new long[MAX_NUM_PARTICLES];
        readonly static long[] sAccelerationsZ = new long[MAX_NUM_PARTICLES];
        readonly static long[] sDistances = new long[MAX_NUM_PARTICLES];
        readonly static bool[] sActives = new bool[MAX_NUM_PARTICLES];
        static int sParticleCount;

        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);
            Parse(lines);

            if (part1)
            {
                var result1 = ClosestParticle();
                Console.WriteLine($"Day20 : Result1 {result1}");
                var expected = 161;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = RemainingParticles();
                Console.WriteLine($"Day20 : Result2 {result2}");
                var expected = 438;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        public static void Parse(string[] inputs)
        {
            sParticleCount = 0;
            foreach (var line in inputs)
            {
                //p=< 3,0,0>, v=< 2,0,0>, a=<-1,0,0>
                var tokens = line.Trim().Split(">, ");
                if (tokens.Length != 3)
                {
                    throw new InvalidProgramException($"Invalid line '{line}' invalid tokens expected 3 got {tokens.Length}");
                }
                var posTokens = tokens[0].Trim().Split('=');
                if (posTokens.Length != 2)
                {
                    throw new InvalidProgramException($"Invalid line '{line}' invalid position token expected 2 got {posTokens.Length}");
                }
                if (posTokens[0].Trim() != "p")
                {
                    throw new InvalidProgramException($"Invalid line '{line}' bad position token '{posTokens[0]}' expected 'p'");
                }
                var posValueTokens = posTokens[1].Trim().Split(',');
                if (posValueTokens.Length != 3)
                {
                    throw new InvalidProgramException($"Invalid line '{line}' bad position values token '{posTokens[1]}' expected 3 got {posValueTokens.Length}");
                }
                var posX = int.Parse(posValueTokens[0].Trim().Trim('<'));
                var posY = int.Parse(posValueTokens[1]);
                var posZ = int.Parse(posValueTokens[2]);

                var velTokens = tokens[1].Trim().Split('=');
                if (velTokens.Length != 2)
                {
                    throw new InvalidProgramException($"Invalid line '{line}' invalid velocity token expected 2 got {velTokens.Length}");
                }
                if (velTokens[0].Trim() != "v")
                {
                    throw new InvalidProgramException($"Invalid line '{line}' bad velocity token '{velTokens[0]}' expected 'v'");
                }
                var velValueTokens = velTokens[1].Trim().Split(',');
                if (velValueTokens.Length != 3)
                {
                    throw new InvalidProgramException($"Invalid line '{line}' bad velocity values token '{velTokens[1]}' expected 3 got {velValueTokens.Length}");
                }
                var velX = int.Parse(velValueTokens[0].Trim().Trim('<'));
                var velY = int.Parse(velValueTokens[1]);
                var velZ = int.Parse(velValueTokens[2]);

                var accTokens = tokens[2].Trim().Split('=');
                if (accTokens.Length != 2)
                {
                    throw new InvalidProgramException($"Invalid line '{line}' invalid acceleration token expected 2 got {accTokens.Length}");
                }
                if (accTokens[0].Trim() != "a")
                {
                    throw new InvalidProgramException($"Invalid line '{line}' bad acceleration token '{accTokens[0]}' expected 'a'");
                }
                var accValueTokens = accTokens[1].Trim().Split(',');
                if (accValueTokens.Length != 3)
                {
                    throw new InvalidProgramException($"Invalid line '{line}' bad acceleration values token '{accTokens[1]}' expected 3 got {accValueTokens.Length}");
                }
                var accX = int.Parse(accValueTokens[0].Trim().Trim('<'));
                var accY = int.Parse(accValueTokens[1]);
                var accZ = int.Parse(accValueTokens[2].Trim('>'));

                sPositionsX[sParticleCount] = posX;
                sPositionsY[sParticleCount] = posY;
                sPositionsZ[sParticleCount] = posZ;

                sVelocitysX[sParticleCount] = velX;
                sVelocitysY[sParticleCount] = velY;
                sVelocitysZ[sParticleCount] = velZ;

                sAccelerationsX[sParticleCount] = accX;
                sAccelerationsY[sParticleCount] = accY;
                sAccelerationsZ[sParticleCount] = accZ;

                sActives[sParticleCount] = true;
                ++sParticleCount;
            }

            ComputeDistances();
            //OutputParticles();
        }

        static void OutputParticles()
        {
            for (var i = 0; i < sParticleCount; ++i)
            {
                Console.Write($"Particle[{sParticleCount}]");
                Console.Write($" Pos: {sPositionsX[i]},{sPositionsY[i]},{sPositionsZ[i]}");
                Console.Write($" Vel: {sVelocitysX[i]},{sVelocitysY[i]},{sVelocitysZ[i]}");
                Console.Write($" Acc: {sAccelerationsX[i]},{sAccelerationsY[i]},{sAccelerationsZ[i]}");
                Console.Write($" Distance: {sDistances[i]}");
                Console.WriteLine();
            }
        }

        static void ComputeDistances()
        {
            for (var i = 0; i < sParticleCount; ++i)
            {
                var distance = 0L;
                distance += Math.Abs(sPositionsX[i]);
                distance += Math.Abs(sPositionsY[i]);
                distance += Math.Abs(sPositionsZ[i]);
                sDistances[i] = distance;
            }
        }

        public static int SingleStep()
        {
            var closestDistance = long.MaxValue;
            var closestParticle = -1;

            for (var i = 0; i < sParticleCount; ++i)
            {
                if (!sActives[i])
                {
                    continue;
                }
                sVelocitysX[i] += sAccelerationsX[i];
                sVelocitysY[i] += sAccelerationsY[i];
                sVelocitysZ[i] += sAccelerationsZ[i];

                sPositionsX[i] += sVelocitysX[i];
                sPositionsY[i] += sVelocitysY[i];
                sPositionsZ[i] += sVelocitysZ[i];

                var distance = 0L;
                distance += Math.Abs(sPositionsX[i]);
                distance += Math.Abs(sPositionsY[i]);
                distance += Math.Abs(sPositionsZ[i]);
                sDistances[i] = distance;

                if (sDistances[i] < closestDistance)
                {
                    closestDistance = sDistances[i];
                    closestParticle = i;
                }
            }
            return closestParticle;
        }

        public static int ClosestParticle()
        {
            int previousClosestParticle = -1;
            int newClosestParticle;

            long countSame = 0L;
            do
            {
                newClosestParticle = SingleStep();
                if (newClosestParticle != previousClosestParticle)
                {
                    countSame = 0;
                    previousClosestParticle = newClosestParticle;
                }
                ++countSame;
            } while (countSame < (1L << 10));

            return newClosestParticle;
        }

        static int ProcessCollisions()
        {
            bool[] inactiveParticles = new bool[sParticleCount];
            for (var i = 0; i < sParticleCount - 1; ++i)
            {
                if (!sActives[i])
                {
                    continue;
                }
                for (var j = i + 1; j < sParticleCount; ++j)
                {
                    if (!sActives[j])
                    {
                        continue;
                    }
                    if (sPositionsX[j] != sPositionsX[i])
                    {
                        continue;
                    }
                    if (sPositionsY[j] != sPositionsY[i])
                    {
                        continue;
                    }
                    if (sPositionsZ[j] != sPositionsZ[i])
                    {
                        continue;
                    }
                    inactiveParticles[i] = true;
                    inactiveParticles[j] = true;
                }
            }
            int activeParticleCount = 0;
            for (var i = 0; i < sParticleCount; ++i)
            {
                if (inactiveParticles[i])
                {
                    sActives[i] = false;
                }
                if (!sActives[i])
                {
                    continue;
                }
                ++activeParticleCount;
            }
            return activeParticleCount;
        }

        public static int RemainingParticles()
        {
            int previousActiveParticles = -1;
            int newActivePaticles;

            long countSame = 0L;
            do
            {
                SingleStep();
                newActivePaticles = ProcessCollisions();
                if (newActivePaticles != previousActiveParticles)
                {
                    countSame = 0;
                    previousActiveParticles = newActivePaticles;
                }
                ++countSame;
            } while (countSame < (1L << 10));

            return newActivePaticles;
        }

        public static void Run()
        {
            Console.WriteLine("Day20 : Start");
            _ = new Program("Day20/input.txt", true);
            _ = new Program("Day20/input.txt", false);
            Console.WriteLine("Day20 : End");
        }
    }
}
