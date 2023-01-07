using System;
using System.Collections.Generic;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen.Hud.HeroPanel
{
    internal sealed class HeroRespawnTimerButton : AButton
    {


        public HeroRespawnTimerButton(ContentManager content,
            int x,
            int y,
            int width,
            int height
        ) : base(content, x, y, width, height, new[] {Properties.HeroMenuPanel.RespawnIn, "00"})
        {
        }


        public override void Draw(SpriteBatch spriteBatch, float transparency)
        {
            var secondsLeft = GameState.Current.HeroRespawnManager.GetRespawnSecondsLeft(Players.Player);
            if (secondsLeft != null)
            {
                mText = new[] {Properties.HeroMenuPanel.RespawnIn, secondsLeft.ToString()};
                base.Draw(spriteBatch, transparency);
            }

        }
        public override void IsClicked(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
        }
    }
}
