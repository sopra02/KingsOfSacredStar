using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World.Unit.PathFinding
{
    internal sealed class PathFindingZones
    {
        private readonly int mSizeX;
        private readonly int mSizeY;
        private readonly int mGridSize;
        private readonly int[,] mZones;
        private readonly HashSet<int> mUsedZoneNumbers;
        private int mCurrentZoneNumber;

        private readonly List<Point> mSurroundingGrids = new List<Point>
        {
            new Point(-1, -1),
            new Point(-1, 0),
            new Point(-1, 1),
            new Point(0, -1),
            new Point(0, 1),
            new Point(1, -1),
            new Point(1, 0),
            new Point(1, 1)
        };

        private readonly List<Point> mDirectAdjacent = new List<Point>
        {
            new Point(-1, 0),
            new Point(0, -1),
            new Point(0, 1),
            new Point(1, 0)

        };

        public PathFindingZones(int sizeX, int sizeY, int gridSize)
        {
            mSizeX = sizeX;
            mSizeY = sizeY;
            mGridSize = gridSize;
            mZones = new int[sizeX, sizeY];

            for (var x = 0; x < sizeX; x++)
            {
                for (var y = 0; y < sizeY; y++)
                {
                    mZones[x,y] = 1;
                }
            }

            mUsedZoneNumbers = new HashSet<int> {0, 1};
            mCurrentZoneNumber = 1;
        }

        /// <summary>
        /// potentially divide the area
        /// </summary>
        /// <param name="x">same as in the mMap of the GameState</param>
        /// <param name="y">same as in the mMap of the GameState</param>
        public void Block(int x, int y)
        {
            var pos = new Point(x,y);
            SetZone(pos, 0);

            var newZonePositions = new List<Point>();
            foreach (var grid in mSurroundingGrids)
            {
                var adjacent = pos + grid;
                if (GetZone(adjacent) == 0)
                {
                    continue;
                }
                newZonePositions.Add(adjacent);
            }

            if (newZonePositions.Count > 0)
            {
                RepairMap(newZonePositions);
            }
        }

        /// <summary>
        /// potentially combine zones
        /// </summary>
        /// <param name="x">same as in the mMap of the GameState</param>
        /// <param name="y">same as in the mMap of the GameState</param>
        public void Unblock(int x, int y)
        {
            var pos = new Point(x, y);

            var adjacentGrids = new List<Point>();

            foreach (var dir in mSurroundingGrids)
            {
                var adjacent = pos + dir;
                if (GetZone(adjacent) == 0)
                {
                    continue;
                }
                adjacentGrids.Add(adjacent);
            }

            if (adjacentGrids.Count == 0)
            {
                SetZone(pos, NewZoneNumber());
                return;
            }

            var newZoneNumber = GetZone(adjacentGrids[0]);
            SetZone(pos, newZoneNumber);

            adjacentGrids.RemoveAll(p => GetZone(p) == newZoneNumber);

            while (adjacentGrids.Count > 0)
            {
                mUsedZoneNumbers.Remove(GetZone(adjacentGrids[0]));
                SetConnectedGrids(adjacentGrids[0], newZoneNumber);
                adjacentGrids.RemoveAll(p => GetZone(p) == newZoneNumber);
            }
        }

        /// <summary>
        /// true, if the areas of the player and the target are connected
        /// </summary>
        /// <param name="player">actual position of the player</param>
        /// <param name="target">actual position of the target</param>
        /// <returns></returns>
        public bool SameZone(Vector2 player, Vector2 target)
        {
            var px = (int) Math.Floor(player.X / mGridSize) + 1 + mSizeX / 2;
            var py = (int) Math.Floor(player.Y / mGridSize) + 1 + mSizeX / 2;
            var tx = (int) Math.Floor(target.X / mGridSize) + 1 + mSizeX / 2;
            var ty = (int) Math.Floor(target.Y / mGridSize) + 1 + mSizeX / 2;
            var aPlayer = new Point(px, py);
            var aTarget = new Point(tx, ty);

            return GetZone(aPlayer) == GetZone(aTarget);
        }

        private int GetZone(Point pos)
        {
            if (pos.X < 0 || pos.X >= mSizeX || pos.Y < 0 || pos.Y >= mSizeY)
            {
                return 0;
            }

            return mZones[pos.X, pos.Y];
        }

        private void SetZone(Point pos, int value)
        {
            if (pos.X < 0 || pos.X >= mSizeX || pos.Y < 0 || pos.Y >= mSizeY)
            {
                return;
            }

            mZones[pos.X, pos.Y] = value;
        }

        private int NewZoneNumber()
        {
            while (mUsedZoneNumbers.Contains(mCurrentZoneNumber))
            {
                mCurrentZoneNumber += 1;
            }

            mUsedZoneNumbers.Add(mCurrentZoneNumber);
            return mCurrentZoneNumber;
        }

        private void RepairMap(IReadOnlyList<Point> grids)
        {

            var oldZone = GetZone(grids[0]);
            var firstPoint = grids[0];
            var otherZones = new List<Point>();

            var pathFinder = new PathFinder(v => GetZone(v.ToPoint()) != oldZone, 1);

            foreach (var grid in grids)
            {
                pathFinder.AStar(firstPoint.ToVector2(), grid.ToVector2());

                if (!pathFinder.TargetWillBeReached())
                {
                    otherZones.Add(grid);
                }
            }

            while (otherZones.Count != 0)
            {
                var grid = otherZones[0];
                var newZone = NewZoneNumber();
                SetConnectedGrids(grid, newZone);

                otherZones.RemoveAll(n => GetZone(n) == newZone);
            }
        }

        private void SetConnectedGrids(Point start, int newZone)
        {
            var oldZone = GetZone(start);
            var extendableGrids = new Stack<Point>();
            SetZone(start, newZone);
            extendableGrids.Push(start);

            while (extendableGrids.Count != 0)
            {
                var grid = extendableGrids.Pop();

                foreach (var direction in mDirectAdjacent)
                {
                    var adjacent = grid + direction;

                    if (oldZone != GetZone(adjacent))
                    {
                        continue;
                    }
                    SetZone(adjacent, newZone);
                    extendableGrids.Push(adjacent);
                }
            }
        }
    }
}