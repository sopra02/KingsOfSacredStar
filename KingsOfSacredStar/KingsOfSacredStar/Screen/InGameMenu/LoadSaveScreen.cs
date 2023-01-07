using System;
using System.Collections.Generic;
using System.Linq;
using KingsOfSacredStar.Screen.Hud;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen.InGameMenu
{
    internal sealed class LoadSaveScreen : AMenuPage
    {

        private readonly GameHudText mBackground;
        private readonly GameHudButton[] mFiles;
        private readonly GameHudButton mBack;
        private readonly Action<string> mLoadSaveAction;
        private readonly Screen mThisScreen;

        public LoadSaveScreen(ContentManager content, bool load) : base(new AButton[]
        {
            new MenuButton(content,
                700,
                660,
                100,
                50,
                new[] {Properties.LoadSaveScreen.Back},
                new List<Screen> {Screen.InGameMenu},
                new List<Screen> {Screen.LoadScreen, Screen.SaveScreen})
        })
        {
            mBackground = new GameHudText(content, 600, 50, 300, 700, new string[0]);

            mFiles = new[]
            {
                new GameHudButton(content, 700, 80, 100, 100, new[] {Properties.LoadGameMenu.File1}),
                new GameHudButton(content, 700, 200, 100, 100, new[] {Properties.LoadGameMenu.File2}),
                new GameHudButton(content, 700, 320, 100, 100, new[] {Properties.LoadGameMenu.File3}),
                new GameHudButton(content, 700, 440, 100, 100, new[] {Properties.LoadGameMenu.File4})
            };

            mBack = new GameHudButton(content, 700, 680, 100, 50, new[] {Properties.LoadGameMenu.Back});
            mLoadSaveAction = load
                ? (Action<string>) (str => LoadAndSaveManager.Current.LoadGame(str))
                : str => LoadAndSaveManager.Current.SaveGame(str);
            mThisScreen = load ? Screen.LoadScreen : Screen.SaveScreen;
        }


        public override void Update(GameTime gameTime, Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            var index = 1;
            foreach (var file in mFiles)
            {
                if (file.GetClicked())
                {
                    removeScreens(new List<Screen> { mThisScreen });
                    mLoadSaveAction($"File{index}");
                    addScreens(new List<Screen> { Screen.InGameMenu });
                    return;
                }
                index++;
            }
            base.Update(gameTime, addScreens, removeScreens);
        }

        public override bool ProcessMouseLeftClick(Point inputLastMouseClickPosition)
        {
            if (mFiles.Any(file => file.InBox(inputLastMouseClickPosition)))
            {
                return true;
            }
            base.ProcessMouseLeftClick(inputLastMouseClickPosition);
            return true;
        }

        public override void DrawHud(SpriteBatch spriteBatch)
        {
            base.DrawHud(spriteBatch);
            mBackground.Draw(spriteBatch, 1f);
            foreach (var file in mFiles)
            {
                file.Draw(spriteBatch, 1f);
            }
            mBack.Draw(spriteBatch, 1f);
        }
    }
}