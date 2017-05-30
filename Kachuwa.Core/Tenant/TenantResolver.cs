using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Tenant
{
    public class TenantResolver : ITenantResolver
    {
        private readonly ITenantService _tenantService;

        public TenantResolver(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        public async Task<Tenant> ResolveAsync(HttpContext context)
        {

            //TODO 
            var tenants = await _tenantService.GetTenantsAsync();
            var tenant = tenants.FirstOrDefault(t =>
                  t.Hostnames.Any(h => h.Equals(context.Request.Host.Value.ToLower())));
            if (tenant != null)
            {
                return tenant;
            }

            return tenants.First();
        }
    }
}