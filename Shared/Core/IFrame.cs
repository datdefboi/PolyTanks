using System.Drawing;
using PolyTanks.Helpers;

namespace Core
{
    public interface IFrame
    {
        void DrawPolygon(VectorGroup points, Color color, bool isCurved =
            false, float stroke = 1);

        void FillPolygon(VectorGroup points, Color color,
            bool isCurved = false);

        void FillCircle(Vector position, float radius, Color color);

        void LookAt(Vector point);
    }
}