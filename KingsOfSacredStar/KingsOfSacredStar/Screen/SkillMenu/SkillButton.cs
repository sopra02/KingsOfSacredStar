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
    internal sealed class SkillButton : AButton
    {

        private readonly Skills mInformationSkill;
        private readonly Hero mHero;

        public SkillButton(ContentManager content,
            int x,
            int y,
            int width,
            int height,
            Skills skill) : base(content, x, y, width, height, new[] {"Level: 0"}, boxColor: default, font: content.Load<SpriteFont>("fonts/Menu"))
        {
            mInformationSkill = skill;
            if (GameState.Current.HeroesByPlayer[Players.Player] is Hero hero)
            {
                mHero = hero;
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float transparency)
        {
            mText = new[] {"Level: " + mHero.HeroSkills.GetLevel(mInformationSkill).ToString()};
            base.Draw(spriteBatch, transparency);
        }

        public override void IsClicked(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
        }
    }
}