using Microsoft.Xna.Framework;

namespace Final_solo_project
{
    internal class Enemy : GameObject
    {
        public int KillScore { get; } = 250;

        // animazione 2x2 = 4 frame
        private float animTimer;
        private const float FrameTime = 0.12f; // velocità animazione (tunable)
        private const int TotalFrames = 4;

        public Enemy(SpriteSheet visualization) : base(visualization)
        {
            Velocity = Vector2.Zero;

            // se vuoi: assicurati che parta dal frame 0
            Visualization.SpriteIndex = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            animTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (animTimer >= FrameTime)
            {
                animTimer = 0f;
                Visualization.SpriteIndex++;

                if (Visualization.SpriteIndex >= TotalFrames)
                    Visualization.SpriteIndex = 0;
            }

            // aggiorna rotazioni ecc. se le usi (nel tuo SpriteSheet.Update cambia rotation)
            Visualization.Update(gameTime);
        }

        public override bool IsOutOfBounds
        {
            get { return TopLeftPosition.Y > GameSetting.WindowHeight; }
        }
    }
}
