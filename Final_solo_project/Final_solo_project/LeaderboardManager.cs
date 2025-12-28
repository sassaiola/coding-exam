using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Final_solo_project
{
    internal struct ScoreEntry
    {
        public string Name;
        public int Score;

        public ScoreEntry(string name, int score)
        {
            Name = name;
            Score = score;
        }
    }

    internal static class LeaderboardManager
    {
        private const string FileName = "leaderboard.txt";
        private const int MaxEntries = 10;

        private static string FolderPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                         "Final_solo_project");

        private static string FilePath => Path.Combine(FolderPath, FileName);

        public static List<ScoreEntry> LoadTop10()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return new List<ScoreEntry>();

                var lines = File.ReadAllLines(FilePath);
                var list = new List<ScoreEntry>();

                foreach (var raw in lines)
                {
                    var line = raw.Trim();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // formato: Name|Score
                    var parts = line.Split('|');
                    if (parts.Length != 2) continue;

                    string name = parts[0].Trim();
                    if (string.IsNullOrWhiteSpace(name)) name = "PLAYER";

                    if (!int.TryParse(parts[1].Trim(), out int score)) continue;
                    if (score < 0) continue;

                    list.Add(new ScoreEntry(name, score));
                }

                return list
                    .OrderByDescending(e => e.Score)
                    .Take(MaxEntries)
                    .ToList();
            }
            catch
            {
                return new List<ScoreEntry>();
            }
        }

        public static bool WouldEnterTop10(int score)
        {
            var list = LoadTop10();
            if (list.Count < MaxEntries) return true;
            return score > list.Min(e => e.Score);
        }

        public static List<ScoreEntry> AddScore(string name, int score)
        {
            name = (name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name)) name = "PLAYER";
            if (name.Length > 12) name = name.Substring(0, 12);

            var list = LoadTop10();
            list.Add(new ScoreEntry(name, score));

            list = list
                .OrderByDescending(e => e.Score)
                .Take(MaxEntries)
                .ToList();

            Save(list);
            return list;
        }

        private static void Save(List<ScoreEntry> list)
        {
            try
            {
                Directory.CreateDirectory(FolderPath);

                var lines = list.Select(e => $"{e.Name}|{e.Score}");
                File.WriteAllLines(FilePath, lines);
            }
            catch
            {
                // no crash
            }
        }
    }
}

