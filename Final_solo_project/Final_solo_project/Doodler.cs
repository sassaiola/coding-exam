using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics.Eventing.Reader;
using System.Xml;




namespace Final_solo_project
{
    internal class Doodler : GameObject
    {
        public bool IsOnPlatform { get; set; }
        private float platformAnimTimer;
        private bool platformAnimToggle;


        public float JumpSpeed { get; set; } = 15f;
        public bool IsFalling
        {
            get { return Velocity.Y > 0; }
        }
        public override bool IsOutOfBounds
        {
            get
            {
                if (TopLeftPosition.Y > GameSetting.WindowHeight) return true;
                return false;
            }
        }



        public Doodler(SpriteSheet visualization) : base(visualization)
        {
            Velocity = Vector2.Zero;
        }

        public void UpdateAnimation(GameTime gameTime)
        {
            if (IsOnPlatform)
            {
                platformAnimTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (platformAnimTimer >= 0.3f)
                {
                    platformAnimTimer = 0f;
                    platformAnimToggle = !platformAnimToggle;
                }

                Visualization.SpriteIndex = platformAnimToggle ? 1 : 2;
            }
            else if (Velocity.Y < 0)
            {
                Visualization.SpriteIndex = 3; // salita
            }
            else if (Velocity.Y > 0)
            {
                Visualization.SpriteIndex = 0; // discesa
            }
        }

        public override void Update(GameTime gameTime)
        {
            IsOnPlatform = false; // lo ScreenPlay lo metterà true quando rileva landing

            if (IsOnPlatform)
            {
                platformAnimTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (platformAnimTimer >= 0.3f)
                {
                    platformAnimTimer = 0f;
                    platformAnimToggle = !platformAnimToggle;
                }
                Visualization.SpriteIndex = platformAnimToggle ? 1 : 2;

            }
            else if (Velocity.Y > 0) { Visualization.SpriteIndex = 0; }
            else if (Velocity.Y < 0) { Visualization.SpriteIndex = 3; }



                KeyboardState keyboard = Keyboard.GetState();
            float moveSpeed = 5f;

            if (keyboard.IsKeyDown(Keys.Left))
            {
                Velocity= new Vector2(-moveSpeed, Velocity.Y);
            }
            else if (keyboard.IsKeyDown(Keys.Right))
            {
                Velocity = new Vector2(moveSpeed, Velocity.Y);
            }
            else 
            {
                Velocity = new Vector2 ( 0, Velocity.Y );
            }

            Velocity = new Vector2(Velocity.X, Velocity.Y + 0.4f);


            if (Math.Abs(Velocity.Y) > 0.1f)
                Visualization.SpriteIndex = 3;   // jump
            else
                Visualization.SpriteIndex = 0;   // idle placeholder

            





          
            base.Update(gameTime);
          if (this.TopLeftPosition.X> GameSetting.WindowWidth)
            {
                this.TopLeftPosition = new Vector2(-this.Size.X, this.TopLeftPosition.Y);
                IsActive = true;
                Visualization.TopLeftPosition = this.TopLeftPosition;

            }


          if (this.TopLeftPosition.X+Size.X<0)
            {
                this.TopLeftPosition = new Vector2(GameSetting.WindowWidth, this.TopLeftPosition.Y);
            }
        }






    }
}


