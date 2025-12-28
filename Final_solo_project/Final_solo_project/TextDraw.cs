using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_solo_project
{
    internal static class TextDraw
    {
        public static void Shadow(SpriteBatch sb, SpriteFont font, string text, Vector2 pos, Color color,
                                  Vector2 shadowOffset, Color shadowColor, float scale = 1f)
        {
            sb.DrawString(font, text, pos + shadowOffset, shadowColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            sb.DrawString(font, text, pos, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public static void Outline(SpriteBatch sb, SpriteFont font, string text, Vector2 pos, Color color,
                                   Color outlineColor, int thickness = 2, float scale = 1f)
        {
            for (int x = -thickness; x <= thickness; x++)
                for (int y = -thickness; y <= thickness; y++)
                {
                    if (x == 0 && y == 0) continue;
                    sb.DrawString(font, text, pos + new Vector2(x, y), outlineColor, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                }

            sb.DrawString(font, text, pos, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public static Vector2 Centered(SpriteFont font, string text, Vector2 center, float scale = 1f)
        {
            var size = font.MeasureString(text) * scale;
            return center - size / 2f;
        }
    }
}

