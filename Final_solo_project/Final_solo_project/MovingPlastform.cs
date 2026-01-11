using Microsoft.Xna.Framework;
using System;

namespace Final_solo_project
{
    internal class MovingPlatform : PlatformBase
    {
        private float speed;

        public MovingPlatform(SpriteSheet visualization, float speed = 2.5f)
            : base(visualization, PlatformType.Moving)
        {
            this.speed = speed;
            Velocity = new Vector2(speed, 0f); 
            JumpMultiplier = 1.5f;
        }

        public override void MoveGameObject()
        {
            base.MoveGameObject();

            // rimbalzo sui bordi schermo
            if (TopLeftPosition.X <= 0)
            {
                TopLeftPosition = new Vector2(0, TopLeftPosition.Y);
                Velocity = new Vector2(Math.Abs(Velocity.X), 0f);
            }
            else if (TopLeftPosition.X + Size.X >= GameSetting.WindowWidth)
            {
                TopLeftPosition = new Vector2(GameSetting.WindowWidth - Size.X, TopLeftPosition.Y);
                Velocity = new Vector2(-Math.Abs(Velocity.X), 0f);
            }

            // risincronizza la visualization
            Visualization.TopLeftPosition = TopLeftPosition;
        }
    }
}

