using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Final_solo_project
{
    internal class StartScreen : Screen
    {
        private SpriteFont font;
        private Texture2D pixel;

        private float t; // timer animazioni

        public override void Initialize()
        {
            t = 0f;
        }

        public override void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("fonts/UIFont2");

            pixel = new Texture2D(GameSetting.GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            t += dt;

            // START
            if (UserInput.IsNewKeyPress(Keys.Space))
            {
                GameSetting.ActiveScreen = GameSetting.PlayScreen;
                GameSetting.ActiveScreen.Initialize();
                return;
            }

            // Toggle SFX (opzionale)
            if (UserInput.IsNewKeyPress(Keys.Enter))
            {
                AudioManager.SfxEnabled = !AudioManager.SfxEnabled;
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            sb.GraphicsDevice.Clear(new Color(90, 150, 240));

            string title = "SLASHING ADVENTURE";
            string sub = "ARROWS: Move";
            string sub2 = "SPACE: Shoot";
            string sub3 = $"SFX: {(AudioManager.SfxEnabled ? "ON" : "OFF")} (ENTER to toggle)";

            // titolo
            DrawShadowText(sb, title, new Vector2(0, 140), centerX: true, scale: 3.5f, Color.White);

            // istruzioni
            DrawShadowText(sb, sub, new Vector2(0, 260), centerX: true, scale: 2.0f, Color.White);
            DrawShadowText(sb, sub2, new Vector2(0, 300), centerX: true, scale: 2.0f, Color.White);
            DrawShadowText(sb, sub3, new Vector2(0, 350), centerX: true, scale: 1.9f, Color.White * 0.95f);

            // ✅ scritta animata
            DrawPulsingText(sb, "PRESS SPACE TO PLAY", new Vector2(0, 450), centerX: true,
                baseScale: 1.6f, ampScale: 0.08f, speed: 5.0f, baseAlpha: 0.85f);
        }

        // ---------- UI helpers ----------
        private void DrawShadowText(SpriteBatch sb, string text, Vector2 pos, bool centerX, float scale, Color color)
        {
            text = SanitizeForFont(text);

            Vector2 size = font.MeasureString(text) * scale;
            Vector2 p = pos;

            if (centerX)
                p.X = (GameSetting.WindowWidth - size.X) / 2f;

            sb.DrawString(font, text, p + new Vector2(2, 2), Color.Black * 0.55f, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            sb.DrawString(font, text, p, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        private void DrawPulsingText(SpriteBatch sb, string text, Vector2 pos, bool centerX,
            float baseScale, float ampScale, float speed, float baseAlpha)
        {
            text = SanitizeForFont(text);

            float s = baseScale + (float)Math.Sin(t * speed) * ampScale;
            float a = baseAlpha + (float)Math.Sin(t * speed) * 0.10f;
            a = MathHelper.Clamp(a, 0.2f, 1f);

            Vector2 size = font.MeasureString(text) * s;
            Vector2 p = pos;

            if (centerX)
                p.X = (GameSetting.WindowWidth - size.X) / 2f;

            sb.DrawString(font, text, p + new Vector2(2, 2), Color.Black * (0.55f * a), 0f, Vector2.Zero, s, SpriteEffects.None, 0f);
            sb.DrawString(font, text, p, Color.White * a, 0f, Vector2.Zero, s, SpriteEffects.None, 0f);
        }

        private string SanitizeForFont(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            // filtra caratteri “strani” che possono far crashare SpriteFont
            // (teniamo ASCII base + lettere/numeri/spazi/punteggiatura comune)
            System.Text.StringBuilder sb = new System.Text.StringBuilder(s.Length);
            foreach (char c in s)
            {
                if (c == '\n' || c == '\r' || c == '\t') { sb.Append(' '); continue; }
                if (c >= 32 && c <= 126) sb.Append(c);
                else sb.Append(' '); // sostituisce unicode non supportati
            }
            return sb.ToString();
        }
    }
}

