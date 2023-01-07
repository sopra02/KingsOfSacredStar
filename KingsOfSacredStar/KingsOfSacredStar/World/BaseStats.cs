using System.Collections.Generic;
using KingsOfSacredStar.World.Unit;

namespace KingsOfSacredStar.World
{
    internal enum StatNames
    {
        Health,
        HealthRegeneration,
        Mana,
        ManaRegeneration,
        Speed,
        DamageInterval,
        BaseDamage,
        MaxLevel,
        Experience,
        ExperienceForKill,
        AttackRange
    }

    internal static class BaseStats
    {
        private const float SpeedFactor = 1.0f;

        public static readonly Dictionary<UnitTypes, Dictionary<StatNames, float>> sUnitStats =
            new Dictionary<UnitTypes, Dictionary<StatNames, float>>
            {
                {
                    UnitTypes.Hero, new Dictionary<StatNames, float>
                    {
                        {StatNames.Health, 160},
                        {StatNames.HealthRegeneration, 2},
                        {StatNames.Mana, 100},
                        {StatNames.ManaRegeneration, 1},
                        {StatNames.MaxLevel, 18},
                        {StatNames.Experience, 100},
                        {StatNames.ExperienceForKill, 10},
                        {StatNames.Speed, 1f * SpeedFactor},
                        {StatNames.DamageInterval, 80},
                        {StatNames.AttackRange, 32},
                        {StatNames.BaseDamage, 30}
                    }
                },
                {
                    UnitTypes.Gate, new Dictionary<StatNames, float>
                    {
                        {StatNames.Health, 800}
                    }
                },
                {
                    UnitTypes.Wall, new Dictionary<StatNames, float>
                    {
                        {StatNames.Health, 500}

                    }
                },
                {
                    UnitTypes.Village, new Dictionary<StatNames, float>
                    {
                        {StatNames.Health, 1500}
                    }
                },
                {
                    UnitTypes.Swordsman, new Dictionary<StatNames, float>
                    {
                        {StatNames.Health, 100},
                        {StatNames.Speed, 0.7f * SpeedFactor},
                        {StatNames.DamageInterval, 70},
                        {StatNames.AttackRange, 32},
                        {StatNames.BaseDamage, 10}
                    }
                },
                {
                    UnitTypes.Bowman, new Dictionary<StatNames, float>
                    {
                        {StatNames.Health, 30},
                        {StatNames.Speed, 0.9f * SpeedFactor},
                        {StatNames.DamageInterval, 60},
                        {StatNames.AttackRange, 128}
                    }
                },
                {
                    UnitTypes.Arrow, new Dictionary<StatNames, float>
                    {
                        {StatNames.Speed, 3f * SpeedFactor},
                        {StatNames.BaseDamage, 6}
                    }
                },
                {
                    UnitTypes.BatteringRam, new Dictionary<StatNames, float>
                    {
                        {StatNames.Health, 200},
                        {StatNames.Speed, 0.5f * SpeedFactor},
                        {StatNames.DamageInterval, 60},
                        {StatNames.AttackRange, 32},
                        {StatNames.BaseDamage, 50}
                    }
                },
                {
                    UnitTypes.Cavalry, new Dictionary<StatNames, float>
                    {
                        {StatNames.Health, 80},
                        {StatNames.Speed, 1.5f * SpeedFactor},
                        {StatNames.DamageInterval, 60},
                        {StatNames.AttackRange, 32},
                        {StatNames.BaseDamage, 12}
                    }
                }
            };

    }
}
