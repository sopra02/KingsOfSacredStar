using KingsOfSacredStar.GameLogic;
using KingsOfSacredStar.Properties;
using KingsOfSacredStar.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace KingsOfSacredStar.Screen.Hud.HeroPanel
{
    internal sealed class ManaButton : ABarButton
    {
        public ManaButton(ContentManager content,
            int x,
            int y,
            int width,
            int height) : base(content, x, y, width, height, Color.Blue)
        {}

        public override void Draw(SpriteBatch spriteBatch, float transparency)
        {
            var hero = GameState.Current.HeroesByPlayer[Players.Player];
            mText = new[] {$"{hero.Mana}/{hero.MaxMana} {HealthManaXPButton.Mana}"};
            SetMultiplier((double)hero.Mana / hero.MaxMana);
            base.Draw(spriteBatch, transparency);
        }
    }
}