using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_solo_project
{
    internal class StaticPlatform:PlatformBase
    {
        public StaticPlatform(SpriteSheet visualization) : base( visualization, PlatformType.Static)
        {
            JumpMultiplier = 1f;
        }
        public override void OnPlayerLanding(Doodler doodler)
        {

        }

    }
}
