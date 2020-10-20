using PolyTanks.Helpers;

namespace Core.Specs
{
    public abstract class TankAppliance
    {
        public abstract VectorGroup Bounds { get; }
        public abstract float BoundsRadius { get; }
        public abstract Vector Origin { get; }

        public abstract float MaxSpeed { get; }
        public abstract float Acceleration { get; }

        public abstract float RotationSpeed { get; }
        public abstract float TurretSpeed { get; }
        public abstract void Render(IFrame frame, TankState state);
    }
}