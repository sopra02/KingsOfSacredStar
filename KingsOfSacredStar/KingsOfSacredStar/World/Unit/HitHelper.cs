using System;

namespace KingsOfSacredStar.World.Unit
{
    internal sealed class HitHelper
    {
        private const int TicksToRecover = 30;
        private readonly IDamageableUnit mDamageableUnit;
        private readonly int mMaxHealth;
        private bool mStartCounting;
        private int mCurrentDamageTick;
        private int mHealth;

        public HitHelper(IDamageableUnit damageableUnit, int health)
        {
            mDamageableUnit = damageableUnit;
            mHealth = health;
            mMaxHealth = health;
        }

        public void RegisterHit(int newHealth)
        {
            if (newHealth < mHealth)
            {
                mDamageableUnit.IsHit = true;
                mStartCounting = true;
            }
            mHealth = Math.Min(mMaxHealth, newHealth);
        }

        public int GetHealth() => mHealth;

        public bool ShouldDespawn => mHealth <= 0;

        public void Update()
        {
            if (mCurrentDamageTick == TicksToRecover)
            {
                if (mHealth > 0)
                {
                    mDamageableUnit.IsHit = false;
                    mCurrentDamageTick = 0;
                }
                mStartCounting = false;
            }

            if (mStartCounting)
            {
                mCurrentDamageTick++;
            }
        }
    }
}
