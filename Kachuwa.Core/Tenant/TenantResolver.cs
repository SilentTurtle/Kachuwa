using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Tenant
{
    public class TenantResolver : ITenantResolver
    {
        IEnumerable<Tenant> _tenants = new List<Tenant>(new[]
        {
            new Tenant("") {
                Name = "Tenant 1",
                Hostnames = new[] { "localhost:6000", "localhost:6001" }
            },
            new Tenant("") {
                Name = "Tenant 2",
                Hostnames = new[] { "localhost:6002" }
            }
        });

        public async Task<Tenant> ResolveAsync(HttpContext context)
        {
            var tenant = _tenants.FirstOrDefault(t =>
                t.Hostnames.Any(h => h.Equals(context.Request.Host.Value.ToLower())));
            if (tenant != null)
            {
                return tenant;
            }

            return null;
        }
    }
}