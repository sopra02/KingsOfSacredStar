using System;
using System.Collections.Generic;
using KingsOfSacredStar.World;
using KingsOfSacredStar.World.Unit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Screen.Hud
{
    internal sealed class RecruitMenuPanel : AMenuPage
    {
        public RecruitMenuPanel(ContentManager content) : base(GetButtons(content))
        {
        }

        public  override bool ProcessMouseRightClick(Point inputLastMouseClickPosition)
        {
            return false;
        }
        private static AButton[] GetButtons(ContentManager content)
        {
            var buttonPositionX = 195;
            const int buttonPositionY = 700;
            const int buttonWidth = 175;
            const int buttonHeight = 175;
            const int buttonPaddingX = 20;


            var unitDisplayNames = new Dictionary<UnitTypes, string>
            {
                {UnitTypes.Swordsman, Properties.RecruitMenuPanel.Swordsman},
                {UnitTypes.Cavalry, Properties.RecruitMenuPanel.Cavalry},
                {UnitTypes.Bowman, Properties.RecruitMenuPanel.Bowman},
                {UnitTypes.BatteringRam, Properties.RecruitMenuPanel.Ram}
            };

            var buttons = new List<AButton>();

            foreach (var unit in unitDisplayNames)
            {
                var buildingText = new []{unitDisplayNames[unit.Key], Costs.SerializeUnitCost(unit.Key)};
                buttons.Add(new SpawnButton(content,
                    buttonPositionX,
                    buttonPositionY,
                    buttonWidth,
                    buttonHeight,
                    buildingText,
                    unit.Key));
                buttonPositionX += buttonWidth + buttonPaddingX;
            }

            return buttons.ToArray();
        }

        protected override bool CheckEsc(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            return false;
        }
    }
}