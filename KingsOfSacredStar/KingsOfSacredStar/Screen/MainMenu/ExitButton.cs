using System;
using System.Collections.Generic;
using KingsOfSacredStar.InputWrapper;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Screen.MainMenu
{
    internal sealed class ExitButton : AButton
    {
        public ExitButton(ContentManager content,
            int x,
            int y,
            int width,
            int height,
            string[] text) :
            base(content, x, y, width, height, text)
        {
        }

        public override void IsClicked(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            ExitWrapper.Quit();
        }
    }
}