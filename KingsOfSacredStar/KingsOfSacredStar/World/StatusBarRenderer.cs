using System;
using System.Linq;
using KingsOfSacredStar.Screen;
using KingsOfSacredStar.World.Unit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.World
{
    internal sealed class StatusBarRenderer
    {
        private static readonly VertexDeclaration sInstanceVertexPositionDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0)
        );

        private static readonly VertexDeclaration sInstanceVertexPercentageDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1)
        );

        private readonly Camera mCamera;
        private readonly VertexBuffer mVertices;
        private readonly IndexBuffer mIndexBuffer;
        private readonly GraphicsDevice mDevice;
        private readonly Effect mHealthBarEffect;
        private DynamicVertexBuffer mInstanceVertexPositionBuffer;
        private DynamicVertexBuffer mInstanceVertexPercentageBuffer;

        public StatusBarRenderer(GraphicsDevice device, ContentManager content, Camera camera)
        {
            mCamera = camera;
            mDevice = device;
            mHealthBarEffect = content.Load<Effect>("effects/InstancedStatusBar");
            mHealthBarEffect.CurrentTechnique = mHealthBarEffect.Techniques["HardwareInstancing"];

            mVertices = new VertexBuffer(device, new VertexDeclaration(
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0)
                ), 4, BufferUsage.WriteOnly);

            mVertices.SetData(new []
            {
                new Vector3(0, .5f, -.5f),
                new Vector3(1, .5f, -.5f),
                new Vector3(0, -.5f, .5f),
                new Vector3(1, -.5f, .5f)
            });

            mIndexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, 6, BufferUsage.WriteOnly);
            mIndexBuffer.SetData(new short[] {0, 1, 2, 1, 3, 2});
        }

        public void DrawReversed<T>(Color barColor, Func<T, float> percentageMapping, params T[] units) where T : IUnit
        {
            Draw(Matrix.CreateScale(-1, -1, -1), new Vector3(-GameScreen.GridSize / 4f, 1, -GameScreen.GridSize / 2f),  barColor, percentageMapping, units);
        }

        public void Draw<T>(Color barColor, Func<T, float> percentageMapping, params T[] units) where T : IUnit
        {
            Draw(Matrix.Identity, new Vector3(-GameScreen.GridSize * (3f / 4), 1, -GameScreen.GridSize / 2f), barColor, percentageMapping, units);
        }

        private void Draw<T>(Matrix worldMatrix, Vector3 offset, Color barColor, Func<T, float> percentageMapping, params T[] units) where T : IUnit
        {
            if (units.Length ==  0)
            {
                return;
            }
            var renderPosition = units.Select(unit => new Vector3(unit.Position.X, unit.ModelType.Height + 1, unit.Position.Y) + offset).ToArray();
            RenderHelper.SetVertices(mDevice, ref mInstanceVertexPositionBuffer, sInstanceVertexPositionDeclaration, renderPosition);
            var healthRatio = units.Select(percentageMapping).ToArray();
            RenderHelper.SetVertices(mDevice, ref mInstanceVertexPercentageBuffer, sInstanceVertexPercentageDeclaration, healthRatio);

            mDevice.SetVertexBuffers(
                new VertexBufferBinding(mVertices, 0, 0),
                new VertexBufferBinding(mInstanceVertexPositionBuffer, 0, 1),
                new VertexBufferBinding(mInstanceVertexPercentageBuffer, 0, 1)
            );

            mDevice.Indices = mIndexBuffer;

            mHealthBarEffect.Parameters["World"].SetValue(worldMatrix);
            mHealthBarEffect.Parameters["View"].SetValue(mCamera.ViewMatrix);
            mHealthBarEffect.Parameters["Projection"].SetValue(mCamera.ProjectionMatrix);
            // False positive
            // ReSharper disable once ImpureMethodCallOnReadonlyValueField
            mHealthBarEffect.Parameters["Color"].SetValue(barColor.ToVector4());

            // Draw all the instance copies in a single call.
            foreach (var pass in mHealthBarEffect.CurrentTechnique.Passes)
            {
                pass.Apply();

                mDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, mVertices.VertexCount, units.Length);
            }
        }
    }
}
