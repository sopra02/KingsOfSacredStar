using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World.Unit
{
    internal interface IMovableUnit : IUnit
    {
        BoundingBox GetBoundingBox();

        /// <summary>
        /// Updates the inner state of this Unit to track
        /// the given target.
        /// </summary>
        /// <param name="target">The position of the target</param>
        /// <returns>False if the target is unreachable, true otherwise</returns>
        Task<bool> SetMovingTarget(Vector2? target);
        void Reposition(Vector2 direction, bool moving, bool canBeIgnored);
        void WallCollision();
    }
}
