using System.Collections.Generic;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.World.Unit;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World
{
    internal sealed class TakeOverManager
    {
        public bool NeedsDrawing => mOvertaker != mCurrentOwner;

        public float FriendlyPercentage
        {
            get
            {
                if (mOvertaker == Players.Player)
                {
                    return mOvertakeCount / 100f;
                }
                return mCurrentOwner == Players.Global ? 0 : (100 - mOvertakeCount) / 100f;
            }
        }

        public float EnemyPercentage
        {
            get
            {
                if (mOvertaker != Players.Player)
                {
                    return mOvertakeCount / 100f;
                }
                return mCurrentOwner == Players.Global ? 0 : (100 - mOvertakeCount) / 100f;
            }
        }

        private static readonly Dictionary<Players, int> sOwnerOfMines = new Dictionary<Players, int>
        {
            {Players.Global, 0},
            {Players.Player, 0},
            {Players.Ai, 0}
        };
        private static int sMineCount;

        private long mLastTimeOvertake;
        private int mOvertakeCount;
        private Players mOvertaker;
        private Players mCurrentOwner;
        private readonly Vector2 mTargetPos;

        public TakeOverManager(Vector2 targetPos)
        {
            mTargetPos = targetPos;
            mCurrentOwner = Players.Global;
            mOvertaker = Players.Global;
            mOvertakeCount = 0;
        }
        public static void AddMine()
        {
            sMineCount++;
            sOwnerOfMines[Players.Global]++;
        }
        public static void TakeMineOver(Players oldPlayer, Players newPlayer)
        {
            if (sOwnerOfMines[oldPlayer] != 0) sOwnerOfMines[oldPlayer]--;
            sOwnerOfMines[newPlayer]++;
        }

        private IUnit NearestHero()
        {
            return GameState.Current.SpatialUnitsByPlayer.NearestUnitOfType(
                mTargetPos,
                3 * 16,
                UnitTypes.Hero);
        }

        private bool UpdateTakeOverCount(bool halfOfMinesNeeded, IUnit minHero)
        {
            if (minHero.Owner == Players.Global ||
                (halfOfMinesNeeded && sOwnerOfMines[minHero.Owner] < sMineCount / 2)) return false;
            if (minHero.Owner == mOvertaker)
            {
                if (mCurrentOwner != Players.Global && 
                    GameState.Current.SpatialUnitsByPlayer.UnitsInRange(mCurrentOwner, mTargetPos, 3 * 16f).Count != 0)
                {
                    return true;
                }
                mOvertakeCount++;

            }
            else if (minHero.Owner != mCurrentOwner)
            {
                if (mOvertakeCount > 0)
                {
                    mOvertakeCount -= 2;
                }
                else
                {
                    mOvertaker = minHero.Owner;
                }
            }
            else if (mOvertakeCount > 0)
            {
                mOvertakeCount--;
            }
            else
            {
                mOvertakeCount = 0;
            }

            return false;
        }


        public Players Overtaking(GameTime gameTime, bool halfOfMinesNeeded)
        {
            // Check every 10 ticks
            if (mLastTimeOvertake + 10 < gameTime.TotalGameTime.Ticks)
            {
                var minHero = NearestHero();
                if (minHero != null && UpdateTakeOverCount(halfOfMinesNeeded, minHero))
                {
                    return mCurrentOwner;
                }
                mLastTimeOvertake = gameTime.TotalGameTime.Ticks;
            }
            

            if (mOvertakeCount >= 100)
            {
                mOvertakeCount = 0;
                mCurrentOwner = mOvertaker;
            }
            return mCurrentOwner;
        }
    }
}
