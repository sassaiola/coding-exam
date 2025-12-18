using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_solo_project
{
    internal abstract class GameObject
    {
        public Vector2 TopLeftPosition {  get; set; }
        public Vector2 Size { get; set; }
        public SpriteSheet Visualization { get; set; }

        public Vector2 Velocity { get; set; }

        public Vector2 HitboxOffset { get; set; } = Vector2.Zero;
        public Vector2 HitboxSize { get; set; } = new Vector2(70, 85);


        public bool IsActive { get; set; }
        public virtual bool IsOutOfBounds
        {
            get
            {
                if (TopLeftPosition.X + Size.X < 0)
                    return true;

                if (TopLeftPosition.X > GameSetting.WindowWidth)
                    return true;

                if (TopLeftPosition.Y + Size.Y < 0)
                    return true;

                if (TopLeftPosition.Y > GameSetting.WindowHeight)
                    return true;

                return false;
            }
        }
        public bool IsCollidingWith(GameObject other)
        {
            Rectangle rect1 = new Rectangle(
                (int)(TopLeftPosition.X + HitboxOffset.X),
                (int)(TopLeftPosition.Y + HitboxOffset.Y),
                (int)HitboxSize.X,
                (int)HitboxSize.Y
            );

            Rectangle rect2 = new Rectangle(
                (int)(other.TopLeftPosition.X + other.HitboxOffset.X),
                (int)(other.TopLeftPosition.Y + other.HitboxOffset.Y),
                (int)other.HitboxSize.X,
                (int)other.HitboxSize.Y
            );

            return rect1.Intersects(rect2);
        }




        public GameObject(SpriteSheet visualization)
        {
            Visualization = visualization;

            TopLeftPosition = visualization.TopLeftPosition;
            Size = visualization.Size;

            HitboxOffset = Vector2.Zero;
            HitboxSize = Size;

            Velocity = Vector2.Zero;
            IsActive = true;
        }

        public virtual void MoveGameObject()
        {
            TopLeftPosition += Velocity;
            Visualization.TopLeftPosition = TopLeftPosition;
        }
        public virtual void Update(GameTime gameTime)
        {
            if (!IsActive) return;
            MoveGameObject();
            if (IsOutOfBounds)
                IsActive = false;
            Visualization.Update(gameTime);
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
if(!IsActive) return;
Visualization.Draw(spriteBatch);

        }

    }
}
