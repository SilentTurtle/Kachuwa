using System.Collections.Generic;
using System.Threading.Tasks;
using Kachuwa.Web.Theme;

namespace Kachuwa.Tenant
{
    public class TenantService : ITenantService
    {
        public TenantService()
        {

        }

        private IEnumerable<Tenant> _tenants = new List<Tenant>(new[]
         {
            new Tenant("") {
                Name = "Tenant 1",
                Hostnames = new[] { "localhost:8888", "localhost:8889" }
                ,ThemeConfig = new ThemeConfiguration()
                {
                    BackendThemeName = "Admin",
                    FrontendThemeName = "Default",
                    Directory = "Themes",
                    ThemeResolver = new DefaultThemeResolver()
                }
            },
            new Tenant("") {
                Name = "Tenant 2",
                Hostnames = new[] { "localhost:9000" }
                   ,ThemeConfig = new ThemeConfiguration()
                {
                    BackendThemeName = "Admin",
                    FrontendThemeName = "Theme2",
                    Directory = "Themes",
                    ThemeResolver = new DefaultThemeResolver()
                }
            }
        });

        public async Task<IEnumerable<Tenant>> GetTenantsAsync()
        {
            return _tenants;
        }
    }
}