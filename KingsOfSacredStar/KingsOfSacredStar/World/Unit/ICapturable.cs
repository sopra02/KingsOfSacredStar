
namespace KingsOfSacredStar.World.Unit
{
    internal interface ICapturable : IUnit
    {
        bool NeedsDrawing { get; }
        float FriendlyPercentage { get; }
        float EnemyPercentage { get; }
    }
}
