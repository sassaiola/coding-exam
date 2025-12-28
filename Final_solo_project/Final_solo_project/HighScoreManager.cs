using System;
using System.IO;

namespace Final_solo_project
{
    internal static class HighScoreManager
    {
        private const string FileName = "highscore.txt";

        // Percorso sicuro (scrivibile) per Windows
        private static string FolderPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                         "Final_solo_project");

        private static string FilePath => Path.Combine(FolderPath, FileName);

        public static int LoadHighScore()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return 0;

                string text = File.ReadAllText(FilePath).Trim();

                if (int.TryParse(text, out int value) && value >= 0)
                    return value;

                // file corrotto -> reset
                return 0;
            }
            catch
            {
                // Se succede qualsiasi problema di I/O, non vogliamo crashare il gioco
                return 0;
            }
        }

        public static bool TrySetNewHighScore(int score)
        {
            int current = LoadHighScore();
            if (score <= current) return false;

            SaveHighScore(score);
            return true;
        }

        public static void SaveHighScore(int score)
        {
            try
            {
                Directory.CreateDirectory(FolderPath);
                File.WriteAllText(FilePath, score.ToString());
            }
            catch
            {
                // Ignora errori di I/O per non rompere l’esperienza di gioco
            }
        }
    }
}
