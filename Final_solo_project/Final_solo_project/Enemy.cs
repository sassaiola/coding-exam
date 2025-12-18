using Microsoft.Xna.Framework;

namespace Final_solo_project
{
    internal class Enemy : GameObject
    {
        public Enemy(SpriteSheet visualization)
            : base(visualization)
        {
            Velocity = Vector2.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
