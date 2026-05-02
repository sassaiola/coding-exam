using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Final_solo_project
{
    internal abstract class BaseScreen : Screen
    {
        protected KeyboardState currentKeyboard;
        protected KeyboardState previousKeyboard;

        protected SpriteFont font;
        protected Texture2D pixel;

        public override void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("fonts/UIFont2");

            pixel = new Texture2D(GameSetting.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        protected void UpdateKeyboard()
        {
            previousKeyboard = currentKeyboard;
            currentKeyboard = Keyboard.GetState();
        }

        protected bool IsNewKeyPress(Keys key)
        {
            return currentKeyboard.IsKeyDown(key) && previousKeyboard.IsKeyUp(key);
        }

        protected virtual void DrawBottomBar(SpriteBatch spriteBatch)
        {
            int barHeight = 36;
            int y = GameSetting.WindowHeight - barHeight;

            // background
            spriteBatch.Draw(
                pixel,
                new Rectangle(0, y, GameSetting.WindowWidth, barHeight),
                new Color(0, 0, 0, 160)
            );

            // LEFT — current time
            string time = DateTime.Now.ToString("HH:mm:ss");
            spriteBatch.DrawString(font, time, new Vector2(10, y + 8), Color.White);

            // CENTER — bombs
            string bombs = $"Bombs: {GameSetting.ActiveBombs} / {GameSetting.TotalBombs}";
            Vector2 bombsSize = font.MeasureString(bombs);
            spriteBatch.DrawString(
                font,
                bombs,
                new Vector2((GameSetting.WindowWidth - bombsSize.X) / 2f, y + 8),
                Color.White
            );

            // RIGHT — score
            string score = $"Score: {GameSetting.LastScore}";
            Vector2 scoreSize = font.MeasureString(score);
            spriteBatch.DrawString(
                font,
                score,
                new Vector2(GameSetting.WindowWidth - scoreSize.X - 10, y + 8),
                Color.White
            );
        }
    }
}

