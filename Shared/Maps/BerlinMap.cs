using PolyTanks.Helpers;

namespace PolyTanks.Shared.Maps
{
    public class BerlinMap : MapBase
    {
        public override Wall[] Walls => new[]
        {
            new Wall("green", new Vector(1, -2),
                new Vector(2, 0),
                new Vector(1, -2),
                new Vector(-2, 0),
                new Vector(-1, 1)
            ),
        };

        public override float ScallingFactor => 40f;
    }
}