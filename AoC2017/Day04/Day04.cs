using System;
using System.Collections.Generic;

/*

--- Day 4: High-Entropy Passphrases ---

A new system policy has been put in place that requires all accounts to use a passphrase instead of simply a password. 
A passphrase consists of a series of words (lowercase letters) separated by spaces.

To ensure security, a valid passphrase must contain no duplicate words.

For example:

aa bb cc dd ee is valid.
aa bb cc dd aa is not valid - the word aa appears more than once.
aa bb cc dd aaa is valid - aa and aaa count as different words.
The system's full passphrase list is available as your puzzle input. How many passphrases are valid?

Your puzzle answer was 325.

--- Part Two ---

For added security, yet another system policy has been put in place. 
Now, a valid passphrase must contain no two words that are anagrams of each other - that is, a passphrase is invalid if any word's letters can be rearranged to form any other word in the passphrase.

For example:

abcde fghij is a valid passphrase.
abcde xyz ecdab is not valid - the letters from the third word can be rearranged to form the first word.
a ab abc abd abf abj is a valid passphrase, because all letters need to be used when forming another word.
iiii oiii ooii oooi oooo is valid.
oiii ioii iioi iiio is not valid - any of these words can be rearranged to form any other word.
Under this new system policy, how many passphrases are valid?

*/

namespace Day04
{
    class Program
    {
        private Program(string inputFile, bool part1)
        {
            var lines = AoC.Program.ReadLines(inputFile);

            if (part1)
            {
                var result1 = CountValidPassphrase(lines);
                Console.WriteLine($"Day04 : Result1 {result1}");
                var expected = 325;
                if (result1 != expected)
                {
                    throw new InvalidProgramException($"Part1 is broken {result1} != {expected}");
                }
            }
            else
            {
                var result2 = CountValidPassphrase2(lines);
                Console.WriteLine($"Day04 : Result2 {result2}");
                var expected = 119;
                if (result2 != expected)
                {
                    throw new InvalidProgramException($"Part2 is broken {result2} != {expected}");
                }
            }
        }

        static bool ValidPassphrase(string passphrase)
        {
            var words = passphrase.Trim().Split(new string[0], StringSplitOptions.RemoveEmptyEntries);
            List<string> knownWords = new List<string>(words.Length);
            foreach (var word in words)
            {
                if (knownWords.Contains(word))
                {
                    return false;
                }
                knownWords.Add(word);
                //Console.WriteLine($"{word}");
            }
            return true;
        }

        public static long CountValidPassphrase(string[] input)
        {
            var count = 0;
            foreach (var passphrase in input)
            {
                if (ValidPassphrase(passphrase))
                {
                    ++count;
                }
            }
            return count;
        }

        static bool ValidPassphrase2(string passphrase)
        {
            var words = passphrase.Trim().Split(new string[0], StringSplitOptions.RemoveEmptyEntries);
            List<string> knownWords = new List<string>(words.Length);
            foreach (var word in words)
            {
                var chars = word.ToCharArray();
                for (var i = 0; i < chars.Length - 1; ++i)
                {
                    for (var j = i + 1; j < chars.Length; ++j)
                    {
                        var ii = chars[i];
                        var ij = chars[j];
                        if (ij < ii)
                        {
                            chars[i] = ij;
                            chars[j] = ii;
                        }
                    }
                }
                var sorted = new string(chars);
                if (knownWords.Contains(sorted))
                {
                    return false;
                }
                knownWords.Add(sorted);
            }
            return true;
        }

        public static long CountValidPassphrase2(string[] input)
        {
            var count = 0;
            foreach (var passphrase in input)
            {
                if (ValidPassphrase2(passphrase))
                {
                    ++count;
                }
            }
            return count;
        }

        public static void Run()
        {
            Console.WriteLine("Day04 : Start");
            _ = new Program("Day04/input.txt", true);
            _ = new Program("Day04/input.txt", false);
            Console.WriteLine("Day04 : End");
        }
    }
}
