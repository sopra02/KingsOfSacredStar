using System;
using System.Collections.Generic;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.World;
using KingsOfSacredStar.World.Unit.Skills;
using KingsOfSacredStar.World.Unit.Units;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen.SkillMenu
{
    internal sealed class SkillExecuteButton : AButton
    {

        private readonly Skills mExecuteSkill;
        private readonly Hero mHero;

        public SkillExecuteButton(ContentManager content,
            int x,
            int y,
            int width,
            int height,
            string[] text,
            Skills skill) : base(content, x, y, width, height, text, boxColor: default, font: content.Load<SpriteFont>("fonts/small"))
        {
            mExecuteSkill = skill;
            if (GameState.Current.HeroesByPlayer[Players.Player] is Hero hero)
            {
                mHero = hero;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float transparency)
        {
            mText = new[] {mHero.HeroSkills.GetName(mExecuteSkill),
                mHero.HeroSkills.GetManaCost(mExecuteSkill).ToString(),
                mHero.HeroSkills.GetRemainingCooldown(mExecuteSkill).ToString()};

            base.Draw(spriteBatch, IsExecuteSkillLearned() ? transparency : 0.5f);
        }

        private bool IsExecuteSkillLearned()
        {
            return GameState.Current.HeroesByPlayer[Players.Player].HeroSkills.GetLevel(mExecuteSkill) != 0;
        }

        public override void IsClicked(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            if (IsExecuteSkillLearned())
            {
                mHero.HeroSkills.Execute(mExecuteSkill);
            }
        }
    }
}