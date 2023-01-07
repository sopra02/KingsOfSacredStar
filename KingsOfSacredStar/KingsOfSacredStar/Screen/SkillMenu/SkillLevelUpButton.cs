using System;
using System.Collections.Generic;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.World;
using KingsOfSacredStar.World.Unit.Skills;
using KingsOfSacredStar.World.Unit.Units;
using Microsoft.Xna.Framework.Content;

namespace KingsOfSacredStar.Screen.SkillMenu
{
    internal sealed class SkillLevelUpButton : AButton
    {

        private readonly Skills mLevelUpSkill;
        private readonly Hero mHero;

        public SkillLevelUpButton(ContentManager content,
            int x,
            int y,
            int width,
            int height,
            string[] text,
            Skills skill) : base(content, x, y, width, height, text)
        {
            mLevelUpSkill = skill;
            mHero = GameState.Current.HeroesByPlayer[Players.Player];
        }

        public override void IsClicked(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
            mHero.HeroSkills.LevelUp(mLevelUpSkill);
        }
    }
}