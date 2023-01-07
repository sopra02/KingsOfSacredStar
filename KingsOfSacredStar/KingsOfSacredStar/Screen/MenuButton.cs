using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Screen
{
    internal sealed class MenuButton : AButton 
    {

        private readonly List<Screen> mScreensFollowing;
        private readonly List<Screen> mScreensDeleting;

        public MenuButton(ContentManager content,
            int x,
            int y,
            int width,
            int height,
            string[] text,
            List<Screen> screensFollowing,
            List<Screen> screensDeleting) : base(content, x, y, width, height, text)
        {
            mScreensFollowing = screensFollowing;
            mScreensDeleting = screensDeleting;
        }


        public MenuButton(ContentManager content, int x, int y, int width, int height, string[] text) :
            base(content, x, y, width, height, text)
        {
            mScreensFollowing = new List<Screen>();
            mScreensDeleting = new List<Screen>();
        }

        public override void IsClicked(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            addScreens(mScreensFollowing);
            removeScreens(mScreensDeleting);
        }
    }
}
