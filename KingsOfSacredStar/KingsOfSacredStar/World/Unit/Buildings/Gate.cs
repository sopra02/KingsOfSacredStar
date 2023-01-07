using KingsOfSacredStar.GameLogic;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World.Unit.Buildings
{
    internal sealed class Gate : ADestroyableBuilding
    {
        public bool IsOpen { get; private set; }
        private ModelManager.Model mCurrentModel = ModelManager.GetInstance().Gate;

        private double mTotalGameMilliseconds;
        private bool mGateStateSetManually;
        private int mLastTimestampManualGateStateSet;

        // False positive
        // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
        public override ModelManager.Model ModelType => mCurrentModel;
        public Gate(Players owner, Vector2 pos, float rot, int gridSize) :
            base(null,
                UnitTypes.Gate,
                owner,
                pos,
                rot,
                gridSize,
                GetBlockedFields(),
                GetPassableFields())
        {
            IsOpen = true;
            MaxHealth = (int)BaseStats.sUnitStats[UnitType][StatNames.Health];
            mHitHelper = new HitHelper(this, MaxHealth);
        }

        private static Point[] GetBlockedFields()
        {
            return new[] {new Point(-1, 0), new Point(1, 0)};
        }

        private static Point[] GetPassableFields()
        {
            return new[] {new Point(0, 0)};
        }

        public void ToggleGateState()
        {
            SetGateState(!IsOpen);
        }

        public void SetGateState(bool setOpen)
        {
            if (setOpen)
            {
                OpenGate();
            }
            else
            {
                CloseGate();
            }

            mGateStateSetManually = true;
        }

        private void OpenGate()
        {
            IsOpen = true;
            GameState.Current.ChangeFieldState(TileStates.Passable, PassableFields());
            mCurrentModel = ModelManager.GetInstance().Gate;
            GameState.Current.SignalModelUpdate(ModelManager.GetInstance().GateClosed, this);
        }

        private void CloseGate()
        {
            IsOpen = false;
            GameState.Current.ChangeFieldState(TileStates.Blocked, PassableFields());
            mCurrentModel = ModelManager.GetInstance().GateClosed;
            GameState.Current.SignalModelUpdate(ModelManager.GetInstance().Gate, this);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateEverySecond(gameTime);
        }

        private void UpdateEverySecond(GameTime gameTime)
        {
            if ((int)gameTime.TotalGameTime.TotalMilliseconds < mTotalGameMilliseconds + 1000)
            {
                return;
            }

            HandleManualGateStateSet(gameTime);

            if (EnoughTimePassedSinceLastManualGateStateSet(gameTime))
            {
                AutomaticGateToggle();
            }

            mTotalGameMilliseconds = gameTime.TotalGameTime.TotalMilliseconds;


        }

        private void AutomaticGateToggle()
        {
            var enemiesInRange = EnemiesInRange();
            if (enemiesInRange.Count != 0 && IsOpen)
            {
                CloseGate();
            }
            else if (enemiesInRange.Count == 0 && !IsOpen )
            {
                OpenGate();
            }
        }

        private bool EnoughTimePassedSinceLastManualGateStateSet(GameTime gameTime)
        {
            var timeoutInSeconds = 5;

            return gameTime.TotalGameTime.TotalSeconds > mLastTimestampManualGateStateSet + timeoutInSeconds;
        }

        private void HandleManualGateStateSet(GameTime gameTime)
        {
            if (mGateStateSetManually)
            {
                mLastTimestampManualGateStateSet = (int) gameTime.TotalGameTime.TotalSeconds;
                mGateStateSetManually = false;
            }
        }
    }
}
