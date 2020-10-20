namespace Core.Specs
{
    public static class AppliancesRepository
    {
        public static TankAppliance ForID(int id) => id switch
        {
            0 => new T1Appliance()
        };
    }
}