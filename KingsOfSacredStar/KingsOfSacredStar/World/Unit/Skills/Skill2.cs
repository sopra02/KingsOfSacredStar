using System;
using System.Collections.Generic;
using System.Linq;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.World.Unit.Skills
{
    internal sealed class Skill2 : ASkill
    {
        public override string Name => Properties.SkillName.Heal;
        public override int ManaCost => 40 + 3 * GameState.Current.HeroesByPlayer[mPlayer].HeroSkills.GetLevel(Skills.Skill2);
        protected override int Cooldown => 4;

        private const int Heal = 50;
        private readonly SoundEffectManager mSoundEffectManager;

        public Skill2(Players player, int level, ContentManager content) : base(player, level, Color.LightGreen, 60)
        {
            mSoundEffectManager = new SoundEffectManager(content, "sounds/heal");
        }

        public override void Execute()
        {
            mActivate = true;
        }

        public override void Update(Vector2 position, GameTime gameTime)
        {
            IsActive = false;
            if (!Requirements(gameTime))
            {
                return;
            }

            mActivate = false;
            mSoundEffectManager.Play(0);

            var units = new List<IDamageableUnit>();

            foreach (var self in GameState.Current.UnitsByPlayer.Keys)
            {
                if (self == mPlayer)
                {
                    units.AddRange(GameState.Current.SpatialUnitsByPlayer.UnitsInRange(mPlayer, position, Range + GameState.Current.HeroesByPlayer[mPlayer].HeroSkills.GetLevel(Skills.Skill2))
                        .OfType<IDamageableUnit>());
                }
            }

            foreach (var unit in units)
            {
                unit.Health = Math.Min(unit.Health + Heal + 5 * GameState.Current.HeroesByPlayer[mPlayer].HeroSkills.GetLevel(Skills.Skill2), unit.MaxHealth);
            }

            IsActive = true;
        }
    }
}