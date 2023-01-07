using System.Linq;
using KingsOfSacredStar.GameLogic;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World.Unit.Buildings
{
    internal abstract class ABuilding : AUnit
    {
        private readonly Point[] mBlockedFields;
        private readonly Point[] mPassableFields;
        private readonly Vector2 mGridPos;

        protected ABuilding(ModelManager.Model model,
            UnitTypes unitType,
            Players owner,
            Vector2 pos,
            float rot,
            int gridSize,
            Point[] blockedFields = null,
            Point[] passableFields = null)
            : base(model, unitType, owner, pos, gridSize, rot)
        {
            mBlockedFields = blockedFields ?? new []{ new Point(0, 0) };
            mPassableFields = passableFields ?? new Point[0];
            mGridPos = pos;
        }

        public override void Update(GameTime gameTime)
        {
            // Nothing to update, except for animations maybe
        }

        public Point[] BlockedFields()
        {
            var rotationMatrix = Matrix.CreateRotationZ(Rotation);

            return mBlockedFields
                .Select(p => p.ToVector2())
                .Select(v => Vector2.Transform(v, rotationMatrix))
                .Select(v => v + mGridPos)
                .Select(v => v.ToPoint())
                .ToArray();
        }

        public Point[] PassableFields()
        {
            var rotationMatrix = Matrix.CreateRotationZ(Rotation);

            return mPassableFields
                .Select(p => p.ToVector2())
                .Select(v => Vector2.Transform(v, rotationMatrix))
                .Select(v => v + mGridPos)
                .Select(v => v.ToPoint())
                .ToArray();

        }
    }
}
