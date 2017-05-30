using Kachuwa.Tenant;
using Kachuwa.Web.Theme;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kachuwa.Tenant
{
    public static class TenantBuilderExtensions
    {
        public static void UseTenant(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<TenantResolverMiddleware>();
          
        }
        public static void RegisterTenantService(this IServiceCollection services)
        {
            //todo service registration

            services.TryAddSingleton<ITenantResolver,TenantResolver>();
            services.TryAddSingleton<ITenantService, TenantService>();
            services.RegisterThemeService();
        }
    }
}