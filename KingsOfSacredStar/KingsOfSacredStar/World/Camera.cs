using System;
#if DEBUG
using System.Diagnostics;
#endif
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World
{
    /// <summary>
    /// Camera that controls the displayed objects of the game.
    /// </summary>
    internal sealed class Camera
    {
        private readonly GraphicsDeviceManager mGraphics;

        // Let's start at X = 0 so we're looking at things head-on


        private readonly float mCameraAngle;

        private int mClippingSizeX;
        private int mClippingSizeZ;
        private readonly int mClippingSizeThreshold;
        private readonly float mCameraMovementHeightScaling;

        internal Vector3 Position
        {
            get;
            set;
        } = new Vector3(0, 80, 64);
        internal Matrix ViewMatrix
        {
            get
            {
                var below = new Vector3(Position.X, 0, Position.Z);
                // We'll create a rotation matrix using our angle
                var rotationMatrix = Matrix.CreateRotationY(mCameraAngle);
                var lookAtVector = below + Vector3.Transform(new Vector3(0, 0, -64), rotationMatrix);
                var upVector = Vector3.UnitY;

                return Matrix.CreateLookAt(
                    Position, lookAtVector, upVector);
            }
        }

        public Matrix ProjectionMatrix
        {
            get
            {
                const float fieldOfView = MathHelper.PiOver4;
                const int nearClipPlane = 1;
                const int farClipPlane = 1024;
                var aspectRatio = mGraphics.GraphicsDevice.Viewport.Width / (float)mGraphics.GraphicsDevice.Viewport.Height;

                return Matrix.CreatePerspectiveFieldOfView(
                    fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
            }
        }

        public Camera(GraphicsDeviceManager graphics)
        {
            mGraphics = graphics;
            mCameraAngle = 0f;
            mClippingSizeThreshold = 64;
            mCameraMovementHeightScaling = 0.01f;
        }


        public void MoveCameraRight(float speed)
        {
            var offset = new Vector3();
            offset.X += speed * Math.Abs(Position.Y * mCameraMovementHeightScaling);
            MoveCameraByOffset(offset);

        }

        public void MoveCameraDown(float speed)
        {
            var offset = new Vector3();
            offset.Z += speed * Math.Abs(Position.Y * mCameraMovementHeightScaling);
            MoveCameraByOffset(offset);
        }

        public void MoveCameraZoomIn(float speed)
        {
            var offset = new Vector3();
            offset.Y += speed;
            MoveCameraByOffset(offset);
        }

        private void MoveCameraByOffset(Vector3 offset)
        {
            Position += Vector3.Transform(offset, Matrix.CreateRotationY(mCameraAngle));
            ClipCamera();
        }
#if DEBUG
        [Conditional("FALSE")]
#endif
        private void ClipCamera()
        {
            Position = new Vector3(
                MathHelper.Clamp(Position.X, -mClippingSizeX + mClippingSizeThreshold, mClippingSizeX - mClippingSizeThreshold),
                MathHelper.Clamp(Position.Y, 50, 300),
                MathHelper.Clamp(Position.Z, -mClippingSizeZ + mClippingSizeThreshold*2, mClippingSizeZ));
        }

        public void SetClippingSize()
        {
            mClippingSizeX = GameState.Current.MapSize.X;
            mClippingSizeZ = GameState.Current.MapSize.Y;
        }
    }
}

