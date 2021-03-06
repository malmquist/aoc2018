﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day4B
{
    static class Program
    {
        static void Main()
        {
            Console.WriteLine(GetAnswer());
        }

        private static int GetAnswer()
        {
            int result = -1;
            int maxTimes = -1;

            foreach (var guard in GetGuards())
            {
                var sleepyMinute = guard.GetSleepyMinute();

                if (sleepyMinute.times > maxTimes)
                {
                    maxTimes = sleepyMinute.times;
                    result = guard.Id * sleepyMinute.minute;
                }
            }

            return result;
        }

        private static IEnumerable<Guard> GetGuards()
        {
            var guards = new Dictionary<int, Guard>();

            Guard guard = null;
            var fellAsleep = DateTime.MinValue;

            foreach (var (timestamp, message) in GetLogs())
            {
                if (message[0] == 'G')
                {
                    var id = GetId(message);

                    if (!guards.ContainsKey(id))
                        guards[id] = new Guard(id);

                    guard = guards[id];
                }
                else if (message[0] == 'f')
                {
                    fellAsleep = timestamp;
                }
                else if (message[0] == 'w')
                {
                    guard.AddShift(fellAsleep, timestamp);
                }
            }

            return guards.Values.ToList();
        }

        private static int GetId(string message)
        {
            return int.Parse(Regex.Match(message, @"Guard #(\d+)").Groups[1].Value);
        }

        private static SortedDictionary<DateTime, string> GetLogs()
        {
            var regex = new Regex(@"\[(.+)\] (.+)");
            var logs = new SortedDictionary<DateTime, string>();

            foreach (var line in File.ReadAllLines("input.txt"))
            {
                var match = regex.Match(line);
                var timestamp = DateTime.Parse(match.Groups[1].Value);
                var message = match.Groups[2].Value;

                logs[timestamp] = message;
            }

            return logs;
        }
    }

    public class Guard
    {
        private readonly List<bool[]> shifts;

        public Guard(int id)
        {
            shifts = new List<bool[]>();
            Id = id;
        }

        public int Id { get; }

        public void AddShift(DateTime fellAsleep, DateTime wokeUp)
        {
            var minutes = new bool[60];

            for (int minute = fellAsleep.Minute; minute < wokeUp.Minute; ++minute)
                minutes[minute] = true;

            shifts.Add(minutes);
        }

        public (int minute, int times) GetSleepyMinute()
        {
            (int minute, int times) result = (-1, -1);

            for (int m = 0; m < 60; ++m)
            {
                var t = TimesSleptAt(m);

                if (t > result.times)
                    result = (m, t);
            }

            return result;
        }

        private int TimesSleptAt(int minute)
        {
            return shifts.Sum(minutes => minutes[minute] ? 1 : 0);
        }
    }
}
