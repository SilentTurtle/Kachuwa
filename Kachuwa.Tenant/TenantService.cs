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
                Name = "kachuwaframework.com",
                Hostnames = new[] { "kachuwaframework.com" }
                ,ThemeConfig = new ThemeConfiguration()
                {
                    BackendThemeName = "Admin",
                    FrontendThemeName = "Default",
                    Directory = "Themes",
                    ThemeResolver = new DefaultThemeResolver()
                }
            },
            new Tenant("") {
                Name = "tenant1.kachuwaframework.com",
                Hostnames = new[] { "tenant1.kachuwaframework.com", "http://tenant1.kachuwaframework.com" }
                   ,ThemeConfig = new ThemeConfiguration()
                {
                    BackendThemeName = "Admin",
                    FrontendThemeName = "t1",
                    Directory = "Themes",
                    ThemeResolver = new DefaultThemeResolver()
                }
            }
            ,
            new Tenant("") {
                Name = "tenant2.kachuwaframework.com",
                Hostnames = new[] { "tenant2.kachuwaframework.com", "http://tenant2.kachuwaframework.com"}
                   ,ThemeConfig = new ThemeConfiguration()
                {
                    BackendThemeName = "Admin",
                    FrontendThemeName = "t2",
                    Directory = "Themes",
                    ThemeResolver = new DefaultThemeResolver()
                }
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
            if (!File.Exists(filepath))
            {
                File.Create(filepath);
                File.WriteAllText(filepath, JsonConvert.SerializeObject(_tenants,formatting:Formatting.Indented));
            }
            var tetantsdata = File.ReadAllText(filepath);
            var tenants = JsonConvert.DeserializeObject<IEnumerable<Tenant>>(tetantsdata, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            });
            return tenants;
        }
    }
}