﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen.Hud
{
    internal sealed class GameHudText : AButton
    {
        public GameHudText(ContentManager content, int x, int y, int width, int height, string[] text)
            : base(content, x, y, width, height, text, boxColor: Color.Gray, font: content.Load<SpriteFont>("fonts/Small"))
        {}


        public override void IsClicked(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            throw new NotImplementedException();
        }
    }
}