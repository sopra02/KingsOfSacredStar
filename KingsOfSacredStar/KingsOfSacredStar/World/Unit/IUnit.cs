using KingsOfSacredStar.GameLogic;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World.Unit
{
    internal interface IUnit
    {
        ModelManager.Model ModelType { get; }

        UnitTypes UnitType { get; }
        Vector2 Position { get; }

        void Update(GameTime gameTime);

        void IsPaused(GameTime gameTime);
        string Serialize();
        Players Owner { get; }
        Matrix RenderPosition { get; }
    }
}
