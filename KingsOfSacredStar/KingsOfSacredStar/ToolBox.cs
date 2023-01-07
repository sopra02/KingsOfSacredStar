using System;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar
{
    internal sealed class ToolBox
    {
        public static ToolBox Current { get; private set; }

        private readonly GraphicsDeviceManager mGraphics;
        public Camera Camera { get; }

        private ToolBox(GraphicsDeviceManager graphics, Camera camera)
        {
            Camera = camera;
            mGraphics = graphics;
        }

        public static void Initialize(GraphicsDeviceManager graphics, Camera camera)
        {
            if (Current != null)
            {
                throw new InvalidOperationException(Properties.Toolbox.Initialized);
            }
            Current = new ToolBox(graphics, camera);
        }

        public  Point ConvertScreenToGameFieldPosition(Point inputLastMouseClickPosition)
        {

            var nearScreenPoint = new Vector3(inputLastMouseClickPosition.X, inputLastMouseClickPosition.Y, 0);
            var farScreenPoint = new Vector3(inputLastMouseClickPosition.X, inputLastMouseClickPosition.Y, 1);

            var nearWorldPoint = mGraphics.GraphicsDevice.Viewport.Unproject(nearScreenPoint, Camera.ProjectionMatrix, Camera.ViewMatrix, Matrix.Identity);
            var farWorldPoint = mGraphics.GraphicsDevice.Viewport.Unproject(farScreenPoint, Camera.ProjectionMatrix, Camera.ViewMatrix, Matrix.Identity);

            var direction = farWorldPoint - nearWorldPoint;

            var zFactor = -nearWorldPoint.Y / direction.Y;
            var zeroWorldPoint = nearWorldPoint + direction * zFactor;

            return new Point((int)zeroWorldPoint.X, (int)zeroWorldPoint.Z);
        }
    }
}
