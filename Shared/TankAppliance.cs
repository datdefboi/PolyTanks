using PolyTanks.Helpers;

namespace Core.Specs
{
    public abstract class TankAppliance
    {
        public abstract VectorGroup Bounds { get; set; }
        public abstract float BoundsRadius { get; set; }
        public abstract Vector Origin { get; set; }
        
        public abstract float MaxSpeed { get; set; }
        public abstract float Acceleration { get; set; }
        public abstract void Render(IFrame frame, TankState state);
    }
}