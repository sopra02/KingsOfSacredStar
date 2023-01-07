using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen
{
    internal sealed class ActionButton : AButton
    {

        private readonly Action mAction;
        private readonly List<Screen> mScreensFollowing;
        private readonly List<Screen> mScreensDeleting;

        public ActionButton(ContentManager content,
            int x,
            int y,
            int width,
            int height,
            string[] text,
            Action action,
            Color boxColor = default,
            List<Screen> followingScreens = null,
            List<Screen> deletingScreens = null) :
            base(content, x, y, width, height, text, boxColor)
        {
            mAction = action;
            mScreensFollowing = followingScreens;
            mScreensDeleting = deletingScreens;
        }

        public override void IsClicked(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            mAction();
            if (mScreensDeleting != null)
            {
                removeScreens(mScreensDeleting);
            }

            if (mScreensFollowing != null)
            {
                addScreens(mScreensFollowing);
            }
        }
        protected override void DrawBackground(SpriteBatch spriteBatch, float transparency)
        {
            spriteBatch.Draw(mBackground, mBox, mBoxMainColor);
        }
    }
}
