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
    internal class StartScreen : Screen
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
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            string title = "DOODLE JUMP - MONOGAME";
            string instructions =
                "LEFT / RIGHT  : Move\n" +
                "ENTER         : Start Game\n" +
                "ESC           : Quit";

            Vector2 titlePos = new Vector2(
                GameSetting.WindowWidth / 2f - font.MeasureString(title).X / 2f,
                GameSetting.WindowHeight / 3f
            );

            Vector2 instrPos = new Vector2(
                GameSetting.WindowWidth / 2f - font.MeasureString(instructions).X / 2f,
                GameSetting.WindowHeight / 2f
            );

            spriteBatch.DrawString(font, title, titlePos, Color.White);
            spriteBatch.DrawString(font, instructions, instrPos, Color.White);
        }

    }
}
