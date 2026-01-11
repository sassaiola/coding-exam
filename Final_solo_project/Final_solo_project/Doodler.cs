using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Final_solo_project
{
    internal class Doodler : GameObject
    {
        public bool IsOnPlatform { get; set; }

        private float platformAnimTimer;
        private bool platformAnimToggle;

        public float JumpSpeed { get; set; } = 15f;
        public bool IsFalling => Velocity.Y > 0;

        public override bool IsOutOfBounds => TopLeftPosition.Y > GameSetting.WindowHeight;

        // ===== Attack =====
        private bool isAttacking;
        private float attackFrameTimer;

        private const float AttackFrameTime = 0.05f;
        private int attackTotalFrames = 8;

        private SpriteSheet normalSprite;
        private SpriteSheet attackSprite;

        public Doodler(SpriteSheet visualization) : base(visualization)
        {
            normalSprite = visualization;

            Visualization = normalSprite;

            Velocity = Vector2.Zero;
        }

        public void SetAttackSprite(SpriteSheet sprite, int totalFrames)
        {
            attackSprite = sprite;
            attackTotalFrames = totalFrames;

            attackSprite.SpriteIndex = 0;
            attackSprite.TopLeftPosition = TopLeftPosition;
        }

        public void StartAttack()
        {
            if (attackSprite == null) return;
            if (isAttacking) return;

            isAttacking = true;
            attackFrameTimer = 0f;
            attackSprite.SpriteIndex = 0;
        }

        public void UpdateAnimation(GameTime gameTime)
        {
            // ===== Attack one-shot =====
            if (isAttacking && attackSprite != null)
            {
                attackFrameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (attackFrameTimer >= AttackFrameTime)
                {
                    attackFrameTimer = 0f;
                    attackSprite.SpriteIndex++;

                    if (attackSprite.SpriteIndex >= attackTotalFrames)
                    {
                        isAttacking = false;
                        attackSprite.SpriteIndex = 0;
                    }
                }

                return;
            }

            // ===== Animazione normale =====
            if (IsOnPlatform)
            {
                platformAnimTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (platformAnimTimer >= 0.3f)
                {
                    platformAnimTimer = 0f;
                    platformAnimToggle = !platformAnimToggle;
                }

                normalSprite.SpriteIndex = platformAnimToggle ? 1 : 2;
            }
            else if (Velocity.Y < 0)
            {
                normalSprite.SpriteIndex = 3; // salita
            }
            else
            {
                normalSprite.SpriteIndex = 0; // discesa
            }
        }

        public override void Update(GameTime gameTime)
        {
            // Input orizzontale
            KeyboardState keyboard = Keyboard.GetState();
            float moveSpeed = 5f;

            if (keyboard.IsKeyDown(Keys.Left))
                Velocity = new Vector2(-moveSpeed, Velocity.Y);
            else if (keyboard.IsKeyDown(Keys.Right))
                Velocity = new Vector2(moveSpeed, Velocity.Y);
            else
                Velocity = new Vector2(0, Velocity.Y);

            // Gravità
            Velocity = new Vector2(Velocity.X, Velocity.Y + 0.4f);

            // Movimento + update 
            base.Update(gameTime);

            // Warp
            if (TopLeftPosition.X > GameSetting.WindowWidth)
                TopLeftPosition = new Vector2(-Size.X, TopLeftPosition.Y);
            else if (TopLeftPosition.X + Size.X < 0)
                TopLeftPosition = new Vector2(GameSetting.WindowWidth, TopLeftPosition.Y);

            // Sync  entrambe le sheet
            normalSprite.TopLeftPosition = TopLeftPosition;
            if (attackSprite != null)
                attackSprite.TopLeftPosition = TopLeftPosition;

            // Mantieni coerente anche Visualization (che è normalSprite)
            Visualization.TopLeftPosition = TopLeftPosition;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsActive) return;

            // Disegna l’attack SOLO mentre attacca, altrimenti normale
            if (isAttacking && attackSprite != null)
                attackSprite.Draw(spriteBatch);
            else
                normalSprite.Draw(spriteBatch);
        }
    }
}
