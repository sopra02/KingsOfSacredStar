using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.World
{
    internal sealed class PlaneRenderer
    {

        private readonly VertexPositionNormalTexture[] mPlaneVertex = new VertexPositionNormalTexture[6];

        private readonly Effect mEffect;

        private readonly GraphicsDeviceManager mGraphics;

        private readonly Camera mCamera;

        public PlaneRenderer(GraphicsDeviceManager graphics, Camera camera, Effect effect)
        {
            mGraphics = graphics;
            mCamera = camera;
            mEffect = effect;
        }

        public void Draw(Vector2 position = new Vector2(), float scale = 1f)
        {
            mGraphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            mGraphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            var world = Matrix.CreateScale(scale, 1, scale) * Matrix.CreateTranslation(position.X, 0, position.Y);
            if (mEffect is BasicEffect basicEffect)
            {
                basicEffect.View = mCamera.ViewMatrix;
                basicEffect.Projection = mCamera.ProjectionMatrix;
                basicEffect.World = world;
            }
            else
            {
                mEffect.Parameters["View"].SetValue(mCamera.ViewMatrix);
                mEffect.Parameters["Projection"].SetValue(mCamera.ProjectionMatrix);
                mEffect.Parameters["World"].SetValue(world);
            }


            foreach (var pass in mEffect.CurrentTechnique.Passes)
            {

                pass.Apply();

                mGraphics.GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList,
                    mPlaneVertex,
                    0,
                    2);


            }
        }
        internal void InitializePlane(float sizeX, float posY, float sizeZ, float repetitionsX = 1f, float repetitionsY = 1f)
        {
            mPlaneVertex[0].Position = new Vector3(-sizeX, posY, -sizeZ);
            mPlaneVertex[1].Position = new Vector3(sizeX, posY, -sizeZ);
            mPlaneVertex[2].Position = new Vector3(-sizeX, posY, sizeZ);

            mPlaneVertex[3].Position = mPlaneVertex[1].Position;
            mPlaneVertex[4].Position = new Vector3(sizeX, posY, sizeZ);
            mPlaneVertex[5].Position = mPlaneVertex[2].Position;

            mPlaneVertex[0].TextureCoordinate = new Vector2(0, 0);
            mPlaneVertex[1].TextureCoordinate = new Vector2(repetitionsX, 0);
            mPlaneVertex[2].TextureCoordinate = new Vector2(0, repetitionsY);

            mPlaneVertex[3].TextureCoordinate = mPlaneVertex[1].TextureCoordinate;
            mPlaneVertex[4].TextureCoordinate = new Vector2(repetitionsX, repetitionsY);
            mPlaneVertex[5].TextureCoordinate = mPlaneVertex[2].TextureCoordinate;
        }
    }
}
