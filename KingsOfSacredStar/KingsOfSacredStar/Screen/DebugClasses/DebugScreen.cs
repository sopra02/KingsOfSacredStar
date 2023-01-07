using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using KingsOfSacredStar.InputWrapper;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen.DebugClasses
{
    internal sealed class DebugScreen : IScreen
    {
        private readonly SpriteFont mFont;
        private readonly double[] mSamples = new double[60];

        private int mSampleIndex;
        private Point mLastMouseClickPosition;
        private Point mLastGameFieldClickPosition;

        private readonly InputManager mInput;
        private readonly Camera mCamera;
        private readonly GraphicsDevice mGraphicsDevice;
        private readonly BasicEffect mBasicEffect;


        public DebugScreen(ContentManager content, InputManager input, Camera camera, IGraphicsDeviceService graphics)
        {
            mFont = content.Load<SpriteFont>("fonts/Small");
            mInput = input;
            mCamera = camera;
            mGraphicsDevice = graphics.GraphicsDevice;
            mBasicEffect = new BasicEffect(mGraphicsDevice);
        }

        public void Update(GameTime gameTime, Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            // Nothing to update
        }

        void IScreen.DrawHud(SpriteBatch spriteBatch)
        {
            ShowDebugInformation(spriteBatch);
        }

        [Conditional("DEBUG")]
        private void ShowDebugInformation(SpriteBatch spriteBatch)
        {
            var fps = CalculateFps();

            var debugInfo = new List<string>
            {
                "FPS: " + fps,
                "Mouse Pos:" + mInput.GetMousePosition(),
                "LastClick: " + mLastMouseClickPosition
            };

            if (GameState.Current != null)
            {
                debugInfo.Add("Is Paused: " + GameState.Current.IsPaused);
            }

            var yPos = 35;
            foreach (var info in debugInfo)
            {
                spriteBatch.DrawString(mFont, info, new Vector2(0, yPos), Color.Yellow);
                yPos += mFont.LineSpacing;
            }
        }

        private int CalculateFps()
        {
            if (mSamples[0] <= 0)
            {
                return 60;
            }

            return (int) ((mSamples[mSamples.Length - 1] <= 0 ? mSampleIndex : mSamples.Length) / mSamples.Sum());
        }

        public void Draw(GameTime gameTime)
        {
            mSamples[mSampleIndex] = gameTime.ElapsedGameTime.TotalSeconds;
            mSampleIndex = (mSampleIndex + 1) % mSamples.Length;
            DrawRayFromCameraToLastClickedGameFieldPosition();
        }

        private void DrawRayFromCameraToLastClickedGameFieldPosition()
        {

            // Start position needs minimal offset, else you are looking exactly at the ray, and it seems invisible.
            var startPoint = mCamera.Position + new Vector3(0, -1, 0);
            var endPoint = new Vector3(mLastGameFieldClickPosition.X, 0, mLastGameFieldClickPosition.Y);


            DrawRay(startPoint, endPoint);
        }

        private void DrawRay(Vector3 startPoint, Vector3 endPoint)
        {
            mBasicEffect.View = mCamera.ViewMatrix;
            mBasicEffect.Projection = mCamera.ProjectionMatrix;
            mBasicEffect.CurrentTechnique.Passes[0].Apply();

            var vertices = new[]
                {new VertexPositionColor(startPoint, Color.White), new VertexPositionColor(endPoint, Color.White)};
            mGraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        }

        public bool ProcessMouseLeftClick(Point inputLastMouseClickPosition) => ProcessMouseClick(inputLastMouseClickPosition);

        public bool ProcessMouseRightClick(Point inputLastMouseClickPosition) => ProcessMouseClick(inputLastMouseClickPosition);

        private bool ProcessMouseClick(Point inputLastMouseClickPosition)
        {
            mLastMouseClickPosition = inputLastMouseClickPosition;
            mLastGameFieldClickPosition = ToolBox.Current.ConvertScreenToGameFieldPosition(mLastMouseClickPosition);
            return false;
        }
    }

}

