using KingsOfSacredStar.GameLogic;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World.Unit.Units
{
    internal sealed class BatteringRam : ATrackingUnit, IDamageableUnit
    {
        public int Health
        {
            get => mHitHelper.GetHealth();
            set => mHitHelper.RegisterHit(value);
        }
        public int MaxHealth { get; }
        public bool IsHit { get; set; }
        public bool ForRemoval => mHitHelper.ShouldDespawn;

        private readonly HitHelper mHitHelper;

        public BatteringRam(Players owner, Vector2 pos, float rot, int gridSize)
            : base(ModelManager.GetInstance().BatteringRam, UnitTypes.BatteringRam, owner, pos, rot, gridSize)
        {
            MaxHealth = (int) BaseStats.sUnitStats[UnitType][StatNames.Health];
            mHitHelper = new HitHelper(this, MaxHealth);
            SetBaseStatsTrackingUnit();
        }

        public override void Update(GameTime gameTime)
        {
            mHitHelper.Update();
            base.Update(gameTime);
        }

        public override string Serialize()
        {
            return base.Serialize() + " " + Health;
        }
    }
}
