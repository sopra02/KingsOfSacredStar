using System;
using System.Collections.Generic;
using System.Linq;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.World.Unit;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World
{
    internal sealed class SpatialStructuredUnits
    {
        private readonly int mGridSize;
        private readonly Dictionary<Players, Dictionary<Point, List<IUnit>>> mUnits;

        public SpatialStructuredUnits(int gridSize)
        {
            mGridSize = gridSize;
            mUnits = new Dictionary<Players, Dictionary<Point, List<IUnit>>>();
        }

        public void Update(Dictionary<Players, List<IUnit>> unitsByPlayer)
        {
            foreach (var units in unitsByPlayer)
            {
                mUnits[units.Key] = ToSpatialDictionary(units.Value);
            }
        }

        private Dictionary<Point, List<IUnit>> ToSpatialDictionary(IEnumerable<IUnit> units)
        {
            var spatialUnits = new Dictionary<Point, List<IUnit>>();
            foreach (var unit in units)
            {
                var grid = new Point((int)Math.Floor(unit.Position.X / mGridSize), (int)Math.Floor(unit.Position.Y / mGridSize));
                if (spatialUnits.ContainsKey(grid))
                {
                    spatialUnits[grid].Add(unit);
                }
                else
                {
                    spatialUnits.Add(grid, new List<IUnit>{unit});
                }
            }

            return spatialUnits;
        }

        private List<Point> SurroundingGrids(Vector2 position, float range)
        {
            var xMin = (int)Math.Floor((position.X - range) / mGridSize);
            var yMin = (int)Math.Floor((position.Y - range) / mGridSize);
            var xMax = (int)Math.Floor((position.X + range) / mGridSize);
            var yMax = (int)Math.Floor((position.Y + range) / mGridSize);
            var surroundingGrids = new List<Point>();

            for (var x = xMin; x <= xMax; x++)
            {
                for (var y = yMin; y <= yMax; y++)
                {
                    surroundingGrids.Add(new Point(x, y));
                }
            }

            return surroundingGrids;
        }

        private IEnumerable<IUnit> UnitsFromGridList(Players player, IEnumerable<Point> grids)
        {
            var units = new List<IUnit>();

            foreach (var grid in grids)
            {
                if (mUnits[player].ContainsKey(grid))
                {
                    units.AddRange(mUnits[player][grid]);
                }
            }

            return units;
        }

        public List<IUnit> UnitsInRange(Players player, Vector2 position, float range)
        {
            var surroundingGrids = SurroundingGrids(position, range);

            var unitsInSurroundingFields = UnitsFromGridList(player, surroundingGrids);

            return unitsInSurroundingFields.Where(unit => Vector2.Distance(position, unit.Position) < range).ToList();
        }

        public IUnit NearestUnitOfType(Vector2 position, float range, UnitTypes type)
        {
            return NearestUnit(position, range, u => u.UnitType == type);
        }

        private IUnit NearestUnit(Vector2 position, float range, Predicate<IUnit> predicate)
        {
            var surroundingGrids = SurroundingGrids(position, range);

            var surroundingUnits = new List<IUnit>();
            foreach (var player in mUnits.Keys)
            {
                surroundingUnits.AddRange(UnitsFromGridList(player, surroundingGrids));
            }

            return NearestUnit(position, range, surroundingUnits, predicate);
        }

        public IUnit NearestUnit(Players player, Vector2 position, float range, Predicate<IUnit> predicate)
        {
            var surroundingGrids = SurroundingGrids(position, range);

            var surroundingUnits = UnitsFromGridList(player, surroundingGrids);

            return NearestUnit(position, range, surroundingUnits, predicate);
        }

        private static IUnit NearestUnit(Vector2 position, float range, IEnumerable<IUnit> surroundingUnits, Predicate<IUnit> predicate)
        {
            IUnit nearestUnit = null;
            float nearestDistance = 0;

            foreach (var unit in surroundingUnits.Where(unit => predicate(unit)))
            {
                var distance = Vector2.Distance(position, unit.Position);
                if ((nearestUnit == null && distance < range) || distance < nearestDistance)
                {
                    nearestUnit = unit;
                    nearestDistance = distance;
                }
            }

            return nearestUnit;
        }
    }
}