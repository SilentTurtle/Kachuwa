using Kachuwa.Tenant;
using Kachuwa.Web;
using Kachuwa.Web.Theme;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
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
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new TenantViewLocationExpander());
            });
            services.RegisterThemeService();
        }
    }
}