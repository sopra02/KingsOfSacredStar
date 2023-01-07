using System;
using System.Collections.Generic;
using KingsOfSacredStar.GameLogic;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World.Unit.PathFinding
{
    internal sealed class CollisionHandler
    {
        private readonly int mGridSize;
        private readonly Dictionary<Players, List<IUnit>> mUnits;
        private readonly Predicate<Vector2> mBlocked;
        private bool mReversedUnits;

        public CollisionHandler(int gridSize, Dictionary<Players, List<IUnit>> units, Predicate<Vector2> blocked)
        {
            mGridSize = gridSize;
            mUnits = units;
            mBlocked = blocked;
            mReversedUnits = false;
        }

        public void Update()
        {
            var clusteredMap = new Dictionary<Point, List<IMovableUnit>>();
            var units = GenerateUnitList();

            foreach (var newUnit in units)
            {
                HandleUnitCollision(clusteredMap, newUnit);

                CalculateClusteredMap(clusteredMap, newUnit);
            }

            HandleWallCollision(clusteredMap);
        }

        private void HandleUnitCollision(IReadOnlyDictionary<Point, List<IMovableUnit>> clusteredMap, IMovableUnit newUnit)
        {
            var newUnitBox = newUnit.GetBoundingBox();
            var fields = GetSurroundingFields(newUnitBox);
            foreach (var field in fields)
            {
                if (clusteredMap.ContainsKey(field))
                {
                    foreach (var unit in clusteredMap[field])
                    {
                        if (newUnitBox.Intersects(unit.GetBoundingBox()))
                        {
                            RepositionFromUnit(newUnit, unit);
                            break;
                        }
                    }
                }
            }
        }

        private void CalculateClusteredMap(IDictionary<Point, List<IMovableUnit>> clusteredMap, IMovableUnit newUnit)
        {
            var newUnitBox = newUnit.GetBoundingBox();
            var fields = GetSurroundingFields(newUnitBox);

            foreach (var field in fields)
            {
                if (clusteredMap.ContainsKey(field))
                {
                    clusteredMap[field].Add(newUnit);
                }
                else
                {
                    clusteredMap.Add(field, new List<IMovableUnit> { newUnit });
                }
            }
        }

        private void HandleWallCollision(Dictionary<Point, List<IMovableUnit>>  clusteredMap)
        {
            foreach (var field in clusteredMap)
            {
                if (mBlocked(field.Key.ToVector2() * mGridSize))
                {
                    foreach (var unit in field.Value)
                    {
                        unit.WallCollision();
                        RepositionFromWall(unit, field.Key);
                    }
                }
            }
        }

        private IEnumerable<IMovableUnit> GenerateUnitList()
        {
            var units = new List<IMovableUnit>();

            foreach (var entry in mUnits)
            {
                foreach (var unit in entry.Value)
                {
                    if (unit is IMovableUnit movableUnit)
                    {
                        units.Add(movableUnit);
                    }
                }
            }

            if (mReversedUnits)
            {
                units.Reverse();
            }

            mReversedUnits = !mReversedUnits;

            return units;
        }

        /// <summary>
        /// For some reason z of the BoundingBox is y in our game
        /// </summary>
        private IEnumerable<Point> GetSurroundingFields(BoundingBox boundingBox)
        {
            var fields = new List<Point>();
            var xMin = (int) Math.Floor(Math.Min(boundingBox.Min.X, boundingBox.Max.X) / mGridSize);
            var xMax = (int) Math.Floor(Math.Max(boundingBox.Min.X, boundingBox.Max.X) / mGridSize);
            var yMin = (int) Math.Floor(Math.Min(boundingBox.Min.Z, boundingBox.Max.Z) / mGridSize);
            var yMax = (int) Math.Floor(Math.Max(boundingBox.Min.Z, boundingBox.Max.Z) / mGridSize);

            for (var x = xMin; x <= xMax; x++)
            {
                for (var y = yMin; y <= yMax; y++)
                {
                    fields.Add(new Point(x, y));
                }
            }

            return fields;
        }

        private static void RepositionFromUnit(IMovableUnit movingUnit, IUnit standingUnit)
        {
            var dir = new Vector2(movingUnit.Position.X - standingUnit.Position.X, movingUnit.Position.Y - standingUnit.Position.Y);
            if (dir.Equals(Vector2.Zero))
            {
                dir = new Vector2(0, -1);
            }
            var unstuckDir = new Vector2((float)(0.98 * dir.X - 0.17 * dir.Y), (float)(0.17 * dir.X + 0.98 * dir.Y));
            movingUnit.Reposition(unstuckDir, true, true);
        }

        
        /// <summary>
        /// For some reason z of the BoundingBox is y in our game
        /// </summary>
        private void RepositionFromWall(IMovableUnit unit, Point field)
        {
            var boundingBox = unit.GetBoundingBox();

            var unitLeftWall = Math.Min(boundingBox.Min.X, boundingBox.Max.X);
            var unitUpperWall = Math.Min(boundingBox.Min.Z, boundingBox.Max.Z);
            var unitRightWall = Math.Max(boundingBox.Min.X, boundingBox.Max.X);
            var unitLowerWall = Math.Max(boundingBox.Min.Z, boundingBox.Max.Z);

            var fieldLeftWall = field.X * mGridSize;
            var fieldUpperWall = field.Y * mGridSize;
            var fieldRightWall = (field.X + 1) * mGridSize;
            var fieldLowerWall = (field.Y + 1) * mGridSize;

            var leftDist = fieldLeftWall - unitRightWall;
            var upperDist = fieldUpperWall - unitLowerWall;
            var rightDist = fieldRightWall - unitLeftWall;
            var lowerDist = fieldLowerWall - unitUpperWall;

            var xDirection = leftDist;
            if (Math.Abs(rightDist) < Math.Abs(leftDist))
            {
                xDirection = rightDist;
            }

            var yDirection = upperDist;
            if (Math.Abs(lowerDist) < Math.Abs(upperDist))
            {
                yDirection = lowerDist;
            }

            unit.Reposition(Math.Abs(xDirection) < Math.Abs(yDirection)
                    ? new Vector2(xDirection, 0)
                    : new Vector2(0, yDirection),
                false,
                false);
        }
    }
}