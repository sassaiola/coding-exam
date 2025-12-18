using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_solo_project
{
    internal class SpriteSheetAnimation : SpriteSheet
    {
        public SpriteSheetAnimation(Texture2D texture, int rows, int columns, Vector2 topLeftPosition, Vector2 size) : base(texture, rows, columns, topLeftPosition, size)
        {
            
        }
    }
}
