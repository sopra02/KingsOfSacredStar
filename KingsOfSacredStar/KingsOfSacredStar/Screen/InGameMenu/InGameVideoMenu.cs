using System.Collections.Generic;
using KingsOfSacredStar.Properties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Screen.InGameMenu
{
    internal sealed class InGameVideoMenu : AMenuPage
    {
        public InGameVideoMenu(ContentManager content, GraphicsDeviceManager graphics) : base(GetButtons(content, graphics)) {}

        private static AButton[] GetButtons(ContentManager content, GraphicsDeviceManager graphics)
        {
            return new AButton[]
            {
                new ActionButton(content, 620, 320, 260, 50, new[] {VideoMenu.Fullscreen + ScreenSize.IsFullScreen()}, () =>
                    {
                        ScreenSize.ChangeSize(graphics); //TODO reload game HUD
                    }, default,new List<Screen> { Screen.InGameMenu, Screen.GameHud}, new List<Screen> {Screen.GameHud,Screen.InGameVideoMenu,Screen.InGameMenu}),
                new MenuButton(content, 620, 530, 260, 50, new[] {VideoMenu.Back}, new List<Screen> {Screen.InGameMenu}, new List<Screen> {Screen.InGameVideoMenu})
            };
        }

    }
}