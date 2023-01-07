using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.World
{
    internal static class RenderHelper
    {
        public static void SetVertices<T>(GraphicsDevice device, ref DynamicVertexBuffer buffer, VertexDeclaration declaration, T[] data) where T : struct
        {

            // If we have more instances than room in our vertex buffer, grow it to the necessary size.
            if (buffer == null || data.Length > buffer.VertexCount)
            {
                buffer?.Dispose();

                buffer = new DynamicVertexBuffer(device, declaration, data.Length, BufferUsage.WriteOnly);
            }

            // Transfer the latest instance transform matrices into the instanceVertexBuffer.
            buffer.SetData(data, 0, data.Length, SetDataOptions.Discard);
        }
    }
}
