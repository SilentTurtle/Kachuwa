using Kachuwa.Caching;
using Kachuwa.Log;
using Kachuwa.Web.Layout;
using Kachuwa.Web.Razor;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kachuwa.Web
{
    public static class KachuwaWebExtensions
    {
        public static IServiceCollection RegisterKachuwaWeb(this IServiceCollection services)
        {
            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            //service to convert view to string

            services.TryAddSingleton<IViewRenderService, ViewRenderService>();
            services.TryAddSingleton<ILayoutRenderer, LayoutContentRenderer>();
            services.TryAddSingleton<IPageService, PageService>();
            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger>();
            var pageService = serviceProvider.GetService<IPageService>();
            var cacheService = serviceProvider.GetService<ICacheService>();
            var ctxaccessor = serviceProvider.GetService<IHttpContextAccessor>();
        
            services.Configure<RazorViewEngineOptions>(opts =>
            {
                opts.FileProviders.Add(
                    new PageFileProvider(pageService, logger, cacheService, ctxaccessor));
            });
          
            // ContextResolver.Set(ctxaccessor);
           // services.AddSingleton(ContextResolver);

            return services;
        }
    }
}