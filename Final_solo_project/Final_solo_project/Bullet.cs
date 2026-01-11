using Microsoft.Xna.Framework;

namespace Final_solo_project
{
    internal class Bullet : GameObject
    {
        public Bullet(SpriteSheet visualization, float speedY = 14f) : base(visualization)
        {
            Velocity = new Vector2(0f, -speedY);

            HitboxOffset = Vector2.Zero;
            HitboxSize = visualization.Size;
        }
    }
}
