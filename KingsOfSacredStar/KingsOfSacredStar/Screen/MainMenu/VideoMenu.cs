using System;
using System.Collections.Generic;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Screen.MainMenu
{
    internal sealed class VideoMenu : AMainMenuPage
    {
        public VideoMenu(ContentManager content, GraphicsDeviceManager graphics, Camera camera) : base(content, graphics, camera, GetButtons(content, graphics)) { }

        private static AButton[] GetButtons(ContentManager content, GraphicsDeviceManager graphics)
        {
            return new AButton[]
            {
                new ActionButton(content, 300, 300, 400, 100, new[] {Properties.VideoMenu.Fullscreen + ScreenSize.IsFullScreen()}, () => ScreenSize.ChangeSize(graphics), followingScreens: new List<Screen>{Screen.VideoMenu}, deletingScreens: new List<Screen>{Screen.VideoMenu}),
                new MenuButton(content, 300, 500, 400, 100, new[] {Properties.VideoMenu.Back}, new List<Screen> {Screen.OptionsMenu}, new List<Screen> {Screen.VideoMenu})
            };
        }

        public override void Update(GameTime gameTime, Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            base.Update(gameTime, addScreens, removeScreens);
            PanToPosition(new Vector3(-2, 0, -3) * GameScreen.GridSize, gameTime);
        }
    }
}