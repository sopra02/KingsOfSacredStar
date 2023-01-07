using System;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework.Input;

namespace KingsOfSacredStar.InputWrapper
{
    internal static class BuildManagerWrapper
    {

        public static BuildManager BuildManager { get; private set; }

        public static void Init(BuildManager buildManager)
        {
            BuildManager = buildManager;
        }


        public static void LoadInputManager(InputManager input)
        {
            input.AddOnKeyboard(Keys.Q, RotateBuildingClockwise90Degrees);
        }


        private static void RotateBuildingClockwise90Degrees()
        {
            BuildManager.RotateBuilding((float) Math.PI / 2);
        }
    }
}