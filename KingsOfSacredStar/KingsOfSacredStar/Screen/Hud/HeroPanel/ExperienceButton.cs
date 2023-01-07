using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.Properties;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen.Hud.HeroPanel
{
    internal sealed class ExperienceButton : ABarButton
    {
        public ExperienceButton(ContentManager content,
            int x,
            int y,
            int width,
            int height) : base(content, x, y, width, height, Color.Yellow)
        {}

        public override void Draw(SpriteBatch spriteBatch, float transparency)
        {
            var hero = GameState.Current.HeroesByPlayer[Players.Player];
            mText = new[] {$"{hero.Experience}/{hero.ExperienceNextLevel} {HealthManaXPButton.XP}"};
            SetMultiplier((double)hero.Experience / hero.ExperienceNextLevel);
            base.Draw(spriteBatch, transparency);
        }
    }
}