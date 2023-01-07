using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Priority_Queue;

namespace KingsOfSacredStar.World.Unit.PathFinding
{
    /// <summary>
    /// Calculates a Path using the A* algorithm.
    /// </summary>
    internal sealed class PathFinder
    {
        private static readonly List<Vector2> sPossibleDirections = new List<Vector2>
        {
            new Vector2(-1, -1),
            new Vector2(-1, 0),
            new Vector2(-1, 1),
            new Vector2(0, -1),
            new Vector2(0, 1),
            new Vector2(1, -1),
            new Vector2(1, 0),
            new Vector2(1, 1)
        };

        private static readonly Dictionary<Vector2, Vector2[]> sBlockedDependency = new Dictionary<Vector2, Vector2[]>
        {
            { sPossibleDirections[0], new[]{ sPossibleDirections[1], sPossibleDirections[3] } },
            { sPossibleDirections[2], new[]{ sPossibleDirections[1], sPossibleDirections[4] } },
            { sPossibleDirections[5], new[]{ sPossibleDirections[3], sPossibleDirections[6] } },
            { sPossibleDirections[7], new[]{ sPossibleDirections[4], sPossibleDirections[6] } }
        };

        private readonly int mGridSize;

        private readonly SimplePriorityQueue<Point> mPaths = new SimplePriorityQueue<Point>();
        private readonly HashSet<Point> mVisited = new HashSet<Point>();
        private bool mTargetReached;
        private readonly Dictionary<Point, Point> mPathRecord = new Dictionary<Point, Point>();
        private readonly Dictionary<Point, float> mWeights = new Dictionary<Point, float>();
        private readonly Predicate<Vector2> mIsObstructed;

        public PathFinder(Predicate<Vector2> isObstructed, int gridSize)
        {
            mGridSize = gridSize;
            mIsObstructed = isObstructed;
        }


        private void Clear()
        {
            mPaths.Clear();
            mVisited.Clear();
            mPathRecord.Clear();
            mWeights.Clear();
            mTargetReached = false;
        }

        public bool TargetWillBeReached()
        {
            return mTargetReached;
        }

        /// <summary>
        /// A extension for A* to find a path to the position nearest to the target more efficiently
        /// </summary>
        public Stack<Vector2> FindBestPath(Vector2 position, Vector2 target)
        {
            var stop = NearestReachablePoint(position, target);

            if (ToGridPosition(stop) == ToGridPosition(target))
            {
                var path = AStar(position, target);
                mTargetReached = true;
                return path;
            }
            else
            {
                var path = AStar(position, stop);
                mTargetReached = false;
                return path;
            }
        }

        private Vector2 NearestReachablePoint(Vector2 position, Vector2 target)
        {
            var pTarget = ToGridPosition(target);
            var newTargets = new Stack<Point>();
            var newTarget = pTarget;
            var visited = new HashSet<Point> {pTarget};
            var useNewTarget = true;

            while (!GameState.Current.mPathZones.SameZone(position, newTarget.ToVector2() * mGridSize))
            {
                if (visited.Count > 50)
                {
                    useNewTarget = false;
                    break;
                }

                foreach (var dir in sPossibleDirections)
                {
                    var adjacent = newTarget + dir.ToPoint();
                    if (visited.Contains(adjacent))
                    {
                        continue;
                    }
                    newTargets.Push(adjacent);
                    visited.Add(adjacent);
                }

                newTarget = newTargets.Pop();
            }

            if (useNewTarget)
            {
                return newTarget.ToVector2() * mGridSize;
            }
            return target;
        }

        /// <summary>
        /// Runs A* on the specified Vectors.
        /// </summary>
        /// <param name="position">The starting position for the search.</param>
        /// <param name="target">The end position for the search.</param>
        /// <returns></returns>
        public Stack<Vector2> AStar(Vector2 position, Vector2 target)
        {
            Clear();

            var start = ToGridPosition(position);
            var bestTarget = start;
            mPaths.Enqueue(start, 0);
            mWeights[start] = 0;

            var stop = ToGridPosition(target);

            while (mPaths.Count != 0)
            {
                var currentNode = mPaths.Dequeue();
                if (currentNode == stop)
                {
                    mTargetReached = true;
                    bestTarget = currentNode;
                    break;
                }

                mVisited.Add(currentNode);

                if ((currentNode.ToVector2() - target / mGridSize).Length() <
                    (bestTarget.ToVector2() - target / mGridSize).Length())
                {
                    bestTarget = currentNode;
                }

                ExpandNode(currentNode, Octile(target));
                
            }

            return BuildStack(bestTarget, mTargetReached ? target : bestTarget.ToVector2() * mGridSize);
        }

        private Func<Point, float> Octile(Vector2 target)
        {
            var factor = Math.Sqrt(2) - 2;
            return point =>
            {
                var distance = point.ToVector2() - target / mGridSize;
                var dx = Math.Abs(distance.X);
                var dy = Math.Abs(distance.Y);
                return (float) (dx + dy + factor * Math.Min(dx, dy));
            };
        }

        private IEnumerable<Vector2> UnobstructedDirections(Point currentNode)
        {
            bool IsObstructed(Vector2 dir) => mIsObstructed((dir + currentNode.ToVector2()) * mGridSize);

            foreach (var direction in sPossibleDirections)
            {

                if (IsObstructed(direction) || (sBlockedDependency.ContainsKey(direction) && sBlockedDependency[direction].Any(IsObstructed)))
                {
                    continue;
                }

                yield return direction;
            }
        }

        private void ExpandNode(Point currentNode, Func<Point, float> heuristic)
        {
            foreach (var dir in UnobstructedDirections(currentNode))
            {
                var nextNode = new Point(currentNode.X + (int)dir.X, currentNode.Y + (int)dir.Y);
                if (mVisited.Contains(nextNode)) continue;

                var nextWeight = mWeights[currentNode] + dir.Length();

                if (mPaths.Contains(nextNode) && nextWeight >= mWeights[nextNode]) continue;

                mPathRecord[nextNode] = currentNode;
                mWeights[nextNode] = nextWeight;

                var priority = nextWeight + heuristic(nextNode);

                if (mPaths.Contains(nextNode))
                {
                    mPaths.UpdatePriority(nextNode, priority);
                }
                else
                {
                    mPaths.Enqueue(nextNode, priority);
                }
            }
        }

        private Stack<Vector2> BuildStack(Point stop, Vector2 target)
        {
            var stack = new Stack<Vector2>();

            stack.Push(target);

            Point? currentPosition = stop;
            while (currentPosition != null)
            {
                if (mPathRecord.TryGetValue(currentPosition.Value, out var newPosition))
                {
                    currentPosition = newPosition;
                    stack.Push((currentPosition.Value.ToVector2() + new Vector2(0.5f, 0.5f)) * mGridSize);
                }
                else
                {
                    currentPosition = null;
                }
            }

            if (stack.Count > 1)
            {
                // Ignore start position
                stack.Pop();
            }

            return stack;
        }

        private Point ToGridPosition(Vector2 vector)
        {
            var point = vector.ToPoint();
            return new Point((int) Math.Floor((float) point.X / mGridSize), (int) Math.Floor((float) point.Y / mGridSize));
        }
    }
}