namespace KingsOfSacredStar.World.Unit
{
    internal interface IDamageableUnit : IUnit, IDespawningUnit
    {
        int Health { get; set; }

        int MaxHealth { get; }
        bool IsHit { get; set; }
    }
}
