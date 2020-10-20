using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;

using static PolyTanks.Helpers.MathF;

namespace PolyTanks.Helpers
{
    [Serializable]
    public struct Vector
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public static Vector FromAngle(float angle) => new Vector(Cos(angle), Sin(angle));
        
        public static Vector Zero => new Vector();
        public static Vector Identity => new Vector(1, 1);
        public static Vector operator +(Vector a, Vector b) => new Vector(a.X + b.X, a.Y + b.Y);
        public static Vector operator -(Vector a, Vector b) => new Vector(a.X - b.X, a.Y - b.Y);
        public static Vector operator *(Vector a, float b) => new Vector(a.X * b, a.Y * b);
        public static Vector operator *(Vector a, double b) => new Vector(a.X * (float)b, a.Y * (float)b);

        public float DistaceTo(Vector b) => (b - this).Length;

        public static implicit operator PointF(Vector a) => new PointF(a.X, a.Y);
        public float Length => Sqrt(Square(X) + Square(Y));
        public float Angle => Atan2(X, Y);

        public override string ToString() => $"({X}, {Y})";
    }
}
