using PolyTanks.Helpers;

namespace PolyTanks.Shared.Maps
{
    public class BerlinMap : MapBase
    {
        public override Wall[] Walls => new[]
        {
            new Wall("gray", new Vector(0, -5),
                (0, 0),
                (1, -1), (1, -1), (4, 0), (7, 0), (8, -1), (9, -1), (10, 0), (30, -20),
                (-30, -20), (-10, 0), (-8, 0), (-7, -1), (-6, -1), (-5, 0), (-3, -1), (-1, 0)
            ),

            new Wall("gray", new Vector(10, 0),
                (0, -1), (0, -2), (1, -4), (0, -5), (20, -25),
                (20, 25), (0, 5), (1, 4), (1, 3), (2, 2), (2, 1)
            ),

            new Wall("gray", new Vector(0, 5),
                (1, 2), (2, 1), (4, 1), (5, 0), (7, 0), (8, 1), (9, 1), (10, 0), (30, 20),
                (-30, 20), (-10, 1), (-8, 1), (-7, 0), (-5, 0), (-3, 1), (-2, 1), (-1, 2)
            ),

            new Wall("gray", new Vector(-10, 0),
                (0, -1), (0, -2), (-1, -4), (0, -5), (-20, -25),
                (-20, 25), (1, 6), (-1, 4), (-1, 3), (-2, 2), (-2, 1)
            ),

            new Wall("seagreen", new Vector(1, -2), //1
                (2, 0),
                (1, -2),
                (-2, 0),
                (-1, 1)
            ),

            new Wall("seagreen", new Vector(5, -3), //2
                (1, 0), (1, -1), (-1, -1), (-1, 0)
            ),

            new Wall("seagreen", new Vector(6, -1), //3
                (2, 0), (0, -1), (-1, -1), (0, 0)
            ),

            new Wall("seagreen", new Vector(9, 1), //4
                (1, 1), (0, 1), (-1, 0), (0, 0)
            ),

            new Wall("seagreen", new Vector(3, 1), // 5
                (-1, -1),
                (1, -1),
                (3, 0),
                (2, 2),
                (-1, 1)
            ),

            new Wall("seagreen", new Vector(2, 4), //6
                (0, 0), (0, 1), (-1, 0)
            ),

            new Wall("seagreen", new Vector(-3, 1), //7
                (1, -1), (1, 1), (2, 2), (0, 2), (-1, 1), (-1, -1)
            ),

            new Wall("seagreen", new Vector(-6, -2), //8
                (1, 1), (0, -2), (-2, -1), (-1, 0)
            ),
            new Wall("seagreen", new Vector(-6, 1), //9
                (0, 0), (0, 2), (-1, 0)
            ),
            new Wall("seagreen", new Vector(-6, 4), //10
                (0, 0), (1, 1), (-1, 1)
            ),
        };

        public override float ScallingFactor => 80f;
    }
}