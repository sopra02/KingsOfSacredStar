using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KingsOfSacredStar
{
    internal static class Settings
    {
        public static Dictionary<string, string> Dict { get; } = new Dictionary<string, string>();

        private const string FilePath = @"KossSettings.ini";
        public static void WriteSettings()
        {
            var keyValue = Dict.Select(i => string.Concat(i.Key, " ", i.Value));

            File.WriteAllLines(FilePath, keyValue);
        }

        public static void ReadSettings()
        {
            foreach (var line in File.ReadLines(FilePath))
            {
                var items = line.Split(' ');
                if (items.Length != 2) continue;
                Console.WriteLine(line);
                Dict.Add(items[0], items[1]);
            }
        }
    }
}
