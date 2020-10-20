using System.Linq;
using PolyTanks.Helpers;

namespace PolyTanks.Shared
{
    public class Wall
    {
        public readonly string ColorCode;
        public readonly Vector Position;
        public readonly VectorGroup Bounds;

        public Wall(string colorCode, Vector position, params (int x, int y)[] bounds)
        {
            ColorCode = colorCode;
            Position = position;
            Bounds = new VectorGroup(bounds.Select(p => new Vector(p.x, p.y)).ToArray());
        }
    }
}