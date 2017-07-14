using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kachuwa.Tenant
{
    public interface ITenantService
    {
        Task<IEnumerable<Tenant>> GetTenantsAsync();
    }
}