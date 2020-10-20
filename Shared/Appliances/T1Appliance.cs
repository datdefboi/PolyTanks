using System.Drawing;
using PolyTanks.Helpers;
using static PolyTanks.Helpers.MathF;

namespace Core.Specs
{
    public class T1Appliance : TankAppliance
    {
        public override VectorGroup Bounds { get; set; } =
            VectorGroup.FromRect(new Size(70, 30));

        public override float BoundsRadius { get; set; } =
            Sqrt(Square(70) + Square(30));

        public override Vector Origin { get; set; } = new Vector(0, 0);
        public override float MaxSpeed { get; set; } = 80f;
        public override float Acceleration { get; set; } = 40f;

        private static VectorGroup headVGroup =
            new VectorGroup(
                new Vector(20, 20),
                new Vector(20, 40),
                new Vector(30, 50),
                new Vector(50, 50),
                new Vector(70, 35),
                new Vector(110, 35),
                new Vector(110, 25),
                new Vector(70, 25),
                new Vector(50, 10),
                new Vector(30, 10)
            ).Move(new Vector(-40, -30));

        private static VectorGroup bodyVGroup =
            VectorGroup.FromRect(new Size(80, 60));

        public override void Render(IFrame frame, TankState state)
        {
            var body = bodyVGroup
                .Move(Origin)
                .Rotate(Vector.Zero, state.Rotation)
                .Move(state.Position);

            var head = headVGroup
                .Move(Origin)
                .Rotate(Vector.Zero, state.Rotation + state.GunRotation - 90)
                .Move(state.Position);

            frame.FillPolygon(body, Color.DarkOliveGreen, false);
            frame.FillPolygon(head, Color.YellowGreen, false);
            frame.FillCircle(state.Position, 3, Color.Brown);

            /*foreach (var (points, turnRatio) in wheesMeta)
            foreach (var p in points)
            {
                var wheel = wheelVGroup.Rotate(Vector.Zero, SteerAngle * turnRatio).Move(Origin).Move(p)
                    .Rotate(Vector.Zero, Rotation).Move(Position);

                if (Abs(overload) > 0.95f)
                {
                    View.MarkSlip(wheel);
                }

                View.FillPolygon(wheel, Color.Black, true);
            }

            VectorGroup Prepare(VectorGroup vg) =>
                vg.Move(Origin).Rotate(Vector.Zero, Rotation).Move(Position);

            var headPoints = Prepare(headVGroup);

            var bodyPoints = Prepare(bodyVGroup);

            View.FillPolygon(bodyPoints, Color.White, false);
            View.DrawPolygon(bodyPoints, Color.Black, false);

            var fuelRatio = Fuel / MaxFuel;

            View.FillPolygon(headPoints, Color.FromArgb(
                    (int) (255 - ((255 - headColor.R) * fuelRatio)),
                    (int) (255 - ((255 - headColor.G) * fuelRatio)),
                    (int) (255 - ((255 - headColor.B) * fuelRatio))),
                true);

            View.DrawPolygon(headPoints, Color.Black, true);*/
        }
    }
}