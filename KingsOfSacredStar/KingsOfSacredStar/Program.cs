using System;

namespace KingsOfSacredStar
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            using (var game = new KossGame())
                game.Run();
        }
    }
#endif
}
