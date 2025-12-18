using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_solo_project
{
    internal class DoubleJumpPlatform:PlatformBase
    {
        public DoubleJumpPlatform (SpriteSheet visualization):base (visualization, PlatformType.Moving)
        {
            JumpMultiplier = 1.8f;
        }
    }
}
