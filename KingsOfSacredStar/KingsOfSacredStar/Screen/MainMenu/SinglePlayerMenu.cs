using System;
using System.Collections.Generic;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Screen.MainMenu
{
    internal sealed class SinglePlayerMenu : AMainMenuPage
    {

        public SinglePlayerMenu(ContentManager content, GraphicsDeviceManager graphics, Camera camera) : base(content, graphics, camera, GetButtons(content)) { }

        private static AButton[] GetButtons(ContentManager content)
        {
            return new AButton[]
            {
                new MenuButton(content, 300, 300, 400, 100, new[] {Properties.SinglePlayerMenu.NewGame}, new List<Screen> { Screen.RecruitMenuPanel, Screen.GameHud, Screen.GameScreen}, new List<Screen> {Screen.SinglePlayerMenu}),
                new MenuButton(content, 300, 500, 400, 100, new[] {Properties.SinglePlayerMenu.LoadGame}, new List<Screen> {Screen.LoadGameMenu}, new List<Screen> {Screen.SinglePlayerMenu}),
                new MenuButton(content, 300, 700, 400, 100, new[] {Properties.SinglePlayerMenu.Back}, new List<Screen> {Screen.MainMenu}, new List<Screen> {Screen.SinglePlayerMenu})
            };
        }

        public override void Update(GameTime gameTime, Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            base.Update(gameTime, addScreens, removeScreens);
            PanToPosition(new Vector3(2 * GameScreen.GridSize, 0, 3), gameTime);
        }
    }
}
