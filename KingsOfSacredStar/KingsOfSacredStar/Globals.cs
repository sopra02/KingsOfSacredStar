using Microsoft.Xna.Framework;

namespace KingsOfSacredStar
{
    internal static class Globals
    {
        public static Point Resolution { get; private set; }

        public static void GetResolution(GraphicsDeviceManager graphics)
        {
            Resolution = new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }
    }
}
