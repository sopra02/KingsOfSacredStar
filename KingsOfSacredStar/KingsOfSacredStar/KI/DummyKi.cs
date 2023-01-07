using System;
using System.Collections.Generic;
using System.Linq;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.World;
using KingsOfSacredStar.World.Unit;
using KingsOfSacredStar.World.Unit.Buildings;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.KI
{
    internal sealed class DummyKi : AKi
    {
        private readonly UnitTypes[] mToMake = { UnitTypes.Bowman, UnitTypes.Cavalry, UnitTypes.Swordsman };
        private UnitTypes? mNextUnitType;
        private static readonly Random sRandom = new Random();
        private int mRingX;
        private int mRingY;
        private const int MinimumBuildRange = 7;
        private int mCurrentBuildRange;
        private bool mBuildLocationPreparedForGate;

        public DummyKi() : base(Players.Ai)
        {
            mCurrentBuildRange = MinimumBuildRange;
        }
        private Vector2 GetHeroTarget(IReadOnlyList<Mine> primMines, IReadOnlyList<Mine> secMines)
        {
            if (primMines.Count > 0)
            {
                return primMines[0].Position;
            }
            return secMines.Count > 0 ? secMines[0].Position : GetSacredStar().Position;
        }

        private Point GetBuildLocation()
        {
            var buildingPos = GetVillagePos() + new Vector2(mRingX, mRingY) -
                              new Vector2(mCurrentBuildRange / 2f, mCurrentBuildRange / 2f);
            return buildingPos.ToPoint();
        }

        private void BuildLogic()
        {
            if (!IsGateLocation())
            {
                if (BuildWall(GetBuildLocation().ToVector2()))
                {
                    CalculateNewRingBuildPosition();
                }
            }
            else if (mBuildLocationPreparedForGate)
            {
                if (BuildGate(GetBuildLocation().ToVector2(), CalculateGateRotation()))
                {
                    CalculateNewRingBuildPosition();
                    // Skip next.
                    CalculateNewRingBuildPosition();
                    mBuildLocationPreparedForGate = false;
                }
            }
            else
            {
                CalculateNewRingBuildPosition();
                mBuildLocationPreparedForGate = true;
            }
        }

        private bool IsGateLocation()
        {
            return mRingY == mCurrentBuildRange / 2 || mRingX == mCurrentBuildRange / 2
                || mRingY == mCurrentBuildRange / 2 + 1 || mRingX == mCurrentBuildRange / 2 + 1;
        }

        private float CalculateGateRotation()
        {
            if (mRingY == 0)
            {
                return (float)Math.PI;
            }

            if (mRingY == mCurrentBuildRange)
            {
                return 0f;
            }
            if (mRingX == 0)
            {
                return (float) Math.PI * 3 / 2;
            }
            return (float) Math.PI * 1 / 2;
        }

        private void CalculateNewRingBuildPosition()
        {
            // Build in circles
            if (mRingX == 0)
            {
                if (mRingY < mCurrentBuildRange)
                {
                    mRingY++;
                }
                else
                {
                    mRingX++;
                }
            }
            else if (mRingX < mCurrentBuildRange)
            {
                if (mRingY == mCurrentBuildRange)
                {
                    mRingX++;
                }

                if (mRingY == 0)
                {
                    mRingX--;
                }
            }
            else
            {
                if (mRingY > 0)
                {
                    mRingY--;
                }
                else
                {
                    mRingX--;
                }
            }

            if (mRingX != 0 || mRingY != 0) return;
            if (mCurrentBuildRange >= 15)
            {
                mCurrentBuildRange = MinimumBuildRange;
            }
            else
            {
                mCurrentBuildRange += 4;
            }
        }

        private void HeroLogic(IReadOnlyList<IDamageableUnit> enemyUnits)
        {
            var needsStone = GetGold() > GetStone();
            var primMines = needsStone ? GetQuarries() : GetGoldMines();
            var secMines = needsStone ? GetGoldMines() : GetQuarries();
            var secStar = GetSacredStar();
            var hero = GetHero();
            if (primMines.Count > 0 || secMines.Count > 0)
                hero.SetMovingTarget(GetHeroTarget(primMines, secMines));
            else if (secStar != null)
                hero.SetMovingTarget(secStar.Position);
            else
                hero.SetTarget(enemyUnits[0], true);
        }

        private void AttackEnemyUnits(IEnumerable<IAttackingUnit> myUnits, IReadOnlyList<IDamageableUnit> enemyUnits)
        {
            foreach (var unit in myUnits)
            {
                // Bowmen only defend the base
                if (unit.UnitType == UnitTypes.Bowman)
                {
                    continue;
                }
                if (unit.HasTarget()) continue;
                var tries = 0;
                IDamageableUnit targetUnit = null;
                do
                {

                    if (tries <= 5)
                    {
                        if (tries == 0)
                        {
                            var unitsInRange =
                                GameState.Current.SpatialUnitsByPlayer.UnitsInRange(mEnemies.First(), unit.Position, 150);
                            if (unitsInRange.Count != 0)
                            {
                                targetUnit = (IDamageableUnit)unitsInRange[sRandom.Next((unitsInRange.Count - 1))];
                            }
                        }
                        
                        if (tries > 0 || targetUnit == null)
                        {
                            targetUnit = enemyUnits[sRandom.Next(enemyUnits.Count)];
                        }
                    }
                    else
                    {
                        targetUnit = null;
                        break;
                    }

                    tries++;
                } while (Math.Abs(AttackModifiers.GetModifier(unit.UnitType, targetUnit.UnitType)) < float.Epsilon);

                if (targetUnit != null)
                {
                    unit.SetTarget(targetUnit, true);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.Ticks % 60 != 0 || !GetRunning()) return;
            var myUnits = GetAttackingUnits();
            var enemyUnits = GetEnemyUnits();
            HeroLogic(enemyUnits);
            BuildLogic();
            RecruitLogic();
            AttackEnemyUnits(myUnits, enemyUnits);
        }

        private void RecruitLogic()
        {
            if (mNextUnitType == null)
            {
                mNextUnitType = EnemyHasUnits() ? mToMake[sRandom.Next(mToMake.Length)] : UnitTypes.BatteringRam;
            }
            else
            {
                mNextUnitType = AddUnits(mNextUnitType.Value) ? null : mNextUnitType;
            }
        }
    }
}
