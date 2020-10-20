using System.Drawing;
using System.Linq;
using Core;
using PolyTanks.Helpers;

namespace PolyTanks.Engine
{
    internal class Frame : IFrame
    {
        private Graphics _currentGraphics;

        private float _height;
        private readonly float _scalling;
        private float _width;

        private Vector camOffset = Vector.Zero;
        private IFrame _frameImplementation;

        public Frame(Graphics g, int width, int height, float scalling)
        {
            _currentGraphics = g;
            _width = width;
            _height = height;
            _scalling = scalling;

            /*g.DrawLine(Pens.Black, _width / 2, 0, _width / 2, _height);
            g.DrawLine(Pens.Black, 0, _height / 2, _width, _height / 2);*/
        }

        private PointF[] NormalizeCoords(VectorGroup group) =>
            group
                .Scale(_scalling)
                .Select(p =>
                    new PointF(p.X + _width / 2 - camOffset.X * _scalling,
                        _height - p.Y - _height / 2 + camOffset.Y * _scalling))
                .ToArray();

        public void DrawPolygon(VectorGroup points, Color color, bool isCurved =
            false, float stroke = 1)
        {
            var normalizedPoints = NormalizeCoords(points);

            if (isCurved)
                _currentGraphics.DrawClosedCurve(new Pen(color, stroke), normalizedPoints);
            else
                _currentGraphics.DrawPolygon(new Pen(color, stroke), normalizedPoints);
        }

        public void FillPolygon(VectorGroup points, Color color,
            bool isCurved = false)
        {
            var normalizedPoints = NormalizeCoords(points);

            if (isCurved)
                _currentGraphics.FillClosedCurve(new SolidBrush(color), normalizedPoints);
            else
                _currentGraphics.FillPolygon(new SolidBrush(color), normalizedPoints);
        }

        public void FillCircle(Vector position, float radius, Color color)
        {
            _currentGraphics.FillEllipse(new SolidBrush(color),
                (position.X - radius - camOffset.X) * _scalling + _width / 2,
                _height - ((position.Y + radius - camOffset.Y) * _scalling + _height / 2), radius * 2,
                radius * 2);
        }

        public void LookAt(Vector point)
        {
            camOffset = point;
        }
    }
}