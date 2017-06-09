using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Kachuwa.Web.Theme;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace Kachuwa.Tenant
{
    public class TenantService : ITenantService
    {
        
        private IEnumerable<Tenant> _tenants = new List<Tenant>(new[]
         {
            new Tenant("") {
                Name = "entrancestudies.com",
                Hostnames = new[] { "entrancestudies.com" }
                ,ThemeConfig = new ThemeConfiguration()
                {
                    BackendThemeName = "Admin",
                    FrontendThemeName = "Default",
                    Directory = "Themes",
                    ThemeResolver = new DefaultThemeResolver()
                },
                  CategoryId = 2
            },
            new Tenant("") {
                Name = "science.entrancestudies.com",
                Hostnames = new[] { "science.entrancestudies.com", "http://science.entrancestudies.com" }
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
                Hostnames = new[] { "management.entrancestudies.com", "http://management.entrancestudies.com", "http://www.management.entrancestudies.com" ,"www.management.entrancestudies.com"}
                   ,ThemeConfig = new ThemeConfiguration()
                {
                    BackendThemeName = "Admin",
                    FrontendThemeName = "M2",
                    Directory = "Themes",
                    ThemeResolver = new DefaultThemeResolver()
                },
                CategoryId = 3
            }
        });

        private readonly IHostingEnvironment _hostingEnvironment;

        public TenantService(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IEnumerable<Tenant>> GetTenantsAsync()
        {
            // return _tenants;
            var filepath = Path.Combine(_hostingEnvironment.ContentRootPath, "tenantconfig\\tenants.json");

            var tetantsdata = File.ReadAllText(filepath);
            var tenants = JsonConvert.DeserializeObject<IEnumerable<Tenant>>(tetantsdata, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });
            return tenants;
        }
    }
}