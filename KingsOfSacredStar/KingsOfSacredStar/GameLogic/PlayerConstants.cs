using System;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.GameLogic
{
    internal static class PlayerConstants
    {
        public static readonly Color[] sPlayerColors =
        {
            // Sorted after Players enum
            Color.Green,
            Color.Blue,
            Color.Red
        };

        public static readonly Players[] sPlayers = (Players[]) Enum.GetValues(typeof(Players));
    }
}
