using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World.Unit.Skills
{
    internal interface ISkill
    {
        string Name { get; }
        int ManaCost { get; }
        int GetLevel();
        int GetRemainingCooldown();
        bool LevelUp();
        void Execute();
        void Update(Vector2 position, GameTime gameTime);
        bool IsActive { get; }
        Color Color { get; }
        float Range { get; }
    }
}