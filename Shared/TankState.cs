using PolyTanks.Helpers;

namespace Core
{
    public class TankState
    {
        public Vector Position { get; set; }
        public float Rotation { get; set; } = 0f;
        public float GunRotation { get; set; } = 90f;
        
        public float Speed { get; set; } = 0f;

        public bool IsInters { get; set; } = false;
        public int ApplianceID { get; set; } = 0;
    }
}