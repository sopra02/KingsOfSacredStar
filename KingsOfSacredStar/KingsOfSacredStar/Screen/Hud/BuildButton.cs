using System;
using System.Collections.Generic;
using KingsOfSacredStar.World;
using KingsOfSacredStar.World.Unit;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen.Hud
{
    internal sealed class BuildButton : AButton
    {
        private readonly UnitTypes? mBuildingType;
        private readonly ModelManager.Model mBuildingModel;
        private readonly bool mGateToggle;

        public BuildButton(ContentManager content,
            int x,
            int y,
            int width,
            int height,
            UnitTypes? type,
            bool gateToggle,
            string[] text,
            ModelManager.Model model) : base(content, x, y, width, height, text, boxColor: default, font: content.Load<SpriteFont>("fonts/small"))
        {
            mBuildingType = type;
            mBuildingModel = model;
            mGateToggle = gateToggle;
        }

        public override void IsClicked(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            GameScreen.BuildManager.EnterBuildMode(mBuildingType, mBuildingModel, mGateToggle);
        }
    }
}
