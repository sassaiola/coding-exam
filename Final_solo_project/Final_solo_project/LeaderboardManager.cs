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
        public string Date; // es: "2025-12-28"

        public ScoreEntry(string name, int score, string date)
        {
            Name = name;
            Score = score;
            Date = date;
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

        // formato riga: Name|Score|Date
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

                    var parts = line.Split('|');

                    // Backward compatible:
                    // vecchio formato: Name|Score
                    // nuovo formato:  Name|Score|Date
                    if (parts.Length < 2) continue;

                    string name = parts[0].Trim();
                    if (string.IsNullOrWhiteSpace(name)) name = "PLAYER";

                    if (!int.TryParse(parts[1].Trim(), out int score)) continue;
                    if (score < 0) continue;

                    string date = (parts.Length >= 3) ? parts[2].Trim() : "";
                    if (string.IsNullOrWhiteSpace(date))
                        date = DateTime.Now.ToString("yyyy-MM-dd");

                    list.Add(new ScoreEntry(name, score, date));
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

        // ✅ questa firma resta uguale a prima (così EndScreen non si rompe)
        public static List<ScoreEntry> AddScore(string name, int score)
        {
            name = (name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name)) name = "PLAYER";
            if (name.Length > 12) name = name.Substring(0, 12);

            string date = DateTime.Now.ToString("yyyy-MM-dd");

            var list = LoadTop10();
            list.Add(new ScoreEntry(name, score, date));

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

                var lines = list.Select(e => $"{e.Name}|{e.Score}|{e.Date}");
                File.WriteAllLines(FilePath, lines);
            }
            catch
            {

            }
        }
    }
}


