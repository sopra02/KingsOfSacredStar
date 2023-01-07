using System;
using System.Collections.Generic;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Screen.MainMenu
{
    internal sealed class LoadGameMenu : AMainMenuPage
    {
        public LoadGameMenu(ContentManager content, GraphicsDeviceManager graphics, Camera camera) : base(content, graphics, camera, GetButtons(content)) { }

        private static AButton[] GetButtons(ContentManager content)
        {
            return new AButton[]
            {
                new LoadGameButton(content, 300, 300, 400, 100, new[] {Properties.LoadGameMenu.File1}, followingScreens: new List<Screen> {Screen.RecruitMenuPanel, Screen.GameHud, Screen.GameScreen},  deletingScreens: new List<Screen> {Screen.LoadGameMenu}, "File1"),
                new LoadGameButton(content, 300, 500, 400, 100, new[] {Properties.LoadGameMenu.File2}, followingScreens: new List<Screen> {Screen.RecruitMenuPanel, Screen.GameHud, Screen.GameScreen},  deletingScreens: new List<Screen> {Screen.LoadGameMenu}, "File2"), 
                new LoadGameButton(content, 300, 700, 400, 100, new[] {Properties.LoadGameMenu.File3}, followingScreens: new List<Screen> {Screen.RecruitMenuPanel, Screen.GameHud, Screen.GameScreen},  deletingScreens: new List<Screen> {Screen.LoadGameMenu}, "File3"),
                new LoadGameButton(content, 900, 300, 400, 100, new[] {Properties.LoadGameMenu.File4}, followingScreens: new List<Screen> {Screen.RecruitMenuPanel, Screen.GameHud, Screen.GameScreen},  deletingScreens: new List<Screen> {Screen.LoadGameMenu}, "File4"),
                new LoadGameButton(content, 900, 500, 400, 100, new[] {Properties.LoadGameMenu._1000_Units}, followingScreens: new List<Screen> {Screen.RecruitMenuPanel, Screen.GameHud, Screen.GameScreen},  deletingScreens: new List<Screen> {Screen.LoadGameMenu}, "Dummy"),
                new MenuButton(content, 900, 700, 400, 100, new[] {Properties.LoadGameMenu.Back}, new List<Screen> {Screen.SinglePlayerMenu}, new List<Screen> {Screen.LoadGameMenu})
            };
        }

        public override void Update(GameTime gameTime, Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            base.Update(gameTime, addScreens, removeScreens);
            PanToPosition(new Vector3(2, 0, 4) * GameScreen.GridSize, gameTime);
        }
    }
}