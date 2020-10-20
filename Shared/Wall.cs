using PolyTanks.Helpers;

namespace PolyTanks.Shared
{
    public class Wall
    {
        public readonly string ColorCode;
        public readonly Vector Position;
        public readonly VectorGroup Bounds;

        public Wall(string colorCode, Vector position, params Vector[] bounds)
        {
            ColorCode = colorCode;
            Position = position;
            Bounds = new VectorGroup(bounds);
        }
    }
}