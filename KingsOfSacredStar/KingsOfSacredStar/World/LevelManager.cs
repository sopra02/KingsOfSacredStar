using System;
using KingsOfSacredStar.World.Unit;

namespace KingsOfSacredStar.World
{
    internal sealed class LevelManager
    {

        public int Experience { get; private set; }
        public int CurrentLevel { get; private set; }

        public LevelManager()
        {
            CurrentLevel = 0;
        }

        public LevelManager(int level, int experience)
        {
            CurrentLevel = level;
            Experience = experience;
        }

        public int LevelHealth()
        {
            var baseStat = (int) BaseStats.sUnitStats[UnitTypes.Hero][StatNames.Health];
            return baseStat + CurrentLevel * 20;
        }

        public int LevelHealthRegeneration()
        {
            var baseStat = (int)BaseStats.sUnitStats[UnitTypes.Hero][StatNames.HealthRegeneration];
            return baseStat * (1 + (int)Math.Round(CurrentLevel * 0.2));
        }

        public int LevelMana()
        {
            var baseStat = (int) BaseStats.sUnitStats[UnitTypes.Hero][StatNames.Mana];
            return baseStat + CurrentLevel * 10;
        }

        public int LevelAttack()
        {
            var baseStat = (int)BaseStats.sUnitStats[UnitTypes.Hero][StatNames.BaseDamage];
            return baseStat + CurrentLevel * 5;
        }

        public int LevelManaRegeneration()
        {
            var baseStat = (int)BaseStats.sUnitStats[UnitTypes.Hero][StatNames.ManaRegeneration];
            return baseStat * (1 + (int)Math.Round(CurrentLevel * 0.2));
        }

        public int XpNeededForNextLevel()
        {
            var baseStat = (int) BaseStats.sUnitStats[UnitTypes.Hero][StatNames.Mana];
            return baseStat + CurrentLevel * 31;
        }

        public bool IncreaseExperience(int amount)
        {
            var newExperience = Experience + amount;
            var levelUp = false;

            if (newExperience <= XpNeededForNextLevel())
            {
                Experience = newExperience;
            }
            else
            {
                Experience = newExperience - XpNeededForNextLevel();
                CurrentLevel += 1;
                levelUp = true;
            }

            if (CurrentLevel >= (int) BaseStats.sUnitStats[UnitTypes.Hero][StatNames.MaxLevel])
            {
                CurrentLevel = (int) BaseStats.sUnitStats[UnitTypes.Hero][StatNames.MaxLevel];
                Experience = 0;
                levelUp = false;
            }

            return levelUp;
        }
    }
}
