using KingsOfSacredStar.GameLogic;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World.Unit.Buildings
{
    internal sealed class SacredStar : ABuilding, ICapturable
    {
        public bool NeedsDrawing => mTakeOverManager.NeedsDrawing;
        public float FriendlyPercentage => mTakeOverManager.FriendlyPercentage;
        public float EnemyPercentage => mTakeOverManager.EnemyPercentage;

        private readonly TakeOverManager mTakeOverManager;
        private bool mIsApplied;

        public SacredStar(Players owner, Vector2 pos, int gridSize)
            : base(ModelManager.GetInstance().SacredStar, UnitTypes.SacredStar, owner, pos, 0f, gridSize)
        {
            mTakeOverManager = new TakeOverManager(Position);
            mIsApplied = false;
        }

        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var newOwner = mTakeOverManager.Overtaking(gameTime, true);
            if (newOwner == Owner)
            {
                if (Owner != Players.Global && !mIsApplied)
                {
                    foreach (var player in PlayerConstants.sPlayers)
                    {
                        GameState.Current.mDamageFactor[player] = 0.5f;
                    }
                    GameState.Current.mDamageFactor[Owner] = 2f;
                }
            }
            else
            { 
                Owner = newOwner;
                mIsApplied = false;
                foreach (var player in PlayerConstants.sPlayers)
                {
                    GameState.Current.mDamageFactor[player] = 1f;
                }
            }
        }
    }
}
