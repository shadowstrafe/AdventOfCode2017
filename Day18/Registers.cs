﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Day18
{
    class Registers
    {
        Dictionary<char, long> registers = new Dictionary<char, long>();

        List<Func<long?>> instructions = new List<Func<long?>>();

        private const int initialValue = 0;
        private long lastPlayed = 0;
        private int currentLine = 0;

        private static readonly Regex regex = new Regex(@"^([a-z]{3}) (-?[a-z0-9]+) ?(-?[a-z0-9]+)?$");
        private static readonly Regex isChar = new Regex(@"[a-z]");

        public void Read(string[] input)
        {
            foreach (var line in input)
            {
                var match = regex.Match(line);

                string opr = match.Groups[1].Value;

                string x = match.Groups[2].Value;

                bool xIsChar = false;
                int xNum = 0;
                char xChar = ' ';

                if (isChar.IsMatch(x))
                {
                    xIsChar = true;
                    xChar = x[0];
                }
                else
                {
                    xIsChar = false;
                    xNum = Int32.Parse(x);
                }

                bool yIsChar = false;
                int yNum = 0;
                char yChar = ' ';

                if (match.Groups[3].Success)
                {
                    string y = match.Groups[3].Value;
                    if (isChar.IsMatch(y))
                    {
                        yChar = y[0];
                        yIsChar = true;
                    }
                    else
                    {
                        yNum = Int32.Parse(y);
                        yIsChar = false;
                    }
                }

                switch (opr)
                {
                    case "snd":
                        if (xIsChar)
                        {
                            instructions.Add(() => PlaySound(xChar));
                        }
                        else
                        {
                            instructions.Add(() => PlaySound(xNum));
                        }
                        break;
                    case "set":
                        if (yIsChar)
                        {
                            instructions.Add(() => SetValue(xChar, yChar));
                        }
                        else
                        {
                            instructions.Add(() => SetValue(xChar, yNum));
                        }
                        break;
                    case "add":
                        if (yIsChar)
                        {
                            instructions.Add(() => AddValue(xChar, yChar));
                        }
                        else
                        {
                            instructions.Add(() => AddValue(xChar, yNum));
                        }
                        break;
                    case "mul":
                        if (yIsChar)
                        {
                            instructions.Add(() => MultiplyValue(xChar, yChar));
                        }
                        else
                        {
                            instructions.Add(() => MultiplyValue(xChar, yNum));
                        }
                        break;
                    case "mod":
                        if (yIsChar)
                        {
                            instructions.Add(() => ModValue(xChar, yChar));
                        }
                        else
                        {
                            instructions.Add(() => ModValue(xChar, yNum));
                        }
                        break;
                    case "rcv":
                        if (xIsChar)
                        {
                            instructions.Add(() => Recover(xChar));
                        }
                        else
                        {
                            instructions.Add(() => Recover(xNum));
                        }
                        break;
                    case "jgz":
                        if (yIsChar)
                        {
                            if (xIsChar)
                            {
                                instructions.Add(() =>
                                {
                                    return JumpGZ(xChar, yChar);
                                });
                            }
                            else
                            {
                                instructions.Add(() =>
                                {
                                    return JumpGZ(xNum, yChar);
                                });
                            }
                        }
                        else
                        {
                            if (xIsChar)
                            {
                                instructions.Add(() =>
                                {
                                    return JumpGZ(xChar, yNum);
                                });
                            }
                            else
                            {
                                instructions.Add(() =>
                                {
                                    return JumpGZ(xNum, yNum);
                                });
                            }
                        }
                        break;
                    default:
                        throw new Exception("Line not handled: " + line);
                }
            }
        }

        public void Run()
        {
            long? rec = null;
            while (rec == null)
            {
                rec = instructions[currentLine++]();
            }
            Console.WriteLine(rec);
        }
        public long GetValue(char x)
        {
            if (registers.ContainsKey(x))
            {
                return registers[x];
            }
            else
            {
                return initialValue;
            }
        }

        public int? PlaySound(char x)
        {
            return PlaySound(GetValue(x));
        }

        public int? PlaySound(long x)
        {
            lastPlayed = x;
            return null;
        }

        public int? SetValue(char x, long y)
        {
            registers[x] = y;
            return null;
        }

        public int? SetValue(char x, char y)
        {
            return SetValue(x, GetValue(y));
        }

        public int? AddValue(char x, long y)
        {
            return SetValue(x, checked(GetValue(x) + y));
        }

        public int? AddValue(char x, char y)
        {
            return AddValue(x, GetValue(y));
        }

        public int? MultiplyValue(char x, long y)
        {
            return SetValue(x, checked(GetValue(x) * y));
        }

        public int? MultiplyValue(char x, char y)
        {
            return MultiplyValue(x, GetValue(y));
        }

        public int? ModValue(char x, long y)
        {
            return SetValue(x, GetValue(x) % y);
        }

        public long? ModValue(char x, char y)
        {
            return ModValue(x, GetValue(y));
        }

        public long? Recover(long x)
        {
            if (x != 0)
            {
                return lastPlayed;
            }
            else
            {
                return null;
            }
        }

        public long? Recover(char x)
        {
            return Recover(GetValue(x));
        }

        public long? JumpGZ(long x, long y)
        {
            if (x > 0)
            {
                if (y > Int32.MaxValue)
                {
                    throw new InvalidOperationException("y is larger than integer");
                }

                checked
                {
                    currentLine = currentLine + (int)y - 1;
                }
            }

            return null;
        }

        public long? JumpGZ(char x, long y)
        {
            return JumpGZ(GetValue(x), y);
        }

        public long? JumpGZ(long x, char y)
        {
            return JumpGZ(x, GetValue(y));
        }

        public long? JumpGZ(char x, char y)
        {
            return JumpGZ(GetValue(x), GetValue(y));
        }
    }
}
