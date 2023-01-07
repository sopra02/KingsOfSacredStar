using KingsOfSacredStar.Properties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen
{
    internal static class ScreenSize
    {
        public static string IsFullScreen()
        {
            if (Settings.Dict["Fullscreen"] == "on")
            {
                return " " + VideoMenu.On;
            }
            return " " + VideoMenu.Off;
        }

        public static void CurrentSize(GraphicsDeviceManager graphics)
        {
            var fullscreen = Settings.Dict["Fullscreen"] == "on";
            graphics.IsFullScreen = fullscreen;
            if (fullscreen)
            {
                graphics.PreferredBackBufferWidth = 1600;
                graphics.PreferredBackBufferHeight = 900;
                graphics.ApplyChanges();
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                graphics.PreferredBackBufferWidth = 1600;
                graphics.PreferredBackBufferHeight = 900;
            }
            graphics.ApplyChanges();
            Globals.GetResolution(graphics);
        }

        public static void ChangeSize(GraphicsDeviceManager graphics)
        {
            Settings.Dict["Fullscreen"] = Settings.Dict["Fullscreen"] == "on" ? "off" : "on";
            CurrentSize(graphics);
            
            Settings.WriteSettings();
        }
    }
}
