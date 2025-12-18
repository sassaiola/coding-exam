using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Final_solo_project
{
    internal class EndScreen : Screen
    {
        private SpriteFont font;
        public override void Initialize()
        {
        }

        public override void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("fonts/UIFont2");
        }


        public override void Update(GameTime gameTime)
        {
            if (UserInput.IsNewKeyPress(Keys.Enter))
            {
                GameSetting.ActiveScreen = GameSetting.PlayScreen;
                GameSetting.ActiveScreen.Initialize();
            }
            
            else if (UserInput.IsNewKeyPress(Keys.M))
            {
                GameSetting.ActiveScreen = GameSetting.StartScreen;
                GameSetting.ActiveScreen.Initialize();
            }
            else if (UserInput.IsNewKeyPress(Keys.Escape))
            {
                System.Environment.Exit(0);
            }
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            string title = "GAME OVER";
            string msg = "ENTER : Restart\nM : Menu\nESC : Exit Game";

            Vector2 titlePos = new Vector2(
                GameSetting.WindowWidth / 2f - font.MeasureString(title).X / 2f,
                GameSetting.WindowHeight / 3f
            );

            Vector2 msgPos = new Vector2(
                GameSetting.WindowWidth / 2f - font.MeasureString(msg).X / 2f,
                GameSetting.WindowHeight / 2f
            );

            spriteBatch.DrawString(font, title, titlePos, Color.White);
            spriteBatch.DrawString(font, msg, msgPos, Color.White);
        }

    }
}
