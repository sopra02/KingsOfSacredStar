using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World
{
    internal sealed class Trapeze
    {
        private readonly Vector2 mTopLeft;
        private readonly Vector2 mTopRight;
        private readonly Vector2 mBottomLeft;
        private readonly Vector2 mBottomRight;
        public Trapeze(Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight)
        {
            Debug.Assert(Math.Abs(topLeft.Y - topRight.Y) < 0.01, "Top coordinates are not equal!");
            Debug.Assert(Math.Abs(bottomLeft.Y - bottomRight.Y) < 0.01, "Bottom coordinates are not equal!");
            Debug.Assert(topLeft.X <= topRight.X);
            Debug.Assert(bottomLeft.X <= bottomRight.X);
            Debug.Assert(bottomLeft.Y >= topLeft.Y);
            mTopLeft = topLeft;
            mTopRight = topRight;
            mBottomLeft = bottomLeft;
            mBottomRight = bottomRight;
        }
        public bool Contains(Vector2 point)
        {
            if (point.Y >= mTopLeft.Y && point.Y <= mBottomLeft.Y)
            {
                var normalizedPointLeft = point - mTopLeft;
                var normalizedPointRight = point - mTopRight;
                var leftSide = mBottomLeft - mTopLeft;
                var rightSide = mBottomRight - mTopRight;
                if (((leftSide / leftSide.Y) * normalizedPointLeft.Y).X <= normalizedPointLeft.X
                    && ((rightSide / rightSide.Y) * normalizedPointRight.Y).X >= normalizedPointRight.X)
                    return true;
            }
            return false;
        }
    }
}
