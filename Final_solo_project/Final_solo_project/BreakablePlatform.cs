namespace Final_solo_project
{
    internal class BreakablePlatform : PlatformBase
    {
        public bool IsBroken { get; private set; }

        public BreakablePlatform(SpriteSheet visualization)
            : base(visualization, PlatformType.Breakable)
        {
            JumpMultiplier = 0.87f; 
        }

        public void ConsumeBreakFlag()
        {
            BrokeThisFrame = false;
        }



        public bool BrokeThisFrame { get; private set; }

        public override void OnPlayerLanding(Doodler doodler)
        {
            if (!IsActive) return;

            IsBroken = true;
            IsActive = false;

            AudioManager.PlayBreakingPlatform(); // audio crack
        }


        public void Repair()
        {
            IsBroken = false;
            IsActive = true;
            BrokeThisFrame = false;
        }




      
    }
}


