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
                Name = "entrancestudies.com",
                Hostnames = new[] { "entrancestudies.com" }
                ,ThemeConfig = new ThemeConfiguration()
                {
                    BackendThemeName = "Admin",
                    FrontendThemeName = "M2",
                    Directory = "Themes",
                    ThemeResolver = new DefaultThemeResolver()
                }
            },
            new Tenant("") {
                Name = "science.entrancestudies.com",
                Hostnames = new[] { "science.entrancestudies.com" }
                   ,ThemeConfig = new ThemeConfiguration()
                {
                    BackendThemeName = "Admin",
                    FrontendThemeName = "M2",
                    Directory = "Themes",
                    ThemeResolver = new DefaultThemeResolver()
                },
                CategoryId = 2
            }
            ,
            new Tenant("") {
                Name = "management.entrancestudies.com",
                Hostnames = new[] { "science.entrancestudies.com" }
                   ,ThemeConfig = new ThemeConfiguration()
                {
                    BackendThemeName = "Admin",
                    FrontendThemeName = "M2",
                    Directory = "Themes",
                    ThemeResolver = new DefaultThemeResolver()
                },
                CategoryId = 2
            }
        });

        public async Task<IEnumerable<Tenant>> GetTenantsAsync()
        {
            return _tenants;
        }
    }
}