using System.Globalization;
using KingsOfSacredStar.InputWrapper;
using KingsOfSacredStar.KI;
using KingsOfSacredStar.Screen;
using KingsOfSacredStar.Screen.MainMenu;
using KingsOfSacredStar.Sound;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    internal sealed class KossGame : Game
    {
        private SpriteBatch mSpriteBatch;

        private ScreenManager mScreenManager;
        private InputManager mInputManager;
        private Camera mCamera;
        private SoundEffectManager mSoundEffectManager;
        private MusicManager mMusicManager;
        private readonly GraphicsDeviceManager mGraphics;


        public KossGame()
        {
            mGraphics = new GraphicsDeviceManager(this);
            Settings.ReadSettings();
            Language.CurrentLanguage();
            if (Settings.Dict["Fullscreen"] != "on")
            {
                ScreenSize.CurrentSize(mGraphics);
            }

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            ExitWrapper.Init(this);
            mGraphics.GraphicsProfile = GraphicsProfile.HiDef;

            if (Settings.Dict["Fullscreen"] == "on")
            {
                ScreenSize.CurrentSize(mGraphics);
            }
            else
            {
                mGraphics.ApplyChanges();
            }
            IsMouseVisible = false;
            mCamera = new Camera(mGraphics) {Position = AMainMenuPage.sCameraPosition};
            CameraWrapper.Init(mCamera);
            Globals.GetResolution(mGraphics);
            mInputManager = new InputManager(Content);
            mInputManager.GetCam(mCamera);
            ExitWrapper.LoadInputManager(mInputManager);
            CameraWrapper.LoadInputManager(mInputManager);
            ModelManager.Initialize(mGraphics.GraphicsDevice, Content, mCamera);
            AKi.LoadInputManager(mInputManager);
            ToolBox.Initialize(mGraphics, mCamera);
            SoundManager.Init();
            mMusicManager = new MusicManager(Content, "music/background", "music/win", "music/lose");

            mSoundEffectManager = new SoundEffectManager(Content, "sounds/battering_ram", "sounds/bowman", "sounds/cavalry", "sounds/swordsman", "sounds/hero");
            AAudioMenu.Init(mSoundEffectManager, mMusicManager);
            GameState.Init(mSoundEffectManager);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            mSpriteBatch = new SpriteBatch(GraphicsDevice);
            mMusicManager.Play(0);
            mScreenManager = new ScreenManager(Content, mGraphics, mCamera, mInputManager, mMusicManager);
            MouseInputWrapper.Init(mScreenManager);
            MouseInputWrapper.LoadInputManager(mInputManager);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (!IsActive)
            {
                return;
            }

            mInputManager.Update();
            mScreenManager.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.BlendState = BlendState.Opaque;

            mScreenManager.Draw(gameTime);

            mSpriteBatch.Begin();
            mScreenManager.DrawHud(mSpriteBatch);
            mSpriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void UnloadContent()
        {
            Settings.Dict["MasterVolume"] = SoundManager.MasterVolume.ToString(CultureInfo.InvariantCulture);
            Settings.Dict["EffectVolume"] = mSoundEffectManager.Volume.ToString(CultureInfo.InvariantCulture);
            Settings.Dict["MusicVolume"] = mMusicManager.Volume.ToString(CultureInfo.InvariantCulture);
            Settings.WriteSettings();
            GameState.Current?.SaveStatistics();
            base.UnloadContent();
        }
    }
}
