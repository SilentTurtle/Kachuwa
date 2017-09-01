using Kachuwa.Caching;
using Kachuwa.Log;
using Kachuwa.Web.Layout;
using Kachuwa.Web.Middleware;
using Kachuwa.Web.Module;
using Kachuwa.Web.Notification;
using Kachuwa.Web.Rule;
using Kachuwa.Web.Security;
using Kachuwa.Web.Service;
using Kachuwa.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kachuwa.Web
{
    public static class KachuwaWebExtensionsHelper
    {
        public static IServiceCollection RegisterKachuwaWebServices(this IServiceCollection services)
        {
            services.TryAddSingleton<ILayoutRenderer, LayoutContentRenderer>();
            services.TryAddSingleton<ISeoService, SeoService>();
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
            services.AddSingleton<IRuleService, RuleService>();
            services.AddSingleton<IScriptRunner, SQLScriptRunner>();
            services.AddSingleton<IModuleService, ModuleService>();
            services.AddSingleton<IModuleManager, ModuleManager>();
            services.AddScoped<IModuleComponentProvider, ModuleComponentProvider>();
         
            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddSingleton<ISmsSender, SmsSender>();
            services.AddSingleton<IMenuService, MenuService>();
            services.AddSingleton<ISettingService, SettingService>();
            services.AddSingleton<ITemplateEngine, MustacheTemplateEngine>();
            services.AddSingleton<ITokenGenerator, TokenGenerator>();
            services.AddSingleton<IFileService, LocalFileService>();
            services.RegisterNotificationService();
            //****for testing******//
            //services.TryAddSingleton<IRazorViewEngine, RazorViewEngine2>();
            //services.TryAddSingleton<IView,Razor2View>();

            //var ctxaccessor = services.BuildServiceProvider().GetService<IHttpContextAccessor>();
            //var ctx = new ContextResolver(ctxaccessor);
            //services.AddSingleton(ctx);
            var modules = new ModuleRegistrar(services, logger);
            return services;
        }
        public static IApplicationBuilder UseKachuwaWeb(this IApplicationBuilder app,bool useDefaultRoute)
        {
          
            app.UseMiddleware<ModuleResourceMiddleware>();
            if (useDefaultRoute)
            {
                app.UseRoutes();
            }
            return app;
        }
        public static IApplicationBuilder UseRoutes(this IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                  name: "default",
                  template: "{pageUrl?}",
                  defaults: new { controller = "KachuwPage", action = "Index" }
                 // , constraints: new { pageUrl = @"\w+" }
                 );

                routes.MapRoute(
                    name: "default1",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(name: "areaRoute",
                    template: "{area:exists}/{controller}/{action}/{id?}",
                    defaults: new { area = "Admin", controller = "Dashboard", action = "Index" });



            });
            return app;
        }

    }
}