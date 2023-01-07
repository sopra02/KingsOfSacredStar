using System;
using System.Collections.Generic;
using KingsOfSacredStar.World;
using KingsOfSacredStar.World.Unit;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Screen.Hud
{
    internal sealed class BuildMenuPanel : AMenuPage
    {
        public BuildMenuPanel(ContentManager content) : base(GetButtons(content))
        {
        }

        private static AButton[] GetButtons(ContentManager content)
        {
            var buttonPositionX = 195;
            const int buttonPositionY = 700;
            const int buttonWidth = 175;
            const int buttonHeight = 175;
            const int buttonPaddingX = 20;

            var buildingTypes = new List<UnitTypes>
            {
                UnitTypes.Gate,
                UnitTypes.Wall
            };

            var buildingModels = new Dictionary<UnitTypes, ModelManager.Model>
            {
                {UnitTypes.Gate, ModelManager.GetInstance().Gate},
                {UnitTypes.Wall, ModelManager.GetInstance().Wall}
            };

            var buildingDisplayNames = new Dictionary<UnitTypes, string>
            {
                {UnitTypes.Gate, Properties.BuildMenuPanel.Gate},
                {UnitTypes.Wall, Properties.BuildMenuPanel.Wall}
            };

            var buttons = new List<AButton>();

            foreach (var building in buildingTypes)
            {
                var buildingText = new[] {buildingDisplayNames[building], Costs.SerializeUnitCost(building)};
                buttons.Add(new BuildButton(content,
                    buttonPositionX,
                    buttonPositionY,
                    buttonWidth,
                    buttonHeight,
                    building,
                    false,
                    buildingText,
                    buildingModels[building]));
                buttonPositionX += buttonWidth + buttonPaddingX;
            }

            const int deleteButtonHeight = 40;

            buttons.Add(new BuildButton(content,
                buttonPositionX,
                buttonPositionY + buttonHeight - deleteButtonHeight,
                buttonWidth,
                deleteButtonHeight,
                null,
                false,
                new[] {Properties.BuildMenuPanel.Delete},
                null));

            buttons.Add(new BuildButton(content,
                buttonPositionX,
                buttonPositionY + buttonHeight - 3 * deleteButtonHeight,
                buttonWidth,
                deleteButtonHeight,
                null,
                true,
                new[] {Properties.BuildMenuPanel.ToggleGate},
                null));

            return buttons.ToArray();
        }
        protected override bool CheckEsc(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            return false;
        }
    }
}