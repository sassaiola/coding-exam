using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Final_solo_project
{
    internal class PowerUp : GameObject
    {
        public PowerUp(SpriteSheet visualization)
            : base(visualization)
        {
            Velocity = Vector2.Zero; 
        }

    }
}

