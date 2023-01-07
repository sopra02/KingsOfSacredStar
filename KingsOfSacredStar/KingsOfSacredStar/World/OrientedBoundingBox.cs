using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World
{
    internal sealed class OrientedBoundingBox
    {
        private readonly Vector3[] mCorners;

        public OrientedBoundingBox(BoundingBox alignedBoundingBox, Matrix transformation)
        {
            mCorners = alignedBoundingBox.GetCorners();
            for (var i = 0; i < mCorners.Length; i++)
            {
                mCorners[i] = Vector3.Transform(mCorners[i], transformation);
            }

        }

        public Vector3[] GetCorners()
        {
            return mCorners;
        }
    }
}
