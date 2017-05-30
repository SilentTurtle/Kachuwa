using Kachuwa.Web.Theme;

namespace Kachuwa.Tenant
{
    public class Tenant
    {
        public Tenant(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public string Name { get; set; }
        public string[] Hostnames { get; set; }

        public IThemeConfig ThemeConfig { get; set; }

        public string ConnectionString { get; private set; }
        public int CategoryId { get; set; }

    }
}