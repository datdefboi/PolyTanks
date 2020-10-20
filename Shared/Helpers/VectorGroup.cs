using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static PolyTanks.Helpers.MathF;

namespace PolyTanks.Helpers
{
    public class VectorGroup : IEnumerable<Vector>
    {
        private Vector[] points;

        public VectorGroup(params Vector[] points)
        {
            this.points = points;
        }

        public static VectorGroup FromRect(Size bounds)
        {
            var h = bounds.Height / 2f;
            var w = bounds.Width / 2f;

            var bl = new Vector(-w, -h);
            var br = new Vector(w, -h);
            var tl = new Vector(-w, h);
            var tr = new Vector(w, h);

            return new VectorGroup(bl, br, tr, tl);
        }

        public IEnumerator GetEnumerator() => points.GetEnumerator();
        IEnumerator<Vector> IEnumerable<Vector>.GetEnumerator() => ((IEnumerable<Vector>) points).GetEnumerator();

        public static implicit operator PointF[](VectorGroup group) => group.points.Select(p => (PointF) p).ToArray();

        public VectorGroup Rotate(Vector relativePoint, float angle) => new VectorGroup(points.Select(p =>
        {
            var dir = p - relativePoint;
            return Vector.FromAngle(dir.Angle + angle) * dir.Length + relativePoint;
        }).ToArray());

        public VectorGroup Move(Vector direction) => new VectorGroup(points.Select(p => p + direction).ToArray());

        public VectorGroup Scale(float factor) => new VectorGroup(points.Select(p => p * factor).ToArray());

        public bool IsIntersectsByBounding(VectorGroup other)
        {
            float Area(Vector a, Vector b, Vector c)
            {
                return (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
            }

            void Swap(ref float a, ref float b)
            {
                var t = a;
                a = b;
                b = t;
            }

            bool SubIntersect(float a, float b, float c, float d)
            {
                if (a > b) Swap(ref a, ref b);
                if (c > d) Swap(ref c, ref d);
                return Max(a, c) <= Min(b, d);
            }

            bool Intersect(Vector a, Vector b, Vector c, Vector d)
            {
                return SubIntersect(a.X, b.X, c.X, d.X)
                       && SubIntersect(a.Y, b.Y, c.Y, d.Y)
                       && Area(a, b, c) * Area(a, b, d) <= 0
                       && Area(c, d, a) * Area(c, d, b) <= 0;
            }

            for (int i = 0; i < points.Length - 1; i++)
            for (int j = 0; j < other.points.Length - 1; j++)
            {
                if (Intersect(points[i], points[i + 1], other.points[j], other.points[j + 1]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}