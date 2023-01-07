using System;
using System.Collections.Generic;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Screen.MainMenu
{
    internal sealed class AchievementsMenu : AMainMenuPage
    {
        public AchievementsMenu(ContentManager content, GraphicsDeviceManager graphics, Camera camera) : base(content, graphics, camera, GetButtons(content)) {}

        private static AButton[] GetButtons(ContentManager content)
        {
            return new AButton[]
            {
                new MenuButton(content, 300, 300, 400, 100, new[] {Properties.AchievementsMenu.Achievements}, new List<Screen> {Screen.AchievementManager}, new List<Screen> {Screen.AchievementsMenu}),
                new MenuButton(content, 300, 500, 400, 100, new[] {Properties.AchievementsMenu.Statistics}, new List<Screen> {Screen.Statistics}, new List<Screen> {Screen.AchievementsMenu}),
                new MenuButton(content, 300, 700, 400, 100, new[] {Properties.AchievementsMenu.Back}, new List<Screen> {Screen.MainMenu}, new List<Screen> {Screen.AchievementsMenu})
            };
        }

        public override void Update(GameTime gameTime, Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            base.Update(gameTime, addScreens, removeScreens);
            PanToPosition(new Vector3(20, 0, 3), gameTime);
        }
    }
}