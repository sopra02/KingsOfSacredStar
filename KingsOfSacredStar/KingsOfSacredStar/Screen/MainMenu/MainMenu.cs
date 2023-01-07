using System;
using System.Collections.Generic;
using KingsOfSacredStar.Properties;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Screen.MainMenu
{
    internal sealed class MainMenu : AMainMenuPage
    {
        public MainMenu(ContentManager content, GraphicsDeviceManager graphics, Camera camera) : base(content, graphics, camera, new AButton[]{
            new MenuButton(content, 300, 300, 400, 100, new[] {MainMenuText.SinglePlayer}, new List<Screen> {Screen.SinglePlayerMenu}, new List<Screen> {Screen.MainMenu}),
            new MenuButton(content, 300, 500, 400, 100, new[] {MainMenuText.Options}, new List<Screen> {Screen.OptionsMenu}, new List<Screen> {Screen.MainMenu}),
            new MenuButton(content, 300, 700, 400, 100, new[] {MainMenuText.Achievements}, new List<Screen> {Screen.AchievementsMenu}, new List<Screen> {Screen.MainMenu}),
            new MenuButton(content, 900, 300, 400, 100, new[] {MainMenuText.Credits}, new List<Screen> {Screen.CreditsMenu}, new List<Screen> {Screen.MainMenu}),
            new MenuButton(content, 900, 500, 400, 100, new[] {MainMenuText.Help}, new List<Screen> {Screen.HelpMenu}, new List<Screen> {Screen.MainMenu}),
            new ExitButton(content, 900, 700, 400, 100, new[] {MainMenuText.Exit}), 
        })
        {
            GameState.Current = null;
        }

        public override void Update(GameTime gameTime, Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            base.Update(gameTime, addScreens, removeScreens);
            PanToPosition(new Vector3(), gameTime);
        }
    }
}
