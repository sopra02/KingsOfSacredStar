using System;
using System.Collections.Generic;
using KingsOfSacredStar.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Statistic.Achievement
{
    internal sealed class AchievementText : AButton
    {
        public AchievementText(ContentManager content, int x, int y, int width, int height, string text, bool achieved)
        : base(content, x, y, width, height, new []{text}, achieved ?  Color.Brown : Color.IndianRed) {}

        public override void IsClicked(Action<List<Screen.Screen>> addScreens, Action<List<Screen.Screen>> removeScreens)
        {
            // No click action
        }
    }
}
