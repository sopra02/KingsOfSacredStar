using KingsOfSacredStar.Screen;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.InputWrapper
{
    internal static class MouseInputWrapper
    {
        private static ScreenManager sScreen;

        internal static void Init(ScreenManager screenManager)
        {
            sScreen = screenManager;
        }

        internal static void LoadInputManager(InputManager input)
        {
            input.SetMouseEvents(OnLeftClick, OnRightClick);
        }

        private static void OnLeftClick(Point mouseLeftClickPosition)
        {
            sScreen.ProcessMouseLeftClick(mouseLeftClickPosition);
        }
        private static void OnRightClick(Point mouseRightClickPosition)
        {
            sScreen.ProcessMouseRightClick(mouseRightClickPosition);
        }
    }
}
