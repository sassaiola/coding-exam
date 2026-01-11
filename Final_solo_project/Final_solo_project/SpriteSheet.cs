using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Final_solo_project
{
    internal class SpriteSheet
    {
        public Texture2D Texture { get; set; }

        public int Rows { get; set; }
        public int Columns { get; set; }
        public Vector2 TopLeftPosition { get; set; }
        public Vector2 Size { get; set; }
        public int SpriteIndex { get; set; }

        public int CropX { get; set; } = 0;
        public int CropY { get; set; } = 0;


        private Rectangle[] _customSourceRects;

        public float Rotation { get; set; }
        public float RotationSpeed { get; set; }

        public virtual void Update(GameTime gameTime)
        {
            Rotation += RotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public SpriteSheet(Texture2D texture, int rows, int columns, Vector2 topLeftPosition, Vector2 size)
        {
            Texture = texture;
            Rows = rows;
            Columns = columns;
            TopLeftPosition = topLeftPosition;
            Size = size;
            SpriteIndex = 0;
        }


        private int ClampIndex(int idx)
        {
            int max = Rows * Columns - 1;
            if (idx < 0) return 0;
            if (idx > max) return max;
            return idx;
        }

        public Rectangle SourceRectangle
        {
            get
            {
                if (Texture == null || Rows <= 0 || Columns <= 0)
                    return Rectangle.Empty;

                int idx = ClampIndex(SpriteIndex);

                if (_customSourceRects != null && idx < _customSourceRects.Length)
                    return _customSourceRects[idx];

                int frameW = Texture.Width / Columns;
                int frameH = Texture.Height / Rows;

                int row = idx / Columns;
                int col = idx % Columns;

                int x = col * frameW;
                int y = row * frameH;

                int srcX = x + CropX;
                int srcY = y + CropY;

                int srcW = frameW - 2 * CropX;
                int srcH = frameH - 2 * CropY;

                if (srcW < 1) srcW = 1;
                if (srcH < 1) srcH = 1;

                if (srcX + srcW > Texture.Width) srcW = Texture.Width - srcX;
                if (srcY + srcH > Texture.Height) srcH = Texture.Height - srcY;

                return new Rectangle(srcX, srcY, srcW, srcH);
            }
        }

        public Rectangle DestinationRectangle =>
            new Rectangle((int)TopLeftPosition.X, (int)TopLeftPosition.Y, (int)Size.X, (int)Size.Y);

        public void BuildNormalizedTightSourceRects(byte alphaThreshold = 10, int padding = 1)
        {
            if (Texture == null || Rows <= 0 || Columns <= 0) return;

            int frameW = Texture.Width / Columns;
            int frameH = Texture.Height / Rows;

            var data = new Color[Texture.Width * Texture.Height];
            Texture.GetData(data);

            int totalFrames = Rows * Columns;
            var tight = new Rectangle[totalFrames];

            int globalW = 1;
            int globalH = 1;

            for (int i = 0; i < totalFrames; i++)
            {
                int row = i / Columns;
                int col = i % Columns;

                int fx = col * frameW;
                int fy = row * frameH;

                int minX = frameW, minY = frameH, maxX = -1, maxY = -1;

                for (int y = 0; y < frameH; y++)
                {
                    for (int x = 0; x < frameW; x++)
                    {
                        int px = fx + x;
                        int py = fy + y;
                        Color c = data[py * Texture.Width + px];

                        if (c.A > alphaThreshold)
                        {
                            if (x < minX) minX = x;
                            if (y < minY) minY = y;
                            if (x > maxX) maxX = x;
                            if (y > maxY) maxY = y;
                        }
                    }
                }

                if (maxX < 0 || maxY < 0)
                {
                    tight[i] = new Rectangle(fx, fy, frameW, frameH);
                }
                else
                {
                    minX = Math.Max(0, minX - padding);
                    minY = Math.Max(0, minY - padding);
                    maxX = Math.Min(frameW - 1, maxX + padding);
                    maxY = Math.Min(frameH - 1, maxY + padding);

                    int w = (maxX - minX) + 1;
                    int h = (maxY - minY) + 1;

                    globalW = Math.Max(globalW, w);
                    globalH = Math.Max(globalH, h);

                    tight[i] = new Rectangle(fx + minX, fy + minY, w, h);
                }
            }

            var normalized = new Rectangle[totalFrames];

            for (int i = 0; i < totalFrames; i++)
            {
                int row = i / Columns;
                int col = i % Columns;

                int fx = col * frameW;
                int fy = row * frameH;

                Rectangle t = tight[i];

                // centro del tight rect
                int cx = t.X + t.Width / 2;
                int cy = t.Y + t.Height / 2;

                int x = cx - globalW / 2;
                int y = cy - globalH / 2;

                // clamp dentro frame
                int minFrameX = fx;
                int minFrameY = fy;
                int maxFrameX = fx + frameW - globalW;
                int maxFrameY = fy + frameH - globalH;

                if (maxFrameX < minFrameX) maxFrameX = minFrameX;
                if (maxFrameY < minFrameY) maxFrameY = minFrameY;

                x = Math.Clamp(x, minFrameX, maxFrameX);
                y = Math.Clamp(y, minFrameY, maxFrameY);

                normalized[i] = new Rectangle(x, y, globalW, globalH);
            }

            _customSourceRects = normalized;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Texture == null) return;

            spriteBatch.Draw(
                Texture,
                destinationRectangle: DestinationRectangle,
                sourceRectangle: SourceRectangle,
                color: Color.White
            );
        }
    }
}


