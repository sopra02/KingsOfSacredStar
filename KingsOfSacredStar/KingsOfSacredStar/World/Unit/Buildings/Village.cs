using System.Collections.Generic;
using System.Linq;
using KingsOfSacredStar.GameLogic;
using Microsoft.Xna.Framework;

namespace KingsOfSacredStar.World.Unit.Buildings
{
    internal sealed class Village : ADestroyableBuilding
    {
        public Village(Players owner, Vector2 pos, float rot, int gridSize)
            : base(ModelManager.GetInstance().Village,
                UnitTypes.Village,
                owner,
                pos,
                rot,
                gridSize,
                GetBlockedFields(),
                GetPassableFields())
        {
            MaxHealth = (int)BaseStats.sUnitStats[UnitType][StatNames.Health];
            mHitHelper = new HitHelper(this, MaxHealth);
        }

        private static Point[] GetBlockedFields()
        {
            return new[]
            {
                new Point(0, 1),
                new Point(1, 1),
                new Point(0, 0),
                new Point(1, 0),
                new Point(0, -1),
                new Point(1, -1)
            };
        }

        private static IEnumerable<Point> Surrounding(Point point) {
            return new[]
            {
                point - new Point(-1, -1),
                point - new Point(-1, 0),
                point - new Point(-1, 1),
                point - new Point(0, -1),
                point - new Point(0, 1),
                point - new Point(1, -1),
                point - new Point(1, 0),
                point - new Point(1, 1)
            };
        }

        private static bool Contains(IEnumerable<Point> point, Point toCheck)
        {
            return point.Any(x => x.Equals(toCheck));
        }

        private static Point[] GetPassableFields()
        {
            var tmp = new List<Point>();
            foreach(var x in GetBlockedFields())
            {
                foreach (var y in Surrounding(x)) {
                    if (!(Contains(GetBlockedFields(), y) || Contains(tmp.ToArray(), y)))
                        tmp.Add(y);
                }
            }
            return tmp.ToArray();
        }
    }
}
