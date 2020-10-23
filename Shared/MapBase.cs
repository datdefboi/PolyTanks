namespace PolyTanks.Shared
{
    public abstract class MapBase
    {
        public abstract Wall[] Walls { get; }

        public abstract float ScallingFactor { get; }
    }
}