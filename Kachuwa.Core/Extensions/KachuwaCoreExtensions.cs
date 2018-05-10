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
using System.Threading;
using Kachuwa.Caching;
using Kachuwa.Configuration;
using Kachuwa.Localization;
using Kachuwa.Log;
using Kachuwa.Log.Insight;
using Kachuwa.Plugin;
using Kachuwa.Security;
using Kachuwa.Storage;
using Kachuwa.Web;
using Kachuwa.Web.Theme;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using ILogger = Kachuwa.Log.ILogger;

namespace Kachuwa.Core.Extensions
{
    public static class KachuwaCoreExtensions
    {
        public static IServiceCollection RegisterKachuwaCoreServices(this IServiceCollection services,
            IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetService<IConfiguration>();
            var changeEvent = serviceProvider.GetService<ConfigChangeEvent>();
            var autoReset = new AutoResetEvent(false);
            changeEvent.Attach(new KachuwaConfigChangeListner());
            ChangeToken.OnChange(() =>
                    configuration.GetReloadToken(),
                () =>
                {
                    autoReset.Set();
                    changeEvent.Notify();
                }
            );
            //config to json service
            services.TryAddSingleton<IConfigToJson, ConfigToJson>();
            // Add functionality to inject IOptions<T>
            services.AddOptions();// Add our Config object so it can be injected
            services.Configure<KachuwaAppConfig>(configuration.GetSection("KachuwaAppConfig"));
            services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<KachuwaAppConfig>>().Value);
            services.AddScoped(cfg => cfg.GetService<IOptionsSnapshot<KachuwaConnectionStrings>>().Value);
            //to access kachuwa app config
            //IOptions<KachuwaAppConfig> settings or Configuration.GetValue<string>("KachuwaAppConfig:AppName");  

            services.AddHttpContextAccessor();
            //registering service for later use
            services.AddSingleton(services);
            services.TryAddSingleton<ILoggerSetting, DefaultLoggerSetting>();
            services.TryAddSingleton<ILogProvider, DefaultLogProvider>();
            services.TryAddSingleton<ILogger, FileBaseLogger>();

            //removed 
            //services.TryAddSingleton<ILoggerService, FileBaseLogger>();
            services.AddTransient<LogErrorAttribute>();
            var logger = serviceProvider.GetService<ILogger>();
            IHostingEnvironment hostingEnvironment = serviceProvider.GetService<IHostingEnvironment>();
            var plugs = new PluginBootStrapper(hostingEnvironment, logger, services);

            services.AddScoped<IViewRenderService, ViewRenderService>();
            //services.TryAddSingleton<IViewComponentSelector, Default2ViewComponentSelector>();
            // services.TryAddTransient<IViewComponentHelper, Default2ViewComponentHelper>();
           
            
             services.TryAddSingleton<ICacheService, DefaultCacheService>();
            services.AddTransient<KachuwaCacheAttribute>();
            services.TryAddSingleton<IStorageProvider, LocalStorageProvider>();
            services.RegisterKachuwaStorageService(new DefaultFileOptions()
            {

            });
            //TODO:: allow in start up
     
            var asdf = new Bootstrapper(services, serviceProvider);
            // services.AddSingleton(configuration);

            // services.TryAddSingleton<ICache, DefaultCache>();
           
            services.EnableKachuwaLocalization(config =>
            {
                config.UseDbResources = true;
                config.UseJsonResources = true;
            });
            services.RegisterKachuwaWeb();
          
            //Add Cors support to the service
            //services.AddCors();
            var policy = new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicy();

            policy.Headers.Add("*");
            policy.Methods.Add("*");
            policy.Origins.Add("*");
            policy.SupportsCredentials = true;

            services.AddCors(x => x.AddPolicy("corsGlobalPolicy", policy));
            services.TryAddSingleton<ICspNonceService, CspNonceService>();

            //services.Configure<ApplicationInsightsSettings>(options => configuration.GetSection("ApplicationInsights").Bind(options));

            //enable socket
            services.AddWebSocketManager();
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Insert(0,new ViewOverrideLocationExpander());

            });
            return services;
            // Add application services.
            //services.AddTransient<IEmailSender, EmailSender>();
            //services.AddTransient<ISmsSender, SmsSender>();
          
        }


        public static void UseKachuwaInsight(this ILoggerFactory loggerFactory, IOptions<ApplicationInsightsSettings> applicationInsightsSettings, IServiceProvider serviceProvider)
        {

            loggerFactory.AddApplicationInsights(applicationInsightsSettings.Value, serviceProvider);


        }
        public static IApplicationBuilder UseKachuwaApps(this IApplicationBuilder app,
            IServiceProvider serviceProvider,IHostingEnvironment hostingEnvironment)
        {

            new KachuwaAppBuilder(app, serviceProvider, hostingEnvironment);
            return app;

        }

        public static IApplicationBuilder UseKachuwaCore(this IApplicationBuilder app, IServiceProvider serviceProvider)
        {

            //app.UseTenant();
            //TODO cache middle ware causing problem
            //app.UseMiddleware<CacheMiddleware>();
            //app.UseSecurityHeadersMiddleware(config =>
            //{
            //    config.AddContentTypeOptionsNoSniff();
            //   // config.AddContentSecurity("YXjwbSX9P94nhr8M8UdVfon4v1KGwTJDk/dRqx72CwM=");
            //    config.AddFrameOptionsSameOrigin("/");
            //    config.AddXssProtectionBlock();
            //    // config.AddStrictTransportSecurityMaxAge();
            //    config.AddContentSecurity(builder =>
            //    {
            //        builder.AddScriptPolicy(script =>
            //        {
            //            script.AllowSelf().AllowInline().AddNonce().AllowfromCdn(new[] {""});
                        

            //        });
            //        builder.AddStylePolicy(style =>
            //        {
            //            style.AllowSelf().AllowInline().AddNonce().AllowfromCdn(new[] { "https://fonts.googleapis.com" });
            //        });
            //    });
            //    config.RemoveServerHeader();

            //});
            app.UseKSockets(serviceProvider);
            app.UseKachuwaLocalization();
            // yes, demo code
            // app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseCors("corsGlobalPolicy");

            return app;

        }

        public static IServiceCollection ConfigureKachuwa(this IServiceCollection service, Action<KachuwaSetUp> configuration)
        {
            var setup = new KachuwaSetUp();
            configuration(setup);
         
            return service;
        }
    }
    public class KachuwaSetUp
    {
       
      
    }

    public static class KachuwaSetUpServiceExtensions
    {
        public static KachuwaSetUp UseDefaultMemory(this KachuwaSetUp setup,IServiceCollection services)
        {
            services.TryAddSingleton<ICacheService, DefaultCacheService>();
            return setup;
        }
        public static KachuwaSetUp UseRedisCache(this KachuwaSetUp setup, IServiceCollection services)
        {
            services.TryAddSingleton<ICacheService, RedisCacheService>();
            return setup;
        }
        public static KachuwaSetUp UseKachuwaLocalization(this KachuwaSetUp setup,
            IServiceCollection services, Action<LocaleSetting> config)
        {
            services.EnableKachuwaLocalization(config);
            return setup;
        }
       
    }
}
