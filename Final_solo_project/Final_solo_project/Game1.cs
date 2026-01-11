using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Final_solo_project
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private StartScreen _startScreen;
        private ScreenPlay _playScreen;
        private EndScreen _endScreen;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 1200;
            _graphics.ApplyChanges();

            GameSetting.WindowWidth = _graphics.PreferredBackBufferWidth;
            GameSetting.WindowHeight = _graphics.PreferredBackBufferHeight;
            GameSetting.GraphicsDevice = GraphicsDevice;

            _startScreen = new StartScreen();
            _playScreen = new ScreenPlay();
            _endScreen = new EndScreen();

            GameSetting.StartScreen = _startScreen;
            GameSetting.PlayScreen = _playScreen;
            GameSetting.EndScreen = _endScreen;

            GameSetting.ActiveScreen = GameSetting.StartScreen;
            GameSetting.ActiveScreen.Initialize();

            base.Initialize();
        }


        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            GameSetting.WindowWidth = GraphicsDevice.Viewport.Width;
            GameSetting.WindowHeight = GraphicsDevice.Viewport.Height;
            GameSetting.GraphicsDevice = GraphicsDevice;

            //AUDIO
            AudioManager.LoadContent(Content);

            // content per le screen 
            GameSetting.StartScreen.LoadContent(Content);
            GameSetting.PlayScreen.LoadContent(Content);
            GameSetting.EndScreen.LoadContent(Content);
        }




        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            UserInput.Update();

            GameSetting.ActiveScreen.Update(gameTime);

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            GameSetting.ActiveScreen?.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }



    }
}
