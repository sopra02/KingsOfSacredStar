using System;
using System.Collections.Generic;
using KingsOfSacredStar.GameLogic;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World.Unit.Buildings
{
    internal abstract class ADestroyableBuilding : ABuilding, IDamageableUnit
    {
        public int Health
        {
            get => mHitHelper.GetHealth();
            set => mHitHelper.RegisterHit(value);
        }

        public int MaxHealth { get; protected set; }
        public bool IsHit { get; set; }
        public bool ForRemoval => mHitHelper.ShouldDespawn;

        protected HitHelper mHitHelper;
        private int mTotalGameSeconds;

        protected ADestroyableBuilding(ModelManager.Model model, UnitTypes unitType, Players owner, Vector2 pos,  float rot, int gridSize, Point[] blockedFields = null, Point[] passableFields = null)
        : base(model, unitType, owner, pos, rot, gridSize, blockedFields, passableFields)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            mHitHelper.Update();
            UpdateEverySecond(gameTime);
        }

        public override string Serialize()
        {
            return base.Serialize() + " " + Health;
        }
        private void UpdateEverySecond(GameTime gameTime)
        {
            if ((int)gameTime.TotalGameTime.TotalSeconds == mTotalGameSeconds)
            {
                return;
            }

            if (Health != MaxHealth)
            {
                RegenerateHealth();
            }

            mTotalGameSeconds = (int)gameTime.TotalGameTime.TotalSeconds;
        }


        private void RegenerateHealth()
        {
            var enemiesInRange = EnemiesInRange();
            if (enemiesInRange.Count == 0)
            {
                Health += (int)Math.Min(MaxHealth, Math.Ceiling(MaxHealth * 0.001f));
            }
        }

        protected List<IUnit> EnemiesInRange()
        {
                var enemyDetectionRange = 6 * GridSize;
                var enemyPlayer = Owner == Players.Player ? Players.Ai : Players.Player;
                var unitsInRange = GameState.Current.SpatialUnitsByPlayer.UnitsInRange(enemyPlayer, Position, enemyDetectionRange);

                return unitsInRange;
        }
    }
}
