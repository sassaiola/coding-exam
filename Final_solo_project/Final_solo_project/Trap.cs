using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_solo_project
{
    internal class Trap : GameObject
    {
        public PlatformBase ParentPlatform { get; }

        public Trap(SpriteSheet visualization, PlatformBase parentPlatform) : base(visualization)
        {
            ParentPlatform = parentPlatform;

            HitboxOffset = Vector2.Zero;
            HitboxSize = visualization.Size; // ✅ hitbox corretta
        }

        public void SnapToPlatformTop()
        {
            TopLeftPosition = new Vector2(
                ParentPlatform.TopLeftPosition.X + (ParentPlatform.Size.X - Size.X) / 2f,
                ParentPlatform.TopLeftPosition.Y - Size.Y
            );
            Visualization.TopLeftPosition = TopLeftPosition;
        }
    }
}

