using System;
using System.Collections.Generic;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.World;
using KingsOfSacredStar.World.Unit;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen.Hud
{
    internal sealed class SpawnButton : AButton
    {
        private readonly UnitTypes mUnitType;
        public SpawnButton(ContentManager content, int x, int y, int width, int height, string[] text, UnitTypes unitType)
            : base(content, x, y, width, height, text, boxColor: default, font: content.Load<SpriteFont>("fonts/small"))
        {
            mUnitType = unitType;
        }

        public override void IsClicked(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            if (Costs.PayUnitCosts(mUnitType, Players.Player))
            {
                GameState.Current.AddUnit(Players.Player, GameState.Current.VillagePosOffset(Players.Player), mUnitType);
            }
        }
    }
}
