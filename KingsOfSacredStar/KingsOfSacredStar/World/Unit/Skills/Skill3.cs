using System.Collections.Generic;
using System.Linq;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.Sound;
using KingsOfSacredStar.World.Unit.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.World.Unit.Skills
{
    internal sealed class Skill3: ASkill
    {
        public override string Name => Properties.SkillName.Ult;
        public override int ManaCost => 20;
        protected override int Cooldown => 2;

        private readonly int mGridSize;
        private const int RestTimeInMilliseconds = 200;
        private int mRemainingRestTime;
        private readonly SoundEffectManager mSoundEffectManager;

        public Skill3(Players player, int level, int gridSize, ContentManager content) : base(player, level, Color.Transparent, 100)
        {
            mSoundEffectManager = new SoundEffectManager(content, "sounds/arrowrain");
            mGridSize = gridSize;
            IsActive = false;
        }

        public override void Execute()
        {

            if (!mActivate)
            {
                mActivate = true;
                mRemainingRestTime = 0;
            }
            else
            {
                mActivate = false;
            }

        }

        public override void Update(Vector2 position, GameTime gameTime)
        {
            if (!Requirements(gameTime))
            {
                return;
            }

            var passedTime = (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            mSoundEffectManager.Play(0);

            if (mRemainingRestTime - passedTime > 0)
            {
                mRemainingRestTime -= passedTime;
                return;
            }

            mRemainingRestTime += RestTimeInMilliseconds - passedTime;

            var units = new List<IDamageableUnit>();

            foreach (var player in GameState.Current.UnitsByPlayer.Keys)
            {
                if (player == mPlayer || player == Players.Global)
                {
                    continue;
                }

                units.AddRange(GameState.Current.SpatialUnitsByPlayer.UnitsInRange(player, position, Range + 6 * GameState.Current.HeroesByPlayer[mPlayer].HeroSkills.GetLevel(Skills.Skill3)).OfType<IDamageableUnit>());
            }

            foreach (var unit in units)
            {
                var arrow = new Arrow(mPlayer, position / mGridSize, 0, mGridSize, unit.Position);
                GameState.Current.mUnitsToAddNextTick.Enqueue(arrow);
                GameState.Current.HeroesByPlayer[mPlayer].AttackedUnits.Add(unit);
            }
        }
    }
}