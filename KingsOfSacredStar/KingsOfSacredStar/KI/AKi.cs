using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.InputWrapper;
using KingsOfSacredStar.World;
using KingsOfSacredStar.World.Unit;
using KingsOfSacredStar.World.Unit.Buildings;
using KingsOfSacredStar.World.Unit.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KingsOfSacredStar.KI
{
    internal abstract class AKi
    {
        private static bool sRunning = true;

        protected static bool GetRunning()
        {
            return sRunning;
        }

        [Conditional("DEBUG")]
        public static void LoadInputManager(InputManager input)
        {
            input.AddOnKeyboard(Keys.K, () => sRunning = !sRunning);
        }

        private readonly Players mPlayerId;
        protected readonly Players[] mEnemies;

        protected AKi(Players playerId)
        {
            mPlayerId = playerId;
            mEnemies = new[] {Players.Player};
        }

        protected Vector2 GetVillagePos()
        {
            return GameState.Current.VillagePos[mPlayerId];
        }

        protected bool BuildWall(Vector2 position)
        {
            return GameState.Current.IsValidBuildingPlace(mPlayerId,
                       position,
                       UnitTypes.Wall,
                       0f) &&
            Costs.HasEnoughResourcesForUnit(UnitTypes.Wall, mPlayerId) &&
                   Costs.PayUnitCosts(UnitTypes.Wall, mPlayerId)  && GameState.Current.AddBuilding(
                       mPlayerId,
                       position,
                       UnitTypes.Wall);

        }
        protected bool BuildGate(Vector2 position, float rotation)
        {
            return GameState.Current.IsValidBuildingPlace(mPlayerId,
                       position,
                       UnitTypes.Gate,
                       rotation) &&
                   Costs.HasEnoughResourcesForUnit(UnitTypes.Gate, mPlayerId) &&
                   Costs.PayUnitCosts(UnitTypes.Gate, mPlayerId) && GameState.Current.AddBuilding(
                       mPlayerId,
                       position,
                       UnitTypes.Gate,
                       rotation);

        }


        protected IEnumerable<IAttackingUnit> GetAttackingUnits()
        {
            var units = new List<IAttackingUnit>();
            foreach (var unit in GameState.Current.UnitsByPlayer[mPlayerId])
            {
                if (unit is IAttackingUnit attackingUnit && !(unit is Hero))
                {
                    units.Add(attackingUnit);
                }
            }

            return units;
        }

        protected List<IDamageableUnit> GetEnemyUnits()
        {
            var units = new List<IDamageableUnit>();
            foreach (var enemy in mEnemies)
            {
                foreach (var unit in GameState.Current.UnitsByPlayer[enemy])
                {
                    if (unit is IDamageableUnit damageableUnit)
                    {
                        units.Add(damageableUnit);
                    }
                }

                foreach (var unit in GameState.Current.BuildingsByPlayer[enemy])
                {
                    if (unit is IDamageableUnit damageableUnit)
                    {
                        units.Add(damageableUnit);
                    }
                }
            }

            return units;

        }

        protected bool EnemyHasUnits()
        {
            return mEnemies.Any(enemy => GameState.Current.UnitsByPlayer[enemy].Count > 0);
        }

        protected bool AddUnits(UnitTypes unitType)
        {
            if (Costs.PayUnitCosts(unitType, mPlayerId))
            {
                GameState.Current.AddUnit(mPlayerId, GameState.Current.VillagePosOffset(mPlayerId), unitType);
                return true;
            }

            return false;
        }

        protected int GetGold()
        {
            return GameState.Current.Resources[mPlayerId][Resources.Gold];
        }

        protected int GetStone()
        {
            return GameState.Current.Resources[mPlayerId][Resources.Stone];
        }

        protected Hero GetHero()
        {
            return GameState.Current.HeroesByPlayer[mPlayerId];
        }

        protected List<Mine> GetGoldMines()
        {
            var minesEnemy = new List<Mine>();
            foreach (var building in GameState.Current.BuildingsByPlayer[Players.Global])
            {
                if (building is Mine m && m.UnitType == UnitTypes.GoldMine && m.Owner != mPlayerId)
                {
                    minesEnemy.Add(m);
                }
            }

            return minesEnemy;
        }

        protected List<Mine> GetQuarries()
        {
            var minesEnemy = new List<Mine>();
            foreach (var building in GameState.Current.BuildingsByPlayer[Players.Global])
            {
                if (building is Mine m && m.UnitType == UnitTypes.Quarry && m.Owner != mPlayerId)
                {
                    minesEnemy.Add(m);
                }
            }
            return minesEnemy;
        }

        protected SacredStar GetSacredStar()
        {
            foreach (var building in GameState.Current.BuildingsByPlayer[Players.Global])
            {
                if (building is SacredStar m && m.Owner != mPlayerId)
                {
                    return m;
                }
            }

            return null;
        }

        public abstract void Update(GameTime gameTime);
    }
}
