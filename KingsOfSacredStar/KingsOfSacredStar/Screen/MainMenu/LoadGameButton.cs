using System;
using System.Collections.Generic;
using KingsOfSacredStar.InputWrapper;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Screen.MainMenu
{
    internal sealed class LoadGameButton : AButton
    {
        private readonly List<Screen> mScreensFollowing;
        private readonly List<Screen> mScreensDeleting;
        private readonly string mFileName;

        public LoadGameButton(ContentManager content,
            int x,
            int y,
            int width,
            int height,
            string[] text,
            List<Screen> followingScreens,
            List<Screen> deletingScreens,
            string fileName):
            base(content, x, y, width, height, text)
        {
            mScreensFollowing = followingScreens;
            mScreensDeleting = deletingScreens;
            mFileName = fileName;
        }

        public override void IsClicked(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            removeScreens(mScreensDeleting);
            addScreens(mScreensFollowing);
            LoadAndSaveManager.Current.LoadGame(mFileName);
            GameStateWrapper.SetPause(false);
        }
    }
}