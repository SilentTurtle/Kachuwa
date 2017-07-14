using System;
using Kachuwa.Caching;
using Kachuwa.Storage;
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
        public static void RegisterTenantService(this IServiceCollection services, Action<TenantConfig> config)
        {
            //todo service registration
            var tenantConfig = new TenantConfig();
            config(tenantConfig);
            //overriting default instances
            services.TryAddSingleton(tenantConfig.Cache);
            services.TryAddSingleton(tenantConfig.StorageProvider);
            services.TryAddSingleton<ITenantResolver,TenantResolver>();
            services.TryAddSingleton<ITenantService, TenantService>();
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new TenantViewLocationExpander());
                options.ViewLocationExpanders.Add(new TenantThemeLocationExpander());
            });
          
        }
    }
}