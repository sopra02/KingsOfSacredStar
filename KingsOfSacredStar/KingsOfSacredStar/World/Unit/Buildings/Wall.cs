using KingsOfSacredStar.GameLogic;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World.Unit.Buildings
{
    internal sealed class Wall : ADestroyableBuilding
    {
        public Wall(Players owner, Vector2 pos, float rot, int gridSize) :
            base(ModelManager.GetInstance().Wall, UnitTypes.Wall, owner, pos, rot, gridSize)
        {
            MaxHealth = (int)BaseStats.sUnitStats[UnitType][StatNames.Health];
            mHitHelper = new HitHelper(this, MaxHealth);
        }
    }
}
