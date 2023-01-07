using System;
using KingsOfSacredStar.GameLogic;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World.Unit.Skills
{
    internal abstract class ASkill : ISkill
    {
        public abstract string Name { get; }
        public abstract int ManaCost { get; }
        protected abstract int Cooldown { get; }
        public bool IsActive { get; protected set; }
        public Color Color { get; }
        public float Range { get; }

        protected readonly Players mPlayer;
        protected bool mActivate;
        private int mLevel;
        private int mRemainingCooldown;

        protected ASkill(Players player, int level, Color skillColor, float range)
        {
            mLevel = level;
            mPlayer = player;
            mRemainingCooldown = 0;
            Color = skillColor;
            Range = range;
        }

        public int GetLevel()
        {
            return mLevel;
        }

        public bool LevelUp()
        {
            mLevel += 1;
            return true;

        }

        public int GetRemainingCooldown()
        {
            return (int) Math.Ceiling(mRemainingCooldown / 1000f);
        }

        public abstract void Execute();

        public abstract void Update(Vector2 position, GameTime gameTime);

        protected bool Requirements(GameTime gameTime)
        {
            var hero = GameState.Current.HeroesByPlayer[mPlayer];

            if (mRemainingCooldown > 0)
            {
                mRemainingCooldown -= (int) gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            if (!mActivate || hero.Mana < ManaCost || mRemainingCooldown > 0)
            {
                mActivate = false;
                return false;
            }

            hero.Mana -= ManaCost;
            mRemainingCooldown = Cooldown * 1000;

            return true;
        }
    }
}
