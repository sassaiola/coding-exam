using Final_solo_project;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Final_solo_project
{
    internal class ScreenPlay : Screen
    {
        private Doodler doodler;
        private List<PlatformBase> platforms;
        private Texture2D pixel;
        private Random random = new Random();
        private float score;
        private SpriteFont font;
        private float elapsedSeconds;
        private Texture2D DoodlerTexture;
        private int debugCollisions;



        public override void Initialize()
        {
            score = 0f;
            elapsedSeconds = 0f;

            if (pixel == null) return;

            ResetLevel();
        }



        public override void LoadContent(ContentManager content)
        {
            DoodlerTexture = content.Load<Texture2D>("SpriteSheetAnimation/Zorroverde");

            //   Texture2D platformTexture = content.Load<Texture2D>("platform");
            pixel = new Texture2D(GameSetting.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            font = content.Load<SpriteFont>("fonts/UIFont2");



            // Texture2D doodlerTexture = pixel;
            Texture2D platformTexture = pixel;
            ResetLevel();


        }
        private void ResetLevel()
        {

            platforms = new List<PlatformBase>();

            // usa pixel come texture placeholder (pixel esiste già da LoadContent)
            Texture2D doodlerTexture = DoodlerTexture ?? pixel;
            Texture2D platformTexture = pixel;


            var doodlerSprite = new SpriteSheet(doodlerTexture,1, 4,                    // 1 riga, 4 frame
    new Vector2(GameSetting.WindowWidth / 2f, GameSetting.WindowHeight / 3f),
    new Vector2(70, 85));

            doodlerSprite.CropX = 5;
            doodlerSprite.CropY = 0;


            // var doodlerSprite = new SpriteSheet(
            //     doodlerTexture, 1, 1,
            //     new Vector2(GameSetting.WindowWidth / 2f, GameSetting.WindowHeight / 3f),
            //     new Vector2(50, 50)

            doodler = new Doodler(doodlerSprite);

            // piattaforma sotto
            float startPlatformX = doodler.TopLeftPosition.X - 15f;
            float startPlatformY = doodler.TopLeftPosition.Y + doodler.Size.Y + 10f;

            var startPlatformSprite = new SpriteSheet(
                platformTexture, 1, 1,
                new Vector2(startPlatformX, startPlatformY),
                new Vector2(80, 20)
            );
            platforms.Add(new StaticPlatform(startPlatformSprite));

            int platformCount = 80;
            float verticalStep = 80f;

            for (int i = 0; i < platformCount; i++)
            {
                float x = random.Next(0, GameSetting.WindowWidth - 80);
                float y = GameSetting.WindowHeight - i * verticalStep;

                var platformSprite = new SpriteSheet(
                    platformTexture, 1, 1,
                    new Vector2(x, y),
                    new Vector2(80, 20)
                );

                platforms.Add(new StaticPlatform(platformSprite));
            }
        }






        public override void Update(GameTime gameTime)
        {
            doodler.IsOnPlatform = false;
            debugCollisions = 0;

            elapsedSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // aggiorna piattaforme (ok come prima)
            foreach (var platform in platforms)
                platform.Update(gameTime);

            // salva bottom del frame precedente
            float previousBottom = doodler.TopLeftPosition.Y + doodler.Size.Y;

            // muovi il doodler (gravità + input + posizione)
            doodler.Update(gameTime);

            // landing check (dopo che si è mosso)
            foreach (var platform in platforms)
            {
                if (doodler.IsCollidingWith(platform)) debugCollisions++;

                float platformTop = platform.TopLeftPosition.Y;
                float currentBottom = doodler.TopLeftPosition.Y + doodler.Size.Y;

                bool crossedTopWhileFalling =
                    doodler.Velocity.Y > 0 &&
                    previousBottom <= platformTop &&
                    currentBottom >= platformTop;

                if (crossedTopWhileFalling && doodler.IsCollidingWith(platform))
                {
                    doodler.IsOnPlatform = true;

                    // snap sopra la piattaforma
                    doodler.TopLeftPosition = new Vector2(
                        doodler.TopLeftPosition.X,
                        platformTop - doodler.Size.Y
                    );
                    doodler.Visualization.TopLeftPosition = doodler.TopLeftPosition;

                    // jump
                    float jump = doodler.JumpSpeed * platform.JumpMultiplier;
                    doodler.Velocity = new Vector2(doodler.Velocity.X, -jump);

                    platform.OnPlayerLanding(doodler);
                    break;
                }
            }







            float scrollThreshold = GameSetting.WindowHeight * 0.6f;


            if ( doodler.TopLeftPosition.Y < scrollThreshold && doodler.Velocity.Y<0 )
           
            {
                float delta = scrollThreshold - doodler.TopLeftPosition.Y;

                score += delta;

                doodler.TopLeftPosition = new Vector2(
                    doodler.TopLeftPosition.X,
                    scrollThreshold
                );

                

                foreach (var platform in platforms)
                {
                    platform.TopLeftPosition = new Vector2(
                        platform.TopLeftPosition.X,
                        platform.TopLeftPosition.Y + delta
                    );
                }
            }

            if (!doodler.IsActive)
            {
                GameSetting.ActiveScreen = GameSetting.EndScreen;
                GameSetting.ActiveScreen.Initialize();
                return;
            }

            doodler.UpdateAnimation(gameTime);

        }
        public override void Draw(SpriteBatch spriteBatch)
        {


            spriteBatch.DrawString(font, $"Colliding:{debugCollisions}", new Vector2(10, 90), Color.Yellow);

            // rettangolo del doodler
            //   var doodlerRect = new Rectangle(
            //       (int)doodler.TopLeftPosition.X,
            //       (int)doodler.TopLeftPosition.Y,
            //       (int)doodler.Size.X,
            //       (int)doodler.Size.Y
            //   );
            //
            //   spriteBatch.Draw(pixel, doodlerRect, Color.Green);
            doodler.Draw(spriteBatch);
            var hitbox = new Rectangle(
    (int)doodler.TopLeftPosition.X,
    (int)doodler.TopLeftPosition.Y,
    (int)doodler.HitboxSize.X,
    (int)doodler.HitboxSize.Y
);

            spriteBatch.Draw(pixel, hitbox, Color.Red * 0.4f);
 


            // rettangoli delle piattaforme
            foreach (var platform in platforms)
            {
                var platformRect = new Rectangle(
                    (int)platform.TopLeftPosition.X,
                    (int)platform.TopLeftPosition.Y,
                    (int)platform.Size.X,
                    (int)platform.Size.Y
                );

                spriteBatch.Draw(pixel, platformRect, Color.Blue);
            }
            string scoreText = "Score: " + ((int)score).ToString();

            spriteBatch.DrawString(
                font,
                scoreText,
                new Vector2(10, 10),
                Color.White
            );
            int totalSeconds = (int)elapsedSeconds;
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            string timeText = $"Time: {minutes:00}:{seconds:00}";

            spriteBatch.DrawString(
                font,
                timeText,
                new Vector2(10, 30),
                Color.White
            );
            



        }


    }
}
