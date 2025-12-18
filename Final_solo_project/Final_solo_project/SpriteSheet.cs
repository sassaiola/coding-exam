using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_solo_project
{
    internal class SpriteSheet
    {

        public Texture2D Texture {  get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public Vector2 TopLeftPosition { get; set; }
        public Vector2 Size {  get; set; }
        public int SpriteIndex { get; set; }

        public float Rotation { get; set; }
        public float RotationSpeed { get; set; }

        public int CropX { get; set; } = 0;
        public int CropY { get; set; } = 0;

        public Vector2 SpriteSize 
        {
            get
            {
                if (Texture == null || Rows <= 0 || Columns <= 0)
                    return Vector2.Zero;
                else return new Vector2(
                    Texture.Width / (float)Columns, Texture.Height / (float)Rows
                    );
            }
        }
        public Rectangle SourceRectangle
        {
            get
            {
                if (Texture == null || Rows <= 0 || Columns <= 0)
                    return Rectangle.Empty;

                int spriteWidth = (int)SpriteSize.X;
                int spriteHeight = (int)SpriteSize.Y;

                // Calcolo riga e colonna a partire da SpriteIndex
                int row = SpriteIndex / Columns;
                int column = SpriteIndex % Columns;

                int x = column * spriteWidth;
                int y = row * spriteHeight;

                return new Rectangle( x + CropX,y + CropY,spriteWidth - 2 * CropX,spriteHeight - 2 * CropY);

            }
        }
        public Rectangle DestinationRectangle
        {
            get
            {
                return new Rectangle(
                    (int)TopLeftPosition.X,
                    (int)TopLeftPosition.Y,
                    (int)Size.X,
                    (int)Size.Y
                );
            }
        }
        public Vector2 Origin
        {
            get
            {
                return new Vector2(
                    DestinationRectangle.Width / 2f,
                    DestinationRectangle.Height / 2f
                );
            }
        }


        public SpriteSheet(Texture2D texture, int rows, int column, Vector2 topLeftPosition,
                        Vector2 size)
                    {
                        this.Texture = texture;
                        Rows = rows;
                        Columns = column;
                        TopLeftPosition = topLeftPosition;
                        Size = size;
                        SpriteIndex = 0;


                    }
        public virtual void Update(GameTime gameTime)
        {
            // Aggiorna la rotazione in base alla RotationSpeed (radianti al secondo)
            Rotation += RotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (Texture == null)
                return;

            spriteBatch.Draw(
    Texture,
    destinationRectangle: DestinationRectangle,
    sourceRectangle: SourceRectangle,
    color: Color.White,
    rotation: 0f,
    origin: Vector2.Zero,
    effects: SpriteEffects.None,
    layerDepth: 0f

            );
        }
    }
}


