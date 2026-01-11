using Microsoft.Xna.Framework;

namespace Final_solo_project
{
    internal class Enemy : GameObject
    {
        public int KillScore { get; } = 250;

        private float animTimer;
        private const float FrameTime = 0.12f; 
        private const int TotalFrames = 4;

        public Enemy(SpriteSheet visualization) : base(visualization)
        {
            Velocity = Vector2.Zero;

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

            Visualization.Update(gameTime);
        }

        public override bool IsOutOfBounds
        {
            get { return TopLeftPosition.Y > GameSetting.WindowHeight; }
        }
    }
}
