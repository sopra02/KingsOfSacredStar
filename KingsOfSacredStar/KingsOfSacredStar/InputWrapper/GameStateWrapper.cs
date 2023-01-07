using System.Linq;
using KingsOfSacredStar.World;
using KingsOfSacredStar.World.Unit;
using KingsOfSacredStar.World.Unit.Buildings;
using Microsoft.Xna.Framework.Input;

namespace KingsOfSacredStar.InputWrapper
{
    internal static class GameStateWrapper
    {
        private static InputManager sInput;
        public static void LoadInputManager(InputManager input)
        {
            sInput = input;
            input.AddOnKeyboard(Keys.P, TogglePauseState);
            input.AddOnKeyboard(Keys.O, ToggleSelectedGateOpen);
        }


        private static void TogglePauseState()
        {
            GameState.Current.IsPaused = !GameState.Current.IsPaused;
            sInput.ClipMouse(!GameState.Current.IsPaused);
        }

        public static void SetPause(bool pause)
        {
            GameState.Current.IsPaused = pause;
            sInput.ClipMouse(!pause);
        }

        public static void StartGame()
        {
            SetPause(false);
        }

        private static void ToggleSelectedGateOpen()
        {
            foreach (var unit in GameState.Current.SelectedEntities.OfType<ABuilding>())
            {
                if (unit.UnitType != UnitTypes.Gate)
                {
                    continue;
                }
                var gate = (Gate) unit;
                gate.SetGateState(!gate.IsOpen);
            }
        }
    }
}