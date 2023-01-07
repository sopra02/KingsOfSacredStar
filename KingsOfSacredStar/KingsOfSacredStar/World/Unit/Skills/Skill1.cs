using System.Collections.Generic;
using System.Linq;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.World.Unit.Skills
{
    internal sealed class Skill1 : ASkill
    {
        public override string Name => Properties.SkillName.AoE;
        public override int ManaCost => 60 + 5 * GameState.Current.HeroesByPlayer[mPlayer].HeroSkills.GetLevel(Skills.Skill1);
        protected override int Cooldown => 6;

        private const int Damage = 50;
        private readonly SoundEffectManager mSoundEffectManager;

        public Skill1(Players player, int level, ContentManager content) : base(player, level, Color.DarkRed, 60)
        {
            mSoundEffectManager = new SoundEffectManager(content, "sounds/sword");
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

            var units = new List<IDamageableUnit>();
            mSoundEffectManager.Play(0);

            foreach (var otherPlayer in GameState.Current.UnitsByPlayer.Keys)
            {
                if (otherPlayer != mPlayer)
                {
                    units.AddRange(GameState.Current.SpatialUnitsByPlayer.UnitsInRange(Players.Ai, position, Range + 2 * GameState.Current.HeroesByPlayer[mPlayer].HeroSkills.GetLevel(Skills.Skill1))
                        .OfType<IDamageableUnit>());
                }
            }

            foreach (var unit in units)
            {
                GameState.Current.HeroesByPlayer[mPlayer].AttackedUnits.Add(unit);
                unit.Health -= (int)(Damage * GameState.Current.mDamageFactor[mPlayer] + 3 * GameState.Current.HeroesByPlayer[mPlayer].HeroSkills.GetLevel(Skills.Skill1));
            }

            IsActive = true;
        }
    }
}