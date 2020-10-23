using System;
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

            MaxSize = new Lazy<float>(() =>
            {
                var max = 0f;
                foreach (var p1 in points)
                {
                    foreach (var p2 in points)
                    {
                        var d = p1.DistaceTo(p2);
                        if (d > max)
                            max = d;
                    }
                }

                return max;
            });
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

        #region Misc

        public IEnumerator GetEnumerator() => points.GetEnumerator();
        IEnumerator<Vector> IEnumerable<Vector>.GetEnumerator() => ((IEnumerable<Vector>) points).GetEnumerator();

        public static implicit operator PointF[](VectorGroup group) => group.points.Select(p => (PointF) p).ToArray();

        #endregion

        public VectorGroup Rotate(Vector relativePoint, float angle) => new VectorGroup(points.Select(p =>
        {
            var dir = p - relativePoint;
            return Vector.FromAngle(dir.Angle + angle) * dir.Length + relativePoint;
        }).ToArray());

        public VectorGroup Move(Vector direction) => new VectorGroup(points.Select(p => p + direction).ToArray());

        public VectorGroup Scale(float factor) => new VectorGroup(points.Select(p => p * factor).ToArray());

        #region Geometry

        public Vector CenterOfMass => points.Aggregate(Vector.Zero, (a, acc) => acc + a) * (1f / points.Length);

        public IEnumerable<((Vector a, Vector b) own, (Vector a, Vector b) other)> FindIntersections(VectorGroup other)
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

            var result = new List<((Vector a, Vector b), (Vector c, Vector d))>();

            for (int i = 0; i < points.Length; i++)
            for (int j = 0; j < other.points.Length; j++)
            {
                var iNext = (i + 1) % points.Length;
                var jNext = (j + 1) % other.points.Length;
                if (Intersect(points[i], points[iNext], other.points[j], other.points[jNext]))
                {
                    result.Add(((points[i], points[iNext]), (other.points[j], other.points[jNext])));
                }
            }

            return result;
        }

        #endregion


        public Lazy<float> MaxSize { get; private set; }
    }
}