using System.Threading.Tasks;
using KingsOfSacredStar.GameLogic;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World.Unit.Units
{
    internal sealed class Bowman : AMovableUnit, IAttackingUnit, IDamageableUnit
    {
        private readonly int mAttackRange;
        private readonly int mDamageInterval;
        private readonly int mGridSize;
        private IDamageableUnit mTarget;
        private Vector2 mTargetPosition;
        private bool mFollow;
        private int mAttackTimeout;
        public int MaxHealth { get; }
        public int Health
        {
            get => mHitHelper.GetHealth();
            set => mHitHelper.RegisterHit(value);
        }
        public bool IsHit { get; set; }
        public bool ForRemoval => mHitHelper.ShouldDespawn;

        private readonly HitHelper mHitHelper;

        public Bowman(Players owner, Vector2 pos, float rotation, int gridSize)
            : base(ModelManager.GetInstance().Bowman, UnitTypes.Bowman, owner, pos, rotation, gridSize)
        {
            mGridSize = gridSize;

            MaxHealth = (int) BaseStats.sUnitStats[UnitTypes.Bowman][StatNames.Health];
            mAttackRange = (int)BaseStats.sUnitStats[UnitTypes.Bowman][StatNames.AttackRange];
            mDamageInterval = (int)BaseStats.sUnitStats[UnitTypes.Bowman][StatNames.DamageInterval];
            mSpeed = BaseStats.sUnitStats[UnitTypes.Bowman][StatNames.Speed];
            mHitHelper = new HitHelper(this, MaxHealth);
        }



        public override void Update(GameTime gameTime)
        {
            mHitHelper.Update();
            if (mTarget == null)
            {
                base.Update(gameTime);
                return;
            }

            if (mTarget.Health <= 0 || (mTarget is IDespawningUnit despawningUnit && despawningUnit.ForRemoval))
            {
                mTarget = null;
                return;
            }
            if (Vector2.Distance(mTarget.Position, Position) >= mAttackRange)
            {
                base.Update(gameTime);
                if (mFollow && Vector2.Distance(mTarget.Position, mTargetPosition) >= mAttackRange)
                {
                    mTargetPosition = mTarget.Position;
                    base.SetMovingTarget(mTarget.Position);
                }
            }
            else
            {
                if (mFollow)
                {
                    base.SetMovingTarget(null);
                }
                if (mAttackTimeout == 0)
                {
                    var arrow = new Arrow(Owner, Position / mGridSize, 0, mGridSize, mTarget.Position);
                    GameState.Current.mUnitsToAddNextTick.Enqueue(arrow);
                    mAttackTimeout++;
                }
                else if (mAttackTimeout == mDamageInterval)
                {
                    mAttackTimeout = 0;
                }
                else
                {
                    mAttackTimeout++;
                }
            }
        }

        public void SetTarget(IDamageableUnit target, bool follow)
        {
            mTarget = target;
            mTargetPosition = target.Position;
            if (follow)
            {
                base.SetMovingTarget(mTargetPosition);
            }

            mFollow = follow;
        }

        public override Task<bool> SetMovingTarget(Vector2? target)
        {
            mTarget = null;
            return base.SetMovingTarget(target);
        }

        public int GetRange() => mAttackRange;

        public bool HasTarget() => mTarget != null;

        public override string Serialize()
        {
            return base.Serialize() + " " + Health;
        }
    }
}
