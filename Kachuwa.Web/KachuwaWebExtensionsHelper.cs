using Kachuwa.Log;
using Kachuwa.Web.Middleware;
using Kachuwa.Web.Module;
using Kachuwa.Web.Rule;
using Kachuwa.Web.Security;
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

            services.AddSingleton<IRuleService, RuleService>();
            services.AddSingleton<IScriptRunner, SQLScriptRunner>();
            services.AddSingleton<IModuleService, ModuleService>();
            services.AddSingleton<IModuleManager, ModuleManager>();
            services.AddSingleton<IEmailSender, EmailSender>();
            services.AddSingleton<ISmsSender, SmsSender>();
            services.AddSingleton<ITemplateEngine, MustacheTemplateEngine>();
            services.AddSingleton<ITokenGenerator, TokenGenerator>();
            //****for testing******//
            //services.TryAddSingleton<IRazorViewEngine, RazorViewEngine2>();
            //services.TryAddSingleton<IView,Razor2View>();

            //var ctxaccessor = services.BuildServiceProvider().GetService<IHttpContextAccessor>();
            //var ctx = new ContextResolver(ctxaccessor);
            //services.AddSingleton(ctx);
            var logger = services.BuildServiceProvider().GetService<ILogger>();
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
                  defaults: new { controller = "KachuwPage", action = "Index" });

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