using System;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.InputWrapper;
using KingsOfSacredStar.Screen;
using KingsOfSacredStar.World.Unit;
using KingsOfSacredStar.World.Unit.Buildings;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World
{
    internal sealed class BuildManager
    {

        private readonly InputManager mInputManager;

        public bool IsActive { get; set; }

        private ModelManager.Model mBuildModelType;

        private float mBuildingRotation;
        private UnitTypes? mBuildingType;
        private bool mGateToggle;

        private IDamageableUnit mLastSelectedUnit;


        public BuildManager(InputManager input)
        {
            mInputManager = input;
            IsActive = false;
        }

        public void Draw(GameTime gameTime)
        {
            if (!IsActive)
            {
                return;
            }

            var buildingHoverPosition = ToolBox.Current.ConvertScreenToGameFieldPosition(mInputManager.GetMousePosition()).ToVector2();

            if (mBuildingType == null)
            {
                HoverSelectionMode(buildingHoverPosition);
                return;
            }

            // Convert for grid based placement
            buildingHoverPosition.X -= buildingHoverPosition.X % GameScreen.GridSize;
            buildingHoverPosition.Y -= buildingHoverPosition.Y % GameScreen.GridSize;

            var matrix = Matrix.CreateTranslation(GameScreen.GridSize / 2f, 0, GameScreen.GridSize / 2f) *
                         Matrix.CreateRotationY(mBuildingRotation) *
                         Matrix.CreateTranslation(GameScreen.GridSize / -2f, 0, GameScreen.GridSize / -2f) *
                         Matrix.CreateTranslation(buildingHoverPosition.X, 0, buildingHoverPosition.Y);

            var effect = GameState.Current.IsValidBuildingPlace(Players.Player,
                buildingHoverPosition / GameScreen.GridSize,
                mBuildingType.Value,
                mBuildingRotation)
                ? ModelManager.RenderingEffect.None
                : ModelManager.RenderingEffect.Hit;
            mBuildModelType.Draw(gameTime, new ModelManager.RenderingInformation(matrix, PlayerConstants.sPlayerColors[(int)Players.Player], effect));
        }

        private void HoverSelectionMode(Vector2 gameFieldPosition)
        {
            var unit = GameState.Current.GetDamageableUnitAtPosition(gameFieldPosition.ToPoint(), Players.Player);

            if (unit != null)
            {
                unit.IsHit = true;
            }

            if (mLastSelectedUnit != null && mLastSelectedUnit != unit)
            {
                mLastSelectedUnit.IsHit = false;
            }

            mLastSelectedUnit = unit;
        }


        public void EnterBuildMode(UnitTypes? type, ModelManager.Model model, bool gateToggle)
        {
            IsActive = true;
            mBuildingType = type;
            mBuildModelType = model;
            mGateToggle = gateToggle;
        }

        public void ProcessMouseLeftClick(Point inputLastMouseClickPosition)
        {
            var gameFieldPosition = ToolBox.Current.ConvertScreenToGameFieldPosition(inputLastMouseClickPosition)
                    .ToVector2();
            if (IsInBuildMode())
            {
                BuildBuilding(gameFieldPosition);
            }
            else if (IsInGateToggleMode())
            {
                ToggleGateState(gameFieldPosition);
            }
            else
            {
                DeleteBuilding(gameFieldPosition);
            }
        }

        private bool IsInBuildMode()
        {
            return IsActive && mBuildingType.HasValue;
        }

        public bool IsInGateToggleMode()
        {
            return IsActive && mGateToggle;
        }

        public bool IsInDeleteMode()
        {
            return IsActive && !IsInBuildMode() && !IsInGateToggleMode();
        }

        private static void ToggleGateState(Vector2 gameFieldPosition)
        {
            GameState.Current.SelectEntity(gameFieldPosition.ToPoint(), Players.Player);
            foreach (var unit in GameState.Current.SelectedEntities)
            {
                if (unit.UnitType == UnitTypes.Gate)
                {
                    ((Gate) unit).ToggleGateState();
                }
            }
        }


        private void BuildBuilding(Vector2 gameFieldPosition)
        {
            gameFieldPosition /= GameScreen.GridSize;
            gameFieldPosition = gameFieldPosition.ToPoint().ToVector2();

            if (mBuildingType.HasValue &&
                Costs.HasEnoughResourcesForUnit(mBuildingType.Value, Players.Player) &&
                GameState.Current.IsValidBuildingPlace(Players.Player,
                    gameFieldPosition,
                    mBuildingType.Value,
                    mBuildingRotation) && GameState.Current.AddBuilding(Players.Player,
                    gameFieldPosition,
                    mBuildingType.Value,
                    mBuildingRotation))
            {
                Costs.PayUnitCosts(mBuildingType.Value, Players.Player);
            }
        }

        private static void DeleteBuilding(Vector2 gameFieldPosition)
        {
            GameState.Current.SelectEntity(gameFieldPosition.ToPoint(), Players.Player);
            foreach (var unit in GameState.Current.SelectedEntities)
            {
                if (unit.UnitType == UnitTypes.Gate || unit.UnitType == UnitTypes.Wall)
                {
                    ((IDamageableUnit) unit).Health = 0;
                }
            }
        }

        public void ProcessMouseRightClick()
        {
            IsActive = false;
        }

        public void RotateBuilding(float deltaRot)
        {
            mBuildingRotation = (mBuildingRotation + deltaRot) % (float) (Math.PI * 2);
        }
    }
}
