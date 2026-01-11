using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Final_solo_project
{
    internal static class UIKeycap
    {
        // Disegna un “tasto” stile tastiera (rettangolo + bordo + label)
        public static void Draw(SpriteBatch sb, Texture2D pixel, SpriteFont font,
                                Vector2 pos, Vector2 size, string label,
                                Color fill, Color border, Color textColor,
                                float textScale = 0.9f)
        {
            // Fill
            sb.Draw(pixel, new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y), fill);

            // Border (2px)
            int b = 2;
            sb.Draw(pixel, new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, b), border); // top
            sb.Draw(pixel, new Rectangle((int)pos.X, (int)(pos.Y + size.Y - b), (int)size.X, b), border); // bottom
            sb.Draw(pixel, new Rectangle((int)pos.X, (int)pos.Y, b, (int)size.Y), border); // left
            sb.Draw(pixel, new Rectangle((int)(pos.X + size.X - b), (int)pos.Y, b, (int)size.Y), border); // right

            // Label centered
            Vector2 textSize = font.MeasureString(label) * textScale;
            Vector2 textPos = pos + (size - textSize) / 2f;
            sb.DrawString(font, label, textPos + new Vector2(2, 2), Color.Black * 0.5f, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
            sb.DrawString(font, label, textPos, textColor, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
        }

        public static float MeasureWidth(Vector2 size) => size.X;
    }
}
