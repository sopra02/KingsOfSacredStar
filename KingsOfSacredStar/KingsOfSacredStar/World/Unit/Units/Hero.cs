using System;
using System.Collections.Generic;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.Sound;
using KingsOfSacredStar.World.Unit.Skills;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.World.Unit.Units
{
    internal sealed class Hero : ATrackingUnit, IDamageableUnit
    {
        private int mTotalGameSeconds;
        public SkillManager HeroSkills { get; }

        public int Experience => mLevelManager.Experience;
        public int ExperienceNextLevel => mLevelManager.XpNeededForNextLevel();
        public bool ForRemoval => false;

        public int Health
        {
            get => mHitHelper.GetHealth();
            set => mHitHelper.RegisterHit(value);
        }

        public int MaxHealth => mLevelManager.LevelHealth();
        public bool IsHit { get; set; }

        private readonly HitHelper mHitHelper;
        public int Mana { get; set; }
        public int MaxMana => mLevelManager.LevelMana();
        public HashSet<IDamageableUnit> AttackedUnits { get; }

        public Color ActiveEffectColor { get; private set; }
        public float ActiveEffectRange { get; private set; }
        private float mPrecisionEffectAlpha;

        private readonly LevelManager mLevelManager;
        private readonly float mGridSize;
        private readonly SoundEffectManager mSoundEffectManager;
        private readonly Matrix mDeadPosition;

        public void Respawn(Vector2 pos)
        {
            Health = MaxHealth;
            Position = pos * mGridSize;
        }

        public Hero(Players owner, Vector2 pos, float rot, int gridSize, ContentManager content)
            : base(ModelManager.GetInstance().Hero, UnitTypes.Hero, owner, pos, rot, gridSize)
        {
            mGridSize = gridSize;
            mLevelManager = new LevelManager();
            Mana = MaxMana;
            mHitHelper = new HitHelper(this, MaxHealth);
            AttackedUnits = new HashSet<IDamageableUnit>();
            HeroSkills = new SkillManager(owner, content);
            SetBaseStatsTrackingUnit();
            mDeadPosition = 
                Matrix.CreateRotationZ(-MathHelper.PiOver2) *
                Matrix.CreateTranslation(-mGridSize, -mGridSize / 4, 0);
        }

        public Hero(Players owner, Vector2 pos, float rot, int gridSize,int mana, int health, LevelManager levelManager, SkillManager skillManager, ContentManager content)
            : base(ModelManager.GetInstance().Hero, UnitTypes.Hero, owner, pos, rot, gridSize)
        {
            mLevelManager = levelManager;
            mHitHelper = new HitHelper(this, MaxHealth);
            AttackedUnits = new HashSet<IDamageableUnit>();
            HeroSkills = skillManager;
            Mana = mana;
            Health = health;
            SetBaseStatsTrackingUnit();
            mSoundEffectManager = new SoundEffectManager(content, "sounds/level_up");
        }

        public override string Serialize()
        {
            return base.Serialize() + " " + Health + " " + Mana + " " + mLevelManager.CurrentLevel + " "+ mLevelManager.Experience + " " + HeroSkills.Serialize();
        }

        private void RecalculateEffectColor()
        {
            const float fadeAmount = 0.01f;
            if (HeroSkills.ActiveSkill != null)
            {
                ActiveEffectColor = HeroSkills.ActiveSkill.Color;
                mPrecisionEffectAlpha = ActiveEffectColor.A / (float)byte.MaxValue;
                ActiveEffectRange = HeroSkills.ActiveSkill.Range;
            }
            else
            {
                mPrecisionEffectAlpha -= fadeAmount;
                if (mPrecisionEffectAlpha < 0)
                {
                    mPrecisionEffectAlpha = 0;
                    ActiveEffectColor = Color.Transparent;
                }
                else
                {
                    var currentColor = ActiveEffectColor;
                    currentColor.A = (byte)(mPrecisionEffectAlpha * byte.MaxValue);
                    ActiveEffectColor = currentColor;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            HeroSkills.Update(Position, gameTime);
            RecalculateEffectColor();
            mHitHelper.Update();
            base.Update(gameTime);
            StoreLastAttacked();
            UpdateEverySecond(gameTime);
        }

        private void StoreLastAttacked()
        {
            if (!AttackedUnits.Contains(mLastAttacked))
            {
                AttackedUnits.Add(mLastAttacked);
            }
        }

        private void UpdateEverySecond(GameTime gameTime)
        {
            if ((int) gameTime.TotalGameTime.TotalSeconds == mTotalGameSeconds)
            {
                return;
            }

            IncreaseHealth(mLevelManager.LevelHealthRegeneration());
            IncreaseMana(mLevelManager.LevelManaRegeneration());
            mTotalGameSeconds = (int) gameTime.TotalGameTime.TotalSeconds;


        }

        private void IncreaseMana(int amount)
        {
            Mana = Math.Min(Mana + amount, MaxMana);
        }

        private void IncreaseHealth(int amount)
        {
            Health = Math.Min(Health + amount, MaxHealth);
        }

        public void KilledAUnit()
        {
            if (mLevelManager.IncreaseExperience((int) BaseStats.sUnitStats[UnitTypes.Hero][StatNames.ExperienceForKill]))
            {
                mSoundEffectManager.Play(0);
                Health = MaxHealth;
                Mana = MaxMana;
                HeroSkills.AddSkillPoint();
                mBaseDamage = mLevelManager.LevelAttack();
            }
        }

        public override Matrix RenderPosition
        {
            get
            {
                if (Health <= 0)
                {
                    return mDeadPosition * base.RenderPosition;
                }

                return base.RenderPosition;
            }
        }
    }
}
