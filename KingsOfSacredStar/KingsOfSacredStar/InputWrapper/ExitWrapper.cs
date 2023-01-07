using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace KingsOfSacredStar.InputWrapper
{
    internal static class ExitWrapper
    {
        private static Game sGame;

        private static bool sEscClicked;
        public static bool EscClicked{
            get
            {
                if (sEscClicked)
                {
                    sEscClicked = false;
                    return true;
                }

                return false;
            }
            private set => sEscClicked = value;
        }
        public static void Init(Game game)
        {
            sGame = game;
        }

        public static void LoadInputManager(InputManager input)
        {
            input.AddOnKeyboard(Keys.Escape, EscEvent);
        }
        // to fix

        private static void EscEvent() {
            EscClicked = true;
        }

        public static void Quit()
        {
            sGame.Exit();
        }
    }
}
