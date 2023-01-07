using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen.Hud
{
    internal sealed class GameHudPlaceHolder : AButton
    {
        public GameHudPlaceHolder(ContentManager content, int x, int y, int width, int height) : base(
            content,
            x,
            y,
            width,
            height,
            new string[0])
        {

        }

        public override void Draw(SpriteBatch spriteBatch, float transparency)
        {
            spriteBatch.Draw(mBackground, mBox, Color.DarkBlue);
        }
        public override void IsClicked(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            // do nothing
        }
    }
}
