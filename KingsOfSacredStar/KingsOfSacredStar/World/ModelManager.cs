using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.World
{
    internal sealed class ModelManager
    {
        public enum RenderingEffect
        {
            None,
            Selected,
            Hit
        }
        public struct RenderingInformation
        {
            public Matrix Transformation { get;}
            public Color Color { get; }
            public RenderingEffect Effect { get; }

            public RenderingInformation(Matrix transformation, Color color, RenderingEffect effect = RenderingEffect.None)
            {
                Transformation = transformation;
                Color = color;
                Effect = effect;
            }
        }

        internal sealed class Model
        {
            private readonly Microsoft.Xna.Framework.Graphics.Model mModel;
            private readonly Camera mCamera;
            private readonly GraphicsDevice mDevice;
            private DynamicVertexBuffer mInstanceVertexBuffer;
            private DynamicVertexBuffer mInstanceColorBuffer;
            private DynamicVertexBuffer mInstanceEffectBuffer;
            private readonly BoundingBoxManager mBoundingBoxManager;
            private readonly BoundingBox mBoundingBox;
            public float Height => Math.Abs(mBoundingBox.Max.Y - mBoundingBox.Min.Y);


            // To store instance transform matrices in a vertex buffer, we use this custom
            // vertex type which encodes 4x4 matrices as a set of four Vector4 values.
            private static readonly VertexDeclaration sInstanceVertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
                new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
                new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
                new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
            );

            private static readonly VertexDeclaration sInstanceColorDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 4)
            );

            private static readonly VertexDeclaration sInstanceEffectDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Short2, VertexElementUsage.TextureCoordinate, 5)
            );

            internal Model(GraphicsDevice device, ContentManager content, string name, Camera camera)
            {
                mDevice = device;
                mModel = content.Load<Microsoft.Xna.Framework.Graphics.Model>(name);
                mCamera = camera;
                mBoundingBoxManager = new BoundingBoxManager(device, camera);
                mBoundingBox = BoundingBoxManager.CreateBoundingBox(mModel, new Vector2());

                foreach (var mesh in mModel.Meshes)
                {
                    foreach (var meshPart in mesh.MeshParts)
                    {
                        var effect = content.Load<Effect>("effects/InstancedModel").Clone();

                        effect.Parameters["LightDirection"].SetValue(new Vector3(0, -1, -1));
                        effect.Parameters["DiffuseLight"].SetValue(Vector3.One * 1.25f);
                        effect.Parameters["AmbientLight"].SetValue(Vector3.One * 0.25f);
                        effect.Parameters["Texture"].SetValue(meshPart.Effect.Parameters["Texture"].GetValueTexture2D());
                        meshPart.Effect = effect;
                    }
                }
            }

            public void Draw(GameTime gameTime, params RenderingInformation[] renderingInformation)
            {
                if (renderingInformation.Length == 0)
                {
                    return;
                }
                mDevice.SamplerStates[0] = SamplerState.PointClamp;

                var transformations = renderingInformation.Select(info => info.Transformation).ToArray();

                RenderHelper.SetVertices(mDevice, ref mInstanceVertexBuffer, sInstanceVertexDeclaration, transformations);

                RenderHelper.SetVertices(mDevice, ref mInstanceColorBuffer, sInstanceColorDeclaration, renderingInformation.Select(info => info.Color.ToVector4()).ToArray());

                RenderHelper.SetVertices(mDevice, ref mInstanceEffectBuffer, sInstanceEffectDeclaration, renderingInformation.Select(info => (int) info.Effect).ToArray());

                foreach (var mesh in mModel.Meshes)
                {
                    foreach (var meshPart in mesh.MeshParts)
                    {

                        // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                        mDevice.SetVertexBuffers(
                            new VertexBufferBinding(meshPart.VertexBuffer, meshPart.VertexOffset, 0),
                            new VertexBufferBinding(mInstanceVertexBuffer, 0, 1),
                            new VertexBufferBinding(mInstanceColorBuffer, 0, 1),
                            new VertexBufferBinding(mInstanceEffectBuffer, 0, 1)
                        );

                        mDevice.Indices = meshPart.IndexBuffer;

                        // Set up the instance rendering effect.
                        var effect = meshPart.Effect;

                        effect.CurrentTechnique = effect.Techniques["HardwareInstancing"];

                        effect.Parameters["World"].SetValue(Matrix.Identity);
                        effect.Parameters["View"].SetValue(mCamera.ViewMatrix);
                        effect.Parameters["Projection"].SetValue(mCamera.ProjectionMatrix);
                        effect.Parameters["HighlightOffset"].SetValue(gameTime.TotalGameTime.Ticks / 400000f);

                        // Draw all the instance copies in a single call.
                        foreach (var pass in effect.CurrentTechnique.Passes)
                        {
                            pass.Apply();

                            mDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, meshPart.StartIndex,
                                meshPart.PrimitiveCount, renderingInformation.Length);
                        }
                    }

                }
                DrawModelBoundingBoxes(transformations);
            }

            [Conditional("DEBUG")]
            private void DrawModelBoundingBoxes(Matrix[] transformations)
            {

                mBoundingBoxManager.DrawBoundingBoxes(this, transformations);
            }

            public OrientedBoundingBox GetBoundingBox(Matrix transformation)
            {
                return new OrientedBoundingBox(mBoundingBox, transformation);
            }

            public float? PickingRayIntersection(Ray pickingRay, Matrix transformation)
            {
                var orientedBoundingBox = GetBoundingBox(transformation);
                var alignedBoundingBox = BoundingBox.CreateFromPoints(orientedBoundingBox.GetCorners());
                var intersectionDistance = pickingRay.Intersects(alignedBoundingBox);
                return intersectionDistance;
            }

            public bool PickingRayHittingMeshes(Ray pickingRay, Vector2 position)
            {
                var modelMeshesBoundingBoxes = BoundingBoxManager.CreateAllBoundingBoxes(mModel, position);

                return modelMeshesBoundingBoxes.Any(box => pickingRay.Intersects(box).HasValue);
            }
        }

        private static ModelManager sInstance;

        public Model Rock { get; }
        public Model Gate { get; }
        public Model GateClosed { get; }
        public Model Wall { get; }
        public Model Arrow { get; }
        public Model SacredStar { get; }
        public Model Village { get; }
        public Model GoldMine { get; }
        public Model Quarry { get; }
        public Model Swordsman { get; }
        public Model Bowman { get; }
        public Model BatteringRam { get; }
        public Model Cavalry { get; }
        public Model Hero { get; }

        private ModelManager(GraphicsDevice device, ContentManager content, Camera camera)
        {
            Gate = new Model(device, content, "models/Gate", camera);
            GateClosed = new Model(device, content, "models/Gate_Closed", camera);
            Arrow = new Model(device, content, "models/Arrow", camera);
            SacredStar = new Model(device, content, "models/Sacred_Star", camera);
            Village = new Model(device, content, "models/Village", camera);
            GoldMine = new Model(device, content, "models/Gold_Mine", camera);
            Quarry = new Model(device, content, "models/Quarry", camera);
            Swordsman = new Model(device, content, "models/Swordsman", camera);
            Bowman = new Model(device, content, "models/Bowman", camera);
            BatteringRam = new Model(device, content, "models/BatteringRam", camera);
            Cavalry = new Model(device, content, "models/Cavalry", camera);
            Hero = new Model(device, content, "models/Hero", camera);
            Wall = new Model(device, content, "models/Wall", camera);
            Rock = new Model(device, content, "models/Rock", camera);
        }

        public static void Initialize(GraphicsDevice device, ContentManager content, Camera camera)
        {
            if (sInstance != null)
            {
                throw new InvalidOperationException(Properties.ModelManager.Initialized);
            }
            sInstance = new ModelManager(device, content, camera);
        }

        public static ModelManager GetInstance()
        {
            if (sInstance == null)
            {
                throw new InvalidOperationException(Properties.ModelManager.NotInitialized);
            }

            return sInstance;
        }
    }
}
