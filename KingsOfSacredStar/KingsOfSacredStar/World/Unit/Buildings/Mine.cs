using System.Collections.Generic;
using KingsOfSacredStar.GameLogic;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World.Unit.Buildings
{
    internal sealed class Mine : ABuilding, ICapturable
    {
        public bool NeedsDrawing => mTakeOverManager.NeedsDrawing;
        public float FriendlyPercentage => mTakeOverManager.FriendlyPercentage;
        public float EnemyPercentage => mTakeOverManager.EnemyPercentage;

        private readonly Dictionary<Players, Dictionary<Resources, int>> mResources;

        public int Level { get; private set; }
        private double mLastTimeLevelUp;
        private double mLastTimeResource;
        private readonly TakeOverManager mTakeOverManager;

        public Mine(Players owner, Vector2 pos, float rot, bool gold, int gridSize, Dictionary<Players, Dictionary<Resources, int>> resources)
            : base(gold ? ModelManager.GetInstance().GoldMine : ModelManager.GetInstance().Quarry,
                gold ? UnitTypes.GoldMine : UnitTypes.Quarry,
                owner, pos, rot, gridSize, GetBlockedFields(), GetPassableFields())
        {
            mTakeOverManager = new TakeOverManager(Position);
            TakeOverManager.AddMine();
            mLastTimeResource = 0;
            mResources = resources;
            Level = 1;
        }

        public override void IsPaused(GameTime gameTime)
        {
            mLastTimeLevelUp = gameTime.TotalGameTime.TotalMilliseconds - (gameTime.TotalGameTime.TotalMilliseconds - mLastTimeLevelUp);
            mLastTimeResource = gameTime.TotalGameTime.TotalMilliseconds - (gameTime.TotalGameTime.TotalMilliseconds - mLastTimeResource);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var newOwner = mTakeOverManager.Overtaking(gameTime, false);
            if (newOwner != Owner)
            {
                Level = 1;
                mLastTimeLevelUp = gameTime.TotalGameTime.TotalMilliseconds;
                mLastTimeResource = gameTime.TotalGameTime.TotalMilliseconds;
                TakeOverManager.TakeMineOver(Owner, newOwner);
                Owner = newOwner;
            }
            if (mLastTimeLevelUp + 60 * 2 * Level * 1000 < gameTime.TotalGameTime.TotalMilliseconds)
            {
                mLastTimeLevelUp = gameTime.TotalGameTime.TotalMilliseconds;
                if (Level < 6) Level++;
            }
            if (mLastTimeResource + 15 *1000 < gameTime.TotalGameTime.TotalMilliseconds)
            {
                mLastTimeResource = gameTime.TotalGameTime.TotalMilliseconds;
                if (Owner != 0)
                {
                    if (UnitType == UnitTypes.GoldMine)
                    {
                        mResources[Owner][Resources.Gold] += Level;
                        GameState.Current.StatisticsByPlayer[Owner]["Gold"] += Level;
                    }
                    else
                    {
                        mResources[Owner][Resources.Stone]+=Level;
                        GameState.Current.StatisticsByPlayer[Owner]["Stone"] += Level;
                    }
                }
            }
        }
        private static Point[] GetBlockedFields()
        {
            return new[]
            {
                new Point(-1, -1),
                new Point(0, -1),
                new Point(1, -1),
                new Point(-1, 0),
                new Point(1, 0)
            };
        }

        private static Point[] GetPassableFields()
        {
            return new[] { new Point(0, 0) };
        }
    }
}
