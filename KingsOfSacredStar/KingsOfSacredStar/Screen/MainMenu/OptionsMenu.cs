using System;
using System.Collections.Generic;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Screen.MainMenu
{
    internal sealed class OptionsMenu : AMainMenuPage
    {
        public OptionsMenu(ContentManager content, GraphicsDeviceManager graphics, Camera camera) : base(content, graphics, camera, GetButtons(content)) { }

        private static AButton[] GetButtons(ContentManager content)
        {
            return new AButton[]
            {
                new MenuButton(content, 300, 300, 400, 100, new[] {Properties.OptionsMenu.Audio}, new List<Screen> {Screen.AudioMenu}, new List<Screen> {Screen.OptionsMenu}),
                //new MenuButton(content, 300, 700, 400, 100, Properties.OptionsMenu.Controls, new List<Screen.Screen> {Screen.Screen.ControlsMenu}, new List<Screen.Screen> {Screen.Screen.OptionsMenu}),
                new MenuButton(content, 300, 500, 400, 100, new[] {Properties.OptionsMenu.Video}, new List<Screen> {Screen.VideoMenu}, new List<Screen> {Screen.OptionsMenu}),
                new ActionButton(content, 900, 300, 400, 100, new[] {Properties.OptionsMenu.Language}, Language.ChangeLanguage, followingScreens: new List<Screen>{Screen.OptionsMenu}, deletingScreens: new List<Screen>{Screen.OptionsMenu}),
                new MenuButton(content, 900, 500, 400, 100, new[] {Properties.OptionsMenu.Back}, new List<Screen> {Screen.MainMenu}, new List<Screen> {Screen.OptionsMenu})
            };
        }

        public override void Update(GameTime gameTime, Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            base.Update(gameTime, addScreens, removeScreens);
            PanToPosition(new Vector3(0, 0, -3) * GameScreen.GridSize, gameTime);
        }
    }
}