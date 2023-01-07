using System.Collections.Generic;
using System.Linq;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.World.Unit;

namespace KingsOfSacredStar.World
{
    internal static class Costs
    {
        private static readonly Dictionary<UnitTypes, Dictionary<Resources, int>> sUnitCosts =
            new Dictionary<UnitTypes, Dictionary<Resources, int>>
            {
                {
                    UnitTypes.Gate, new Dictionary<Resources, int>
                    {
                        {Resources.Stone, 20}
                    }
                },
                {
                    UnitTypes.Wall, new Dictionary<Resources, int>
                    {
                        {Resources.Stone, 5}
                    }
                },
                {
                    UnitTypes.Swordsman, new Dictionary<Resources, int>
                    {
                        {Resources.Gold, 25}
                    }
                },
                {
                    UnitTypes.Bowman, new Dictionary<Resources, int>
                    {
                        {Resources.Gold, 15}
                    }
                },
                {
                    UnitTypes.BatteringRam, new Dictionary<Resources, int>
                    {
                        {Resources.Gold, 50}
                    }
                },
                {
                    UnitTypes.Cavalry, new Dictionary<Resources, int>
                    {
                        {Resources.Gold, 20}
                    }
                }
            };

        public static bool HasEnoughResourcesForUnit(UnitTypes unitType, Players playerId)
        {
            var enoughResources = true;
            var neededResources = sUnitCosts[unitType];
            var availableResources = GameState.Current.Resources;

            foreach (var resource in neededResources)
            {
                if (availableResources[playerId][resource.Key] < resource.Value)
                {
                    enoughResources = false;
                    break;
                }
            }

            return enoughResources;
        }

        public static bool PayUnitCosts(UnitTypes unitType, Players playerId)
        {
            var unitPayed = false;
            if (HasEnoughResourcesForUnit(unitType, playerId))
            {
                var neededResources = sUnitCosts[unitType];
                foreach (var resource in neededResources)
                {
                    GameState.Current.Resources[playerId][resource.Key] -= resource.Value;
                }

                unitPayed = true;
            }

            return unitPayed;
        }

        public static string SerializeUnitCost(UnitTypes unitType)
        {
            return string.Join("\n", sUnitCosts[unitType].Select(cost => cost.Key + " " + cost.Value));
        }
    }
}
