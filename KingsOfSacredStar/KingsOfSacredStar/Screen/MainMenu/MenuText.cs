using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen.MainMenu
{
    internal sealed class MenuText : AButton
    {
        public MenuText(ContentManager content, int x, int y, int width, int height, string[] text) :
            base(content, x, y, width, height, text)
        {
        }

        public override void IsClicked(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
        }

        public override void Draw(SpriteBatch spriteBatch, float transparency)
        {
            DrawText(spriteBatch, transparency);
        }
    }
}
