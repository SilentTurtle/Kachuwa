using Kachuwa.Core.DI;
using Kachuwa.Data;
using Kachuwa.Data.Crud;
using Kachuwa.Web.Razor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Kachuwa.Caching;
using Kachuwa.Log;
using Kachuwa.Plugin;
using Kachuwa.Storage;
using Kachuwa.Web.Theme;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Kachuwa.Core.Extensions
{
    public static class KachuwaCoreExtensions
    {
        public static IServiceCollection RegisterKachuwaCoreServices(this IServiceCollection services,
            IHostingEnvironment hostingEnvironment,
            IConfigurationRoot configuration)
        {
            services.TryAddSingleton<ILogProvider, DefaultLogProvider>();
            services.TryAddSingleton<ILogger, FileBaseLogger>();
            services.TryAddSingleton<ILoggerService, FileBaseLogger>();
            services.AddTransient<LogErrorAttribute>();
            var plugs = new PluginBootStrapper(hostingEnvironment, new FileBaseLogger(), services);


            services.AddScoped<IViewRenderService, ViewRenderService>();
            //services.TryAddSingleton<IViewComponentSelector, Default2ViewComponentSelector>();
           // services.TryAddTransient<IViewComponentHelper, Default2ViewComponentHelper>();
            string conn = configuration.GetConnectionString("DefaultConnection");
            IDatabaseFactory dbFactory = DatabaseFactories.GetFactory(Dialect.SQLServer, conn);
            services.AddSingleton(dbFactory);
            var asdf = new Bootstrapper(services, configuration);
            services.AddSingleton(configuration);
           
           // services.TryAddSingleton<ICache, DefaultCache>();
            services.TryAddSingleton<ICacheService, DefaultCacheService>();
            services.AddTransient<KachuwaCacheAttribute>();
            //services.TryAddSingleton<IStorageProvider, LocalStorageProvider>();
            services.RegisterKachuwaStorageService(new DefaultFileOptions());

            //service to convert view to string
            services.TryAddSingleton<IViewRenderService, ViewRenderService>();

            services.RegisterKachuwaRazorProvider(configuration);

            services.RegisterThemeService(config =>
            {
                config.Directory = "~/Themes";
                config.FrontendThemeName = "Default";
                config.BackendThemeName = "Default";
                config.ThemeResolver = new DefaultThemeResolver();
            });
            return services;
            // Add application services.
            //services.AddTransient<IEmailSender, EmailSender>();
            //services.AddTransient<ISmsSender, SmsSender>();
        }


        public static IApplicationBuilder UseKachuwaApps(this IApplicationBuilder app)
        {
            app.UseMiddleware<CacheMiddleware>();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            return app;

        }
    }
}
