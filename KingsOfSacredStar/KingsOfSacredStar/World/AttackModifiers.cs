using System.Collections.Generic;
using KingsOfSacredStar.World.Unit;

namespace KingsOfSacredStar.World
{
    internal static class AttackModifiers
    {
        private static readonly Dictionary<UnitTypes, Dictionary<UnitTypes, float>> sAttackModifiers =
            new Dictionary<UnitTypes, Dictionary<UnitTypes, float>>
            {
                {
                    UnitTypes.BatteringRam, new Dictionary<UnitTypes, float>
                    {
                        {UnitTypes.Bowman, 0f},
                        {UnitTypes.BatteringRam, 0f },
                        {UnitTypes.Cavalry, 0f},
                        {UnitTypes.Gate, 5f },
                        {UnitTypes.Hero, 0f},
                        {UnitTypes.Swordsman, 0f},
                        {UnitTypes.Wall, 5f}
                    }
                },
                {
                    UnitTypes.Bowman, new Dictionary<UnitTypes, float>
                    {
                        {UnitTypes.BatteringRam, 0.25f },
                        {UnitTypes.Cavalry, 1.25f },
                        {UnitTypes.Gate, 0f },
                        {UnitTypes.Wall, 0f },
                        {UnitTypes.Swordsman, 0.75f }
                    }
                },
                {
                    UnitTypes.Cavalry, new Dictionary<UnitTypes, float>
                    {
                        {UnitTypes.BatteringRam, 2f },
                        {UnitTypes.Gate, 0f },
                        {UnitTypes.Wall, 0f }
                    }
                },
                {
                    UnitTypes.Hero, new Dictionary<UnitTypes, float>
                    {
                        {UnitTypes.BatteringRam, 2f },
                        {UnitTypes.Gate, 0f },
                        {UnitTypes.Wall, 0f }
                    }
                },
                {
                    UnitTypes.Swordsman, new Dictionary<UnitTypes, float>
                    {
                        {UnitTypes.BatteringRam, 2f },
                        {UnitTypes.Bowman, 1.5f },
                        {UnitTypes.Gate, 1f },
                        {UnitTypes.Wall, 1f }
                    }
                }
            };

        public static float GetModifier(UnitTypes attacker, UnitTypes defender)
        {
            if (sAttackModifiers.ContainsKey(attacker) && sAttackModifiers[attacker].ContainsKey(defender))
            {
                return sAttackModifiers[attacker][defender];
            }

            return 1f;
        }
    }
}
