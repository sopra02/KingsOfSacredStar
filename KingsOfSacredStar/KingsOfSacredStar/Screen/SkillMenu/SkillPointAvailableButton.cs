using System;
using System.Collections.Generic;
using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen.SkillMenu
{
    internal sealed class SkillPointAvailableButton : AButton
    {


        public SkillPointAvailableButton(ContentManager content,
            int x,
            int y,
            int width,
            int height
            ) : base(content, x, y, width, height, new[] {Properties.SkillingMenu.Available, "0"})
        {
        }
        public override void Draw(SpriteBatch spriteBatch, float transparency)
        {
            mText = new[] {Properties.SkillingMenu.Available, GameState.Current.HeroesByPlayer[Players.Player].HeroSkills.GetSkillPoints().ToString()};
            base.Draw(spriteBatch, transparency);

        }
        public override void IsClicked(Action<List<Screen>> addScreens, Action<List<Screen>> removeScreens)
        {
        }
    }
}