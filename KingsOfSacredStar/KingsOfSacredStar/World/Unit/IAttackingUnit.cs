namespace KingsOfSacredStar.World.Unit
{
    internal interface IAttackingUnit : IUnit
    {
        void SetTarget(IDamageableUnit target, bool follow);
        int GetRange();
        bool HasTarget();
    }
}
