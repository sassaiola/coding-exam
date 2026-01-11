using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_solo_project
{


        internal enum PlatformType
        {
            Static,
            Moving,
            Breakable
        }

    internal abstract class PlatformBase : GameObject
    {
        public float JumpMultiplier { get; set; } = 1f;
        public bool HasAttachment { get; set; } = false;

        public PlatformType Type { get; protected set; }

        public override bool IsOutOfBounds
        {
            get { return false; }
        }



        public PlatformBase(SpriteSheet visualization, PlatformType type)
            : base(visualization)
        {
            Type = type;
        }

        public virtual void OnPlayerLanding(Doodler doodler)
        {

        }
        

    }
}
