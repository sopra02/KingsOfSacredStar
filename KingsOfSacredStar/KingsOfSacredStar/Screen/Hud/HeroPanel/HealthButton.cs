using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.Properties;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen.Hud.HeroPanel
{
    internal sealed class HealthButton : ABarButton
    {

        public HealthButton(ContentManager content,
            int x,
            int y,
            int width,
            int height) : base(content, x, y, width, height, Color.Red)
        {}

        public override void Draw(SpriteBatch spriteBatch, float transparency)
        {
            var hero = GameState.Current.HeroesByPlayer[Players.Player];
            mText = new[] {$"{hero.Health}/{hero.MaxHealth} {HealthManaXPButton.HP}"};
            SetMultiplier((double)hero.Health / hero.MaxHealth);
            base.Draw(spriteBatch, transparency);
        }
    }
}