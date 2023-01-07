using System;
using System.Collections.Generic;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen.MainMenu
{
    internal class AMainMenuPage : AMenuPage
    {
        private readonly PlaneRenderer mGroundPlaneRenderer;
        protected readonly Camera mCamera;
        private long mStartPan;
        public static readonly Vector3 sCameraPosition = new Vector3(0, GameScreen.GridSize * 3, 4 * GameScreen.GridSize);
        private readonly Vector3 mOriginalPosition;

        protected AMainMenuPage(ContentManager content, GraphicsDeviceManager graphics, Camera camera, AButton[] buttons) : base(buttons)
        {
            if (content == null || graphics == null || camera == null)
            {
                mGroundPlaneRenderer = null;
                mCamera = null;
                return;
            }
            mGroundPlaneRenderer = new PlaneRenderer(graphics, camera, new BasicEffect(graphics.GraphicsDevice)
            {
                TextureEnabled = true,
                Texture = content.Load<Texture2D>("textures/ground_sand")
            });
            mGroundPlaneRenderer.InitializePlane(GameScreen.GridSize * 50, 0, GameScreen.GridSize * 50, 50, 50);
            mCamera = camera;
            mOriginalPosition = camera.Position;
        }

        public override void Update(GameTime gameTime, Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            if (mStartPan == 0)
            {
                mStartPan = gameTime.TotalGameTime.Ticks;
            }

            base.Update(gameTime, addScreens, removeScreens);
        }

        protected void PanToPosition(Vector3 offset, GameTime gameTime)
        {
            var direction = (sCameraPosition - offset) - mOriginalPosition;
            const float panDuration = 10000000;
            if (gameTime.TotalGameTime.Ticks - mStartPan < panDuration)
            {
                mCamera.Position = mOriginalPosition + (direction * ((gameTime.TotalGameTime.Ticks - mStartPan) / panDuration));
            }
            else
            {
                mCamera.Position = sCameraPosition - offset;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (mGroundPlaneRenderer == null)
            {
                return;
            }

            mGroundPlaneRenderer.Draw();

            foreach (var entry in GetObjects(gameTime))
            {
                entry.Key.Draw(gameTime, entry.Value.ToArray());
            }
        }

        private static Dictionary<ModelManager.Model, List<ModelManager.RenderingInformation>> GetObjects(GameTime time)
        {
            const int gSize = GameScreen.GridSize;
            return new Dictionary<ModelManager.Model, List<ModelManager.RenderingInformation>>
            {
                [ModelManager.GetInstance().Rock] = new List<ModelManager.RenderingInformation>
                {
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(-6 * gSize, 0, -3 * gSize),
                        Color.White),
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(-5 * gSize, 0, -3 * gSize),
                        Color.White),
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(-4 * gSize, 0, -3 * gSize),
                        Color.White),
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(-3 * gSize, 0, -3 * gSize),
                        Color.White),
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(-2 * gSize, 0, -3 * gSize),
                        Color.White),
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(-1 * gSize, 0, -3 * gSize),
                        Color.White),
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(0 * gSize, 0, -3 * gSize),
                        Color.White),
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(1 * gSize, 0, -3 * gSize),
                        Color.White),
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(2 * gSize, 0, -3 * gSize),
                        Color.White),
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(3 * gSize, 0, -3 * gSize),
                        Color.White)
                },
                [ModelManager.GetInstance().SacredStar] = new List<ModelManager.RenderingInformation>
                {
                    new ModelManager.RenderingInformation(
                        Matrix.CreateTranslation(GameScreen.GridSize / 2f, 0, GameScreen.GridSize / 2f) *
                        Matrix.CreateRotationY(time.TotalGameTime.Ticks / 10000000f),
                        Color.White)
                },
                [ModelManager.GetInstance().Bowman] = new List<ModelManager.RenderingInformation>
                {
                    new ModelManager.RenderingInformation(
                        Matrix.CreateTranslation(GameScreen.GridSize / 2f + gSize, 0, GameScreen.GridSize / 2f) *
                        Matrix.CreateRotationY(-time.TotalGameTime.Ticks / 10000000f) *
                        Matrix.CreateTranslation(4 * gSize, 0, 3 * gSize),
                        Color.Gold),
                    new ModelManager.RenderingInformation(
                        Matrix.CreateTranslation(GameScreen.GridSize / 2f + gSize, 0, GameScreen.GridSize / 2f) *
                        Matrix.CreateRotationY(-time.TotalGameTime.Ticks / 20000000f) *
                        Matrix.CreateTranslation(3 * gSize, 0, 5 * gSize),
                        Color.Blue),
                    new ModelManager.RenderingInformation(
                        Matrix.CreateTranslation(GameScreen.GridSize / 2f + gSize, 0, GameScreen.GridSize / 2f) *
                        Matrix.CreateRotationY(-time.TotalGameTime.Ticks / 5000000f) *
                        Matrix.CreateTranslation(2 * gSize, 0, 4 * gSize),
                        Color.Green)
                },
                [ModelManager.GetInstance().Cavalry] = new List<ModelManager.RenderingInformation>
                {
                    new ModelManager.RenderingInformation(
                        Matrix.CreateTranslation(GameScreen.GridSize / 2f + gSize, 0, GameScreen.GridSize / 2f) *
                        Matrix.CreateRotationY(-time.TotalGameTime.Ticks / 10000000f) *
                        Matrix.CreateTranslation(0 * gSize, 0, 3 * gSize),
                        Color.Gold),
                    new ModelManager.RenderingInformation(
                        Matrix.CreateTranslation(GameScreen.GridSize / 2f + gSize, 0, GameScreen.GridSize / 2f) *
                        Matrix.CreateRotationY(-time.TotalGameTime.Ticks / 10000000f) *
                        Matrix.CreateTranslation(1 * gSize, 0, 5 * gSize),
                        Color.Blue),
                    new ModelManager.RenderingInformation(
                        Matrix.CreateTranslation(GameScreen.GridSize / 2f + gSize, 0, GameScreen.GridSize / 2f) *
                        Matrix.CreateRotationY(-time.TotalGameTime.Ticks / 10000000f) *
                        Matrix.CreateTranslation(-1 * gSize, 0, 4 * gSize),
                        Color.Green)
                },
                [ModelManager.GetInstance().BatteringRam] = new List<ModelManager.RenderingInformation>
                {
                    new ModelManager.RenderingInformation(
                        Matrix.CreateTranslation(GameScreen.GridSize / 2f + gSize, 0, GameScreen.GridSize / 2f) *
                        Matrix.CreateRotationY(-time.TotalGameTime.Ticks / 10000000f) *
                        Matrix.CreateTranslation(-3 * gSize, 0, 3 * gSize),
                        Color.White),
                    new ModelManager.RenderingInformation(
                        Matrix.CreateTranslation(GameScreen.GridSize / 2f + gSize, 0, GameScreen.GridSize / 2f) *
                        Matrix.CreateRotationY(-time.TotalGameTime.Ticks / 10000000f) *
                        Matrix.CreateTranslation(-4 * gSize, 0, 5 * gSize),
                        Color.Yellow),
                    new ModelManager.RenderingInformation(
                        Matrix.CreateTranslation(GameScreen.GridSize / 2f + gSize, 0, GameScreen.GridSize / 2f) *
                        Matrix.CreateRotationY(-time.TotalGameTime.Ticks / 10000000f) *
                        Matrix.CreateTranslation(-2 * gSize, 0, 4 * gSize),
                        Color.Red)
                },
                [ModelManager.GetInstance().Swordsman] = new List<ModelManager.RenderingInformation>
                {
                    new ModelManager.RenderingInformation(
                        Matrix.CreateTranslation(GameScreen.GridSize / 2f + gSize, 0, GameScreen.GridSize / 2f) *
                        Matrix.CreateRotationY(-time.TotalGameTime.Ticks / 10000000f) *
                        Matrix.CreateTranslation(4 * gSize, 0, -0 * gSize),
                        Color.Red),
                    new ModelManager.RenderingInformation(
                        Matrix.CreateTranslation(GameScreen.GridSize / 2f + gSize, 0, GameScreen.GridSize / 2f) *
                        Matrix.CreateRotationY(-time.TotalGameTime.Ticks / 10000000f) *
                        Matrix.CreateTranslation(6 * gSize, 0, -0 * gSize),
                        Color.Red),
                },
                [ModelManager.GetInstance().Gate] = new List<ModelManager.RenderingInformation>
                {
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(5 * gSize, 0, -1 * gSize),
                        Color.Red)
                },
                [ModelManager.GetInstance().Wall] = new List<ModelManager.RenderingInformation>
                {
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(4 * gSize, 0, -2 * gSize),
                        Color.White),
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(4 * gSize, 0, -3 * gSize),
                        Color.White),
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(6 * gSize, 0, -1 * gSize),
                        Color.White),
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(7 * gSize, 0, -1 * gSize),
                        Color.White),
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(8 * gSize, 0, -1 * gSize),
                        Color.White),
                },
                [ModelManager.GetInstance().Village] = new List<ModelManager.RenderingInformation>
                {
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(-5 * gSize, 0, 0 * gSize),
                        Color.White)
                },
                [ModelManager.GetInstance().GoldMine] = new List<ModelManager.RenderingInformation>
                {
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(-5 * gSize, 0, -5 * gSize),
                        Color.White)
                },
                [ModelManager.GetInstance().Bowman] = new List<ModelManager.RenderingInformation>
                {
                    new ModelManager.RenderingInformation(
                        Matrix.CreateTranslation(GameScreen.GridSize / 2f + 1.5f * gSize,
                            0,
                            GameScreen.GridSize / 2f) *
                        Matrix.CreateRotationY(-time.TotalGameTime.Ticks / 10000000f) *
                        Matrix.CreateTranslation(4 * gSize, 0, 3 * gSize),
                        Color.Gold),
                    new ModelManager.RenderingInformation(
                       Matrix.CreateTranslation(3 * gSize, 0, 5 * gSize),
                        Color.Blue),
                    new ModelManager.RenderingInformation(
                       Matrix.CreateTranslation(2 * gSize, 0, 4 * gSize),
                        Color.Green)
                },

                [ModelManager.GetInstance().Cavalry] = new List<ModelManager.RenderingInformation>
                {
                    new ModelManager.RenderingInformation(
                        Matrix.CreateTranslation(0 * gSize, 0, 3 * gSize),
                        Color.Gold),
                    new ModelManager.RenderingInformation(
                        Matrix.CreateTranslation(1 * gSize, 0, 5 * gSize),
                        Color.Blue),
                    new ModelManager.RenderingInformation(
                        Matrix.CreateTranslation(GameScreen.GridSize / 2f + gSize,
                            0,
                            GameScreen.GridSize / 2f) *
                        Matrix.CreateRotationY(-time.TotalGameTime.Ticks / 5000000f) *
                        Matrix.CreateTranslation(-1 * gSize, 0, 4 * gSize),
                        Color.Green)
                },
                [ModelManager.GetInstance().BatteringRam] = new List<ModelManager.RenderingInformation>
                {
                    new ModelManager.RenderingInformation(
                       Matrix.CreateTranslation(-3 * gSize, 0, 3 * gSize),
                        Color.White),
                    new ModelManager.RenderingInformation(
                        Matrix.CreateTranslation(GameScreen.GridSize / 2f + 0.5f * gSize,
                            0,
                            GameScreen.GridSize / 2f) *
                        Matrix.CreateRotationY(-time.TotalGameTime.Ticks / 20000000f) *
                        Matrix.CreateTranslation(-4 * gSize, 0, 5 * gSize),
                        Color.Yellow),
                    new ModelManager.RenderingInformation(
                        Matrix.CreateTranslation(-2 * gSize, 0, 4 * gSize),
                        Color.Red)
                },
                [ModelManager.GetInstance().Swordsman] = new List<ModelManager.RenderingInformation>
                {
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(4 * gSize, 0, -0 * gSize),
                        Color.Red),
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(6 * gSize, 0, -0 * gSize),
                        Color.Red),
                },
                [ModelManager.GetInstance().Gate] = new List<ModelManager.RenderingInformation>
                {
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(5 * gSize, 0, -1 * gSize), Color.Red)
                },
                [ModelManager.GetInstance().Wall] = new List<ModelManager.RenderingInformation>
                {
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(4 * gSize, 0, -2 * gSize),
                        Color.White),
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(4 * gSize, 0, -3 * gSize),
                        Color.White),
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(6 * gSize, 0, -1 * gSize),
                        Color.White),
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(7 * gSize, 0, -1 * gSize),
                        Color.White),
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(8 * gSize, 0, -1 * gSize),
                        Color.White),
                },

                [ModelManager.GetInstance().Village] = new List<ModelManager.RenderingInformation>
                {
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(-5 * gSize, 0, 0 * gSize),
                        Color.White),
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation( 30 * gSize, 0, -3 * gSize), Color.Gold),
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation( -30 * gSize, 0, -3 * gSize), Color.Red)
                },
                [ModelManager.GetInstance().GoldMine] = new List<ModelManager.RenderingInformation>
                {
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(-3 * gSize, 0, -6 * gSize),
                        Color.White)
                },
                [ModelManager.GetInstance().Quarry] = new List<ModelManager.RenderingInformation>
                {
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(-3 * gSize, 0, 15 * gSize),
                        Color.White)
                },
                [ModelManager.GetInstance().Hero] = new List<ModelManager.RenderingInformation>
                {
                    new ModelManager.RenderingInformation(Matrix.CreateTranslation(gSize / 2f, 0, gSize / 2f) *
                                                          Matrix.CreateRotationY(time.TotalGameTime.Ticks / 10000000f) *
                                                          Matrix.CreateTranslation(30 * gSize, 0, 0), Color.White),
                    new ModelManager.RenderingInformation(Matrix.CreateRotationZ(-MathHelper.PiOver2) *
                                                          Matrix.CreateTranslation(-gSize, -gSize / 4f, 0) *
                                                          Matrix.CreateTranslation(gSize / 2f, 0, gSize / 2f) *
                                                          Matrix.CreateRotationY(time.TotalGameTime.Ticks / 10000000f) *
                                                          Matrix.CreateTranslation(-30 * gSize, 0, 0), Color.White)
                }
            };
        }
    }
}
