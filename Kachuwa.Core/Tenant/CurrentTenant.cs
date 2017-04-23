namespace Kachuwa.Tenant
{
    public class CurrentTenant
    {
        public CurrentTenant(Tenant tenant)
        {
            Info = tenant;
        }

        public Tenant Info { get; private set; }
    }
}