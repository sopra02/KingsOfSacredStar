using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.World
{
    internal sealed class BoundingBoxManager
    {
        private readonly GraphicsDevice mDevice;
        private readonly Camera mCamera;
        private readonly BasicEffect mLineEffect;
        private readonly VertexBuffer mVertices;
        private readonly IndexBuffer mIndices;
        private const int PrimitiveCount = 24;


        public BoundingBoxManager(GraphicsDevice device, Camera camera)
        {
            mDevice = device;
            mCamera = camera;
            mLineEffect = new BasicEffect(mDevice)
            {
                LightingEnabled = false, TextureEnabled = false, VertexColorEnabled = true
            };
            const int vertexCount = 48;

            mVertices = new VertexBuffer(mDevice, typeof(VertexPositionColor), vertexCount, BufferUsage.WriteOnly);

            mIndices = new IndexBuffer(mDevice, IndexElementSize.SixteenBits, vertexCount, BufferUsage.WriteOnly);
            mIndices.SetData(new short[] { 0, 1, 1, 2, 2, 3, 3, 0, 4, 5, 5, 6, 6, 7, 7, 4, 0, 4, 1, 5, 2, 6, 3, 7 });
        }

        public void DrawBoundingBoxes(ModelManager.Model model, params Matrix[] transformations)
        {
            FillBoundingBoxBuffers(model.GetBoundingBox(Matrix.Identity));
            DrawBoundingBoxes(mCamera.ViewMatrix, mCamera.ProjectionMatrix, transformations);
        }

        private void DrawBoundingBoxes(Matrix view, Matrix projection, params Matrix[] transformations)
        {
            mDevice.SetVertexBuffer(mVertices);
            mDevice.Indices = mIndices;
            mLineEffect.View = view;
            mLineEffect.Projection = projection;

            foreach (var world in transformations)
            {
                mLineEffect.World = world;

                foreach (var pass in mLineEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    mDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, PrimitiveCount);
                }
            }
        }


        private static Vector3[] GetVertexElement(ModelMeshPart meshPart, VertexElementUsage usage)
        {
            var vd = meshPart.VertexBuffer.VertexDeclaration;
            var elements = vd.GetVertexElements();

            bool ElementPredicate(VertexElement ve) => ve.VertexElementUsage == usage && ve.VertexElementFormat == VertexElementFormat.Vector3;
            if (!elements.Any(ElementPredicate))
                return new Vector3[0];

            var element = elements.First(ElementPredicate);

            var vertexData = new Vector3[meshPart.NumVertices];
            meshPart.VertexBuffer.GetData((meshPart.VertexOffset * vd.VertexStride) + element.Offset,
                vertexData, 0, vertexData.Length, vd.VertexStride);

            return vertexData;
        }

        private static BoundingBox? GetBoundingBox(ModelMeshPart meshPart, Matrix transform)
        {
            if (meshPart.VertexBuffer == null)
                return null;

            var positions = GetVertexElement(meshPart, VertexElementUsage.Position);
            if (positions.Length == 0)
                return null;

            var transformedPositions = new Vector3[positions.Length];
            Vector3.Transform(positions, ref transform, transformedPositions);

            return BoundingBox.CreateFromPoints(transformedPositions);
        }

        public static BoundingBox CreateBoundingBox(Model model, Vector2 offset)
        {
            var boundingBoxes = CreateAllBoundingBoxes(model, offset);

            return boundingBoxes.Aggregate(BoundingBox.CreateMerged);
        }


        public static IEnumerable<BoundingBox> CreateAllBoundingBoxes(Model model, Vector2 offset)
        {
            var boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            var offset3D = new Vector3(offset.X, 0, offset.Y);

            var allBoundingBoxes = new List<BoundingBox> ();
            foreach (var mesh in model.Meshes)
            {
                var result = mesh.MeshParts
                    .Select(meshPart => GetBoundingBox(meshPart, boneTransforms[mesh.ParentBone.Index]))
                    .Where(boundingBox => boundingBox.HasValue)
                    .Select(boundingBox => boundingBox.Value)
                    .Aggregate(BoundingBox.CreateMerged);
                result.Min += offset3D;
                result.Max += offset3D;

                allBoundingBoxes.Add(result);
            }

            return allBoundingBoxes;
        }

        private void FillBoundingBoxBuffers(OrientedBoundingBox boundingBox)
        {
            mVertices.SetData(boundingBox.GetCorners().Select(position => new VertexPositionColor(position, Color.White)).ToArray());
        }
    }
}
