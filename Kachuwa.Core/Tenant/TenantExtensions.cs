using Kachuwa.Tenant;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Tenant
{
    public static class TenantExtensions
    {
        public static void UseTenant(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<TenantResolverMiddleware>();
          
        }
        public static void RegisterTenantService(this IServiceCollection service)
        {
            //todo service registration
        }
    }
}