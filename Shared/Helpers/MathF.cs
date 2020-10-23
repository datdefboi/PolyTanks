using System;
using System.Linq;

namespace PolyTanks.Helpers
{
    public static class MathF
    {
        public static float EPS = 1e-9f;
        public static float ToDeg(float a) => a / (float) Math.PI * 180f;
        public static float ToDeg(double a) => (float) a / (float) Math.PI * 180f;

        public static float ToRad(float a) => a * (float) Math.PI / 180f;

        public static float Atan2(float x, float y) => ToDeg(Math.Atan2(y, x));
        public static float Cos(float a) => (float) Math.Cos(ToRad(a));
        public static float Sin(float a) => (float) Math.Sin(ToRad(a));
        public static float Sqrt(float a) => (float) Math.Sqrt(a);
        public static float Pow(float a, float power) => (float) Math.Pow(a, power);
        public static float Square(float a) => Pow(a, 2);
        public static float Cube(float a) => Pow(a, 3);

        public static float Abs(float a) => Math.Abs(a);
        public static float Sign(float a) => Math.Sign(a);
        public static float Max(float a, float b) => a >= b ? a : b;
        public static float Min(float a, float b) => a <= b ? a : b;

        public static float Reach(float from, float to, float step)
        {
            var sign = Math.Sign(to - from);
            var dif = Math.Abs(to - from);
            return from + sign * Math.Min(step, dif);
        }
    }
}