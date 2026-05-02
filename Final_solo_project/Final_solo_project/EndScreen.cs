using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Final_solo_project
{
    internal class EndScreen : BaseScreen
    {
        

        private float titleTimer;
        private float pulse;

        private List<ScoreEntry> top10 = new List<ScoreEntry>();

        // Name entry
        private bool enteringName;
        private string nameBuffer = "";


        public override void Initialize()
        {
            titleTimer = 0f;
            pulse = 0f;

            top10 = LeaderboardManager.LoadTop10();

            // top 10 -> abilita input nome
            enteringName = GameSetting.LastScoreQualifiesTop10;
            nameBuffer = "";

        }

        

        public override void Update(GameTime gameTime)
        {
            UpdateKeyboard();

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            titleTimer += dt;

            pulse = 0.85f + 0.15f * (float)Math.Sin(titleTimer * 3f);

            if (enteringName)
            {
                HandleNameEntry();
            }
            else
            {
                if (IsNewKeyPress(Keys.Enter))
                {
                    GameSetting.ActiveScreen = GameSetting.PlayScreen;
                    GameSetting.ActiveScreen.Initialize();
                    return;
                }

                if (IsNewKeyPress(Keys.Space))
                {
                    GameSetting.ActiveScreen = GameSetting.StartScreen;
                    GameSetting.ActiveScreen.Initialize();
                    return;
                }
            }
        }


        private void HandleNameEntry()
        {
            if (IsNewKeyPress(Keys.Back) && nameBuffer.Length > 0)
                nameBuffer = nameBuffer.Substring(0, nameBuffer.Length - 1);

            if (IsNewKeyPress(Keys.Enter))
            {
                if (string.IsNullOrWhiteSpace(nameBuffer))
                    nameBuffer = "PLAYER";

                top10 = LeaderboardManager.AddScore(nameBuffer, GameSetting.LastScore);

                enteringName = false;
                GameSetting.LastScoreQualifiesTop10 = false;
                return;
            }

            for (Keys k = Keys.A; k <= Keys.Z; k++)
            {
                if (IsNewKeyPress(k) && nameBuffer.Length < 12)
                    nameBuffer += k.ToString();
            }

            for (Keys k = Keys.D0; k <= Keys.D9; k++)
            {
                if (IsNewKeyPress(k) && nameBuffer.Length < 12)
                    nameBuffer += (char)('0' + (k - Keys.D0));
            }

            if (IsNewKeyPress(Keys.Space) && nameBuffer.Length < 12)
                nameBuffer += " ";

            if (IsNewKeyPress(Keys.OemMinus) && nameBuffer.Length < 12)
                nameBuffer += "-";
        }


    

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (font == null || pixel == null) return;

            // ===== Title =====
            string title = "GAME OVER";
            Vector2 titleSize = font.MeasureString(title) * (1.4f * pulse);
            Vector2 titlePos = new Vector2(
                (GameSetting.WindowWidth - titleSize.X) / 2f,
                40f
            );

            spriteBatch.DrawString(font, title, titlePos, Color.White, 0f, Vector2.Zero, 1.4f * pulse, SpriteEffects.None, 0f);

            // ===== Score =====
            string scoreText = $"SCORE: {GameSetting.LastScore}";
            spriteBatch.DrawString(font, scoreText, new Vector2(30, 120), Color.White);

            // ===== Bottom bar =====
            DrawBottomBar(spriteBatch);


            // ===== Name entry prompt =====
            if (enteringName)
            {
                spriteBatch.DrawString(font, "NEW HIGH SCORE! ENTER YOUR NAME:", new Vector2(30, 170), Color.Yellow);

                // box
                Vector2 boxPos = new Vector2(30, 210);
                Vector2 boxSize = new Vector2(360, 48);

                DrawPanel(spriteBatch, boxPos, boxSize, new Color(0, 0, 0, 120), Color.White);

                string shown = string.IsNullOrWhiteSpace(nameBuffer) ? "" : nameBuffer;
                // cursore lampeggiante
                bool blink = ((int)(titleTimer * 2f) % 2) == 0;
                if (blink && shown.Length < 12) shown += "_";

                spriteBatch.DrawString(font, shown, boxPos + new Vector2(12, 10), Color.White);

                spriteBatch.DrawString(font, "PRESS ENTER TO SAVE", new Vector2(30, 270), Color.White);
            }
            else
            {
                // ===== Controls =====
                spriteBatch.DrawString(font, "PRESS ENTER TO PLAY AGAIN", new Vector2(30, 170), Color.White);
                spriteBatch.DrawString(font, "PRESS SPACE FOR MAIN MENU", new Vector2(30, 200), Color.White);
                spriteBatch.DrawString(font, "PRESS ESC TO QUIT THE GAME", new Vector2(30, 230), Color.White);

            }

            // ===== Leaderboard =====
            DrawLeaderboard(spriteBatch);
        }

        private void DrawLeaderboard(SpriteBatch sb)
        {
            Vector2 panelPos = new Vector2(GameSetting.WindowWidth - 380, 120);
            Vector2 panelSize = new Vector2(350, 420);

            DrawPanel(sb, panelPos, panelSize, new Color(0, 0, 0, 110), Color.White);

            sb.DrawString(font, "TOP 10", panelPos + new Vector2(120, 12), Color.White);

            var list = (top10 != null) ? top10 : LeaderboardManager.LoadTop10();

            float y = panelPos.Y + 60;

            for (int i = 0; i < 10; i++)
            {
                string line;
                if (i < list.Count)
                {
                    string name = string.IsNullOrWhiteSpace(list[i].Name) ? "PLAYER" : list[i].Name;
                    string date = string.IsNullOrWhiteSpace(list[i].Date) ? "" : $" ({list[i].Date})";
                    line = $"{i + 1}. {name}  -  {list[i].Score}{date}";
                }
                else
                {
                    line = $"{i + 1}. ---";
                }

                sb.DrawString(font, line, new Vector2(panelPos.X + 18, y), Color.White);
                y += 32;
            }
        }

        private void DrawPanel(SpriteBatch sb, Vector2 pos, Vector2 size, Color fill, Color border)
        {
            sb.Draw(pixel, new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y), fill);

            int b = 2;
            sb.Draw(pixel, new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, b), border); // top
            sb.Draw(pixel, new Rectangle((int)pos.X, (int)(pos.Y + size.Y - b), (int)size.X, b), border); // bottom
            sb.Draw(pixel, new Rectangle((int)pos.X, (int)pos.Y, b, (int)size.Y), border); // left
            sb.Draw(pixel, new Rectangle((int)(pos.X + size.X - b), (int)pos.Y, b, (int)size.Y), border); // right
        }
    }
}
