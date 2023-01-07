using System;
using System.Linq;
using KingsOfSacredStar.GameLogic;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World.Unit.Units
{
    internal sealed class Arrow : AUnit, IDespawningUnit
    {
        public bool ForRemoval => Position == mTargetPosition;

        private readonly Vector2 mTargetPosition;
        private readonly int mGridSize;
        private readonly float mMovementSpeed;
        private readonly int mAttackDamage;
        private readonly Vector2 mStartPosition;

        public Arrow(Players owner, Vector2 pos, float rotation, int gridSize, Vector2 targetPosition)
            : base(ModelManager.GetInstance().Arrow, UnitTypes.Arrow, owner, pos, gridSize, rotation)
        {
            mGridSize = gridSize;
            mTargetPosition = targetPosition;
            mStartPosition = Position;
            mMovementSpeed = BaseStats.sUnitStats[UnitTypes.Arrow][StatNames.Speed];
            mAttackDamage = (int) BaseStats.sUnitStats[UnitTypes.Arrow][StatNames.BaseDamage];
            if (Math.Abs(rotation) < float.Epsilon && !ForRemoval)
            {
                var direction = targetPosition - Position;
                Rotation = (float) Math.Atan2(direction.X, direction.Y);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (ForRemoval)
            {
                return;
            }

            if (Vector2.Distance(Position, mTargetPosition) <= mMovementSpeed)
            {
                Position = mTargetPosition;
                foreach (var player in PlayerConstants.sPlayers)
                {
                    if (player == Players.Global || player == Owner)
                    {
                        continue;
                    }
                    var targets = GameState.Current.SpatialUnitsByPlayer.UnitsInRange(player, Position, mGridSize / 2f);
                    foreach (var damageableUnit in targets.OfType<IDamageableUnit>())
                    {
                        damageableUnit.Health =
                            MathHelper.Clamp(damageableUnit.Health - (int)(mAttackDamage * GameState.Current.mDamageFactor[Owner]), 0, damageableUnit.MaxHealth);
                    }
                }
            }
            else
            {
                var direction = mTargetPosition - Position;
                direction.Normalize();
                Position += direction * mMovementSpeed;
            }
        }

        private float CalcHeight()
        {
            var halfDistance = Vector2.Distance(mTargetPosition, mStartPosition) / 2;
            var x = Vector2.Distance(Position, mStartPosition) - halfDistance;
            return (float) ((-Math.Pow(x, 2) + Math.Pow(halfDistance, 2)) / Math.Pow(halfDistance, 2)) * mGridSize * 5;
        }

        private float CalcTilt()
        {
            var halfDistance = Vector2.Distance(mTargetPosition, mStartPosition) / 2;
            var x = Vector2.Distance(Position, mStartPosition) - halfDistance;
            return (float) ((-2 * x) / (Math.Pow(halfDistance, 2) / (mGridSize * 5)));
        }

        public override Matrix RenderPosition => 
            Matrix.CreateTranslation(mGridSize / 4f, mGridSize / -2f, 0) *
            Matrix.CreateRotationX((float) -Math.Atan(CalcTilt())) *
            Matrix.CreateTranslation(mGridSize / -4f, mGridSize / 2f, 0) *
            base.RenderPosition *
            Matrix.CreateTranslation(0, CalcHeight(), 0);
    }
}
