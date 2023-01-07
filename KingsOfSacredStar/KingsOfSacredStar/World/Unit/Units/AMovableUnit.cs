using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KingsOfSacredStar.GameLogic;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World.Unit.Units
{
    /// <summary>
    /// Test class to test moving
    /// Entities.
    /// </summary>
    internal abstract class AMovableUnit : AUnit, IMovableUnit
    {

        protected float mSpeed;

        private readonly Vector2 mOffset;
        private Stack<Vector2> mPath = new Stack<Vector2>(0);
        private int mWallCollisionCounter;
        private readonly int mMaxWallCollisions;


        protected AMovableUnit(ModelManager.Model model,
            UnitTypes unitType,
            Players owner,
            Vector2 pos,
            float rot,
            int gridSize)
            : base(model, unitType, owner, new Vector2(pos.X, pos.Y), gridSize, rot)
        {
            mOffset = new Vector2(gridSize / 2f);
            mWallCollisionCounter = 0;
            mMaxWallCollisions = 90;
        }

        public BoundingBox GetBoundingBox()
        {
            var orientedBoundingBox = ModelType.GetBoundingBox(RenderPosition);
            return BoundingBox.CreateFromPoints(orientedBoundingBox.GetCorners());
        }

        public virtual Task<bool> SetMovingTarget(Vector2? target)
        {
            if (!target.HasValue)
            {
                mPath = new Stack<Vector2>(0);
                return Task.FromResult(true);
            }

            return Task.Run(() =>
            {
                try
                {
                    var pathFinder = GameState.Current.PathFinder;
                    mPath = pathFinder.FindBestPath(Position - mOffset, target.Value);
                    return pathFinder.TargetWillBeReached();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    mPath = new Stack<Vector2>(0);
                    return false;
                }
            });
        }

        /// <summary>
        /// Code is probably not perfect, but makes turning smoother.
        /// </summary>
        private static float NormalizeRotation(float targetRotation)
        {
            // floor modulus
            var modRotation = ((targetRotation % MathHelper.TwoPi) + MathHelper.TwoPi) % MathHelper.TwoPi;
            return modRotation > Math.PI ? modRotation - MathHelper.TwoPi : modRotation;
        }

        public void Reposition(Vector2 direction, bool moving, bool canBeIgnored)
        {
            if (moving)
            {
                if (!direction.Equals(Vector2.Zero))
                {
                    direction.Normalize();
                }

                var speed = mSpeed / 2;
                if (canBeIgnored)
                {
                    speed *= 1 - (mWallCollisionCounter / mMaxWallCollisions);
                }

                Position += direction * speed;
            }
            else
            {
                Position += direction;
                if (!direction.Equals(Vector2.Zero))
                {
                    direction.Normalize();
                }
            }

            if (mPath.Count == 1)
            {
                var target = mPath.Pop();
                target += direction * (mSpeed / 2);
                mPath.Push(target);
            }
        }

        public void WallCollision()
        {
            if (mPath.Count <= 1 ) return;
            mWallCollisionCounter += 3;

            if (mWallCollisionCounter > mMaxWallCollisions) return;
            mWallCollisionCounter = 0;

            while (mPath.Count > 1)
            {
                mPath.Pop();
            }

            var target = mPath.Pop();

            SetMovingTarget(target);
        }

        public override void Update(GameTime gameTime)
        {
            if (mWallCollisionCounter > 0)
            {
                mWallCollisionCounter -= 1;
            }

            if (mPath.Count == 0) return;

                var targetDirection = mPath.Peek() + mOffset - Position;

                if (targetDirection.Length() < mSpeed)
                {
                    Position = mPath.Pop() + mOffset;
                }
                else
                {
                    var newRotation = (float) Math.Atan2(targetDirection.X, targetDirection.Y);
                    Rotation += NormalizeRotation(newRotation - Rotation) * 0.1f;

                    targetDirection.Normalize();
                    Position += targetDirection * mSpeed;
                }
        }
    }
}
