using Microsoft.Xna.Framework.Graphics;

namespace Final_solo_project
{
    internal static class GameSetting
    {
        public static int CurrentScore { get; set; }

        public static int ActiveBombs { get; set; }
        public static int TotalBombs { get; set; }

        public static int WindowWidth { get; set; }
        public static int WindowHeight { get; set; }
        public static GraphicsDevice GraphicsDevice { get; set; }

        // ⭐ "Screen manager" vero e proprio:
        public static Screen StartScreen { get; set; }
        public static Screen PlayScreen { get; set; }
        public static Screen EndScreen { get; set; }

        public static Screen ActiveScreen { get; set; }

        public static int LastScore { get; set; }
        public static bool LastRunWasHighScore { get; set; }
        public static int HighScore { get; set; }
        public static bool LastScoreQualifiesTop10 { get; set; }






    }
}

