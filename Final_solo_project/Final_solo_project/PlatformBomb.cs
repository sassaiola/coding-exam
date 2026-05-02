using Final_solo_project;
using Microsoft.Xna.Framework;
using System.Drawing;

internal class PlatformBomb : GameObject
{
    public PlatformBase SourcePlatform { get; }

    public PlatformBomb(SpriteSheet visualization, PlatformBase sourcePlatform)
        : base(visualization)
    {
        SourcePlatform = sourcePlatform;
        Velocity = new Vector2(0f, -8f); // verso l’alto
    }

    public override bool IsOutOfBounds
    {
        get
        {
            return TopLeftPosition.Y + Size.Y < 0;
        }
    }
}


