using KingsOfSacredStar.GameLogic;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World.Unit.Buildings
{
    internal sealed class Rock : ABuilding
    {
        public Rock(Vector2 pos, int gridSize, float rotation)
            : base(ModelManager.GetInstance().Rock, UnitTypes.Rock, Players.Global, pos, rotation, gridSize) {}

        public override void Update(GameTime gameTime)
        {
            // This Unit does absolutely nothing.
        }
    }
}
