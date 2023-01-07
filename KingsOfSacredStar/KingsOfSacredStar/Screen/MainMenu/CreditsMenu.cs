using System;
using System.Collections.Generic;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Screen.MainMenu
{
    internal sealed class CreditsMenu : AMainMenuPage
    {
        public CreditsMenu(ContentManager content, GraphicsDeviceManager graphics, Camera camera) : base(content, graphics, camera, GetButtons(content)) { }

        private static AButton[] GetButtons(ContentManager content)
        {
            return new AButton[]
            {
                new MenuButton(content, 550, 80, 1000, 50, new[] {Properties.CreditsMenu.Developers}),
                new MenuButton(content, 550, 160, 1000, 50, new[] {Properties.CreditsMenu.Dev1}),
                new MenuButton(content, 550, 240, 1000, 50, new[] {Properties.CreditsMenu.Dev2}),
                new MenuButton(content, 550, 320, 1000, 50, new[] {Properties.CreditsMenu.Dev3}),
                new MenuButton(content, 550, 400, 1000, 50, new[] {Properties.CreditsMenu.Dev4}),
                new MenuButton(content, 550, 480, 1000, 50, new[] {Properties.CreditsMenu.Dev5}),
                new MenuButton(content, 550, 560, 1000, 50, new[] {Properties.CreditsMenu.Dev6}),
                new MenuButton(content, 550, 640, 1000, 50, new[] {Properties.CreditsMenu.Dev7}),
                new MenuButton(content, 550, 720, 1000, 50, new[] {Properties.CreditsMenu.Tutor}),
                new MenuButton(content, 100, 750, 150, 100, new[] {Properties.CreditsMenu.Back}, new List<Screen> {Screen.MainMenu}, new List<Screen> {Screen.CreditsMenu})
            };
        }

        public override void Update(GameTime gameTime, Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            base.Update(gameTime, addScreens, removeScreens);
            PanToPosition(new Vector3(-3, 0, 0) * GameScreen.GridSize, gameTime);
        }
    }
}