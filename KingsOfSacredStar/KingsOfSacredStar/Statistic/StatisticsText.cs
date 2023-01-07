using System;
using System.Collections.Generic;
using KingsOfSacredStar.Screen;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Statistic
{
    internal sealed class StatisticsText : AButton
    {

        public StatisticsText(ContentManager content, int x, int y, int width, int height, string text)
            : base(content, x, y, width, height, new[] {text}) {}

        public override void IsClicked(Action<List<Screen.Screen>> addScreens, Action<List<Screen.Screen>> removeScreens)
        {
            // No click event
        }
    }
}