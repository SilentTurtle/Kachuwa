using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Tenant
{
    public interface ITenantResolver
    {
        Task<Tenant> ResolveAsync(HttpContext context);
    }
}