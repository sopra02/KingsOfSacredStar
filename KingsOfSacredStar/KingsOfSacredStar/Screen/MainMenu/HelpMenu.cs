using System;
using System.Collections.Generic;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Screen.MainMenu
{
    internal sealed class HelpMenu : AMainMenuPage
    {
        private static readonly Func<string[]>[] sTips = {
            () => new[]
            {
                Properties.HelpMenu.HowStart0,
                Properties.HelpMenu.HowStart1
            },
            () => new[]
            {
                Properties.HelpMenu.Color0,
                Properties.HelpMenu.Color1,
                Properties.HelpMenu.Color2
            },
            () => new[]
            {
                Properties.HelpMenu.Recruiting0,
                Properties.HelpMenu.Recruiting1
            },
            () => new[]
            {
                Properties.HelpMenu.Building0,
                Properties.HelpMenu.Building1
            },
            () => new[]
            {
                Properties.HelpMenu.MiniMap0,
                Properties.HelpMenu.MiniMap1
            },
            () => new[]
            {
                Properties.HelpMenu.Scrolling0,
                Properties.HelpMenu.Scrolling1
            },
            () => new[]
            {
                Properties.HelpMenu.Select0,
                Properties.HelpMenu.Select1
            },
            () => new[]
            {
                Properties.HelpMenu.Move0,
                Properties.HelpMenu.Move1
            },
            () => new[]
            {
                Properties.HelpMenu.Hero0,
                Properties.HelpMenu.Hero1,
                Properties.HelpMenu.Hero2,
                Properties.HelpMenu.Hero3
            },
            () => new[]
            {
                Properties.HelpMenu.Mines0,
                Properties.HelpMenu.Mines1,
            },
            () => new[]
            {
                Properties.HelpMenu.SacredStar0,
                Properties.HelpMenu.SacredStar1
            },
            () => new[] {Properties.HelpMenu.GoodLuck},
            () => new[] {Properties.HelpMenu.HaveFun}
        };

        private static int sCurrentPage;

        public HelpMenu(ContentManager content, GraphicsDeviceManager graphics, Camera camera) : base(content, graphics, camera, GetButtons(content)) { }

        private static AButton[] GetButtons(ContentManager content)
        {
            var list = new List<Screen> { Screen.HelpMenu };
            return new AButton[]
            {
                new MenuButton(content, 25, 25, 1550, 500, sTips[sCurrentPage]()),
                new ActionButton(content, 50, 600, 150, 100, new []{ Properties.HelpMenu.Previous },
                    () => sCurrentPage = Math.Max(sCurrentPage - 1, 0), sCurrentPage == 0 ? Color.Gray : default, list, list),
                new ActionButton(content, 1400, 600, 150, 100, new []{ Properties.HelpMenu.Next },
                    () => sCurrentPage = Math.Min(sCurrentPage + 1, sTips.Length - 1), sCurrentPage == sTips.Length - 1 ? Color.Gray : default, list, list),
                new MenuButton(content, 1400, 750, 150, 100, new[] {Properties.HelpMenu.Back}, new List<Screen> {Screen.MainMenu}, list)
            };
        }
    }
}