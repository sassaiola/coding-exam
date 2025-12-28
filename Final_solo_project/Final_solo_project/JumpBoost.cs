using Microsoft.Xna.Framework;

namespace Final_solo_project
{
    internal class JumpBoost : GameObject
    {
        public PlatformBase ParentPlatform { get; }
        public float BoostMultiplier { get; set; } = 3f;

        // ---- animazione ----
        private bool isBouncing;
        private float animTimer;
        private float frameTime = 0.06f; // velocità anim (tunable)
        private int totalFrames = 4;     // hyperjump.png = 4 frame

        public JumpBoost(SpriteSheet visualization, PlatformBase parentPlatform)
            : base(visualization)
        {
            ParentPlatform = parentPlatform;

            HitboxOffset = Vector2.Zero;
            HitboxSize = visualization.Size;

            Visualization.SpriteIndex = 0; // idle frame
        }

        public void SnapToPlatformTop()
        {
            TopLeftPosition = new Vector2(
                ParentPlatform.TopLeftPosition.X + (ParentPlatform.Size.X - Size.X) / 2f,
                ParentPlatform.TopLeftPosition.Y - Size.Y
            );
            Visualization.TopLeftPosition = TopLeftPosition;
        }

        public void TriggerBounceAnim()
        {
            isBouncing = true;
            animTimer = 0f;
            Visualization.SpriteIndex = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (!IsActive) return;

            // se la piattaforma si muove/ricicla, resta agganciato sopra
            SnapToPlatformTop();

            if (isBouncing)
            {
                animTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (animTimer >= frameTime)
                {
                    animTimer = 0f;
                    Visualization.SpriteIndex++;

                    if (Visualization.SpriteIndex >= totalFrames)
                    {
                        // fine one-shot: torna idle
                        Visualization.SpriteIndex = 0;
                        isBouncing = false;
                    }
                }
            }

            Visualization.Update(gameTime);
        }
    }
}


