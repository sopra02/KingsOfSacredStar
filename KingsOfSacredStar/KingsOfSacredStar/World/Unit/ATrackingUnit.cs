using System.Linq;
using System.Threading.Tasks;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.Screen;
using KingsOfSacredStar.World.Unit.Buildings;
using KingsOfSacredStar.World.Unit.Units;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World.Unit
{
    internal abstract class ATrackingUnit : AMovableUnit, IAttackingUnit
    {

        private IDamageableUnit mTarget;
        protected IDamageableUnit mLastAttacked;
        private bool mFollow;
        private Vector2 mLastTargetPos;
        private int mAttackRange;
        protected int mBaseDamage;
        private int mDamageInterval;
        private int mDamageTimeout;


        protected ATrackingUnit(ModelManager.Model model,
            UnitTypes unitType,
            Players owner,
            Vector2 pos,
            float rot,
            int gridSize)
            : base(model, unitType, owner, pos, rot, gridSize) {}

        protected void SetBaseStatsTrackingUnit()
        {
            mAttackRange = (int)BaseStats.sUnitStats[UnitType][StatNames.AttackRange];
            mSpeed = BaseStats.sUnitStats[UnitType][StatNames.Speed];
            mBaseDamage = (int)BaseStats.sUnitStats[UnitType][StatNames.BaseDamage];
            mDamageInterval = (int)BaseStats.sUnitStats[UnitType][StatNames.DamageInterval];
        }

        public int GetRange() => mAttackRange;

        public bool HasTarget()
        {
            return mTarget != null;
        }

        public void SetTarget(IDamageableUnit target, bool follow)
        {
            if (target != null && follow)
            {
                mLastTargetPos = target.Position;
                base.SetMovingTarget(target.Position);
            }

            mTarget = target;
            mFollow = follow;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (mTarget == null || mTarget.Health <= 0 || (mTarget is IDespawningUnit despawningUnit && despawningUnit.ForRemoval))
            {
                mTarget = null;
                return;
            }

            if (Vector2.Distance(mTarget.Position, mLastTargetPos) >= mAttackRange && mFollow)
            {
                mLastTargetPos = mTarget.Position;
                base.SetMovingTarget(mTarget.Position);
            }

            var canAttack = Vector2.Distance(mTarget.Position, Position) <= mAttackRange;

            if (mTarget is ABuilding building)
            {
                if (building.BlockedFields()
                    .Select(gridPosition => gridPosition.ToVector2() * GameScreen.GridSize)
                    .Any(position => Vector2.Distance(position, Position) - 2 * GameScreen.GridSize <= mAttackRange))
                {
                    canAttack = true;
                }
            }

            if (canAttack)
            {
                DealDamage();
            }
        }

        private void DealDamage()
        {
            if (mDamageTimeout == 0)
            {
                var damage = mBaseDamage *
                             AttackModifiers.GetModifier(UnitType, mTarget.UnitType) *
                             GameState.Current.mDamageFactor[Owner];

                if (damage > 0)
                {
                    mTarget.Health = MathHelper.Clamp(mTarget.Health - (int)damage, 0, mTarget.MaxHealth);
                }
                mDamageTimeout++;
                mLastAttacked = mTarget;
            }
            else if (mDamageTimeout == mDamageInterval)
            {
                mDamageTimeout = 0;
            }
            else
            {
                mDamageTimeout++;
            }
        }

        public override Task<bool> SetMovingTarget(Vector2? target)
        {
            mTarget = null;
            return base.SetMovingTarget(target);
        }
    }
}
