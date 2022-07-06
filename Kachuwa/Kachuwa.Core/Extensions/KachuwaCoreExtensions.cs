using Kachuwa.Core.DI;
using Kachuwa.Web.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Threading;
using Kachuwa.Caching;
using Kachuwa.Configuration;
using Kachuwa.Localization;
using Kachuwa.Log;
using Kachuwa.Log.Insight;
using Kachuwa.Messaging;
using Kachuwa.Plugin;
using Kachuwa.Security;
using Kachuwa.Storage;
using Kachuwa.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Hosting;
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
            var applicationLifetime = serviceProvider.GetService<IHostApplicationLifetime>();
            var autoReset = new AutoResetEvent(false);
            changeEvent.Attach(new KachuwaConfigChangeListner(applicationLifetime));
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

            //TODO:: conflict on services

            // services.AddHttpContextAccessor();

            //registering service for later use
            services.AddSingleton(services);
            services.TryAddSingleton<ILoggerSetting, DefaultLoggerSetting>();
            services.TryAddSingleton<ILogProvider, DefaultLogProvider>();
            services.TryAddSingleton<ILogger, FileBaseLogger>();
            services.TryAddSingleton<IKachuwaPubSub>(new KachuwaPubSub());
            //removed 
            //services.TryAddSingleton<ILoggerService, FileBaseLogger>();
            services.AddTransient<LogErrorAttribute>();
            var logger = serviceProvider.GetService<ILogger>();
            IWebHostEnvironment hostingEnvironment = serviceProvider.GetService<IWebHostEnvironment>();
            var plugs = new PluginBootStrapper(hostingEnvironment, logger, services);

            services.TryAddSingleton<IPluginService, PluginService>();
            services.AddScoped<IViewRenderService, ViewRenderService>();
            //services.TryAddSingleton<IViewComponentSelector, Default2ViewComponentSelector>();
            // services.TryAddTransient<IViewComponentHelper, Default2ViewComponentHelper>();


            services.UseDefaultMemoryCache();
            services.UseWindowsDefenderScanner();
            services.AddTransient<KachuwaCacheAttribute>();
            services.RegisterKachuwaStorageService(new DefaultFileOptions()
            {

            });
            //TODO:: allow in start up

             new Bootstrapper(services, serviceProvider);
            // services.AddSingleton(configuration);

            // services.TryAddSingleton<ICache, DefaultCache>();
            services.AddLocalization();
            services.EnableKachuwaLocalization(config =>
            {
                config.UseDbResources = true;
                config.UseJsonResources = true;
            });
            //Add Cors support to the service
            //services.AddCors();
            var policy = new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicy();

            policy.Headers.Add("*");
            policy.Methods.Add("*");
            policy.Origins.Add("*");
            policy.PreflightMaxAge = TimeSpan.MaxValue;
            // policy.SupportsCredentials = true;

            services.AddCors(x => x.AddPolicy("corsGlobalPolicy", policy));
            services.TryAddSingleton<ICspNonceService, CspNonceService>();

            //services.Configure<ApplicationInsightsSettings>(options => configuration.GetSection("ApplicationInsights").Bind(options));
            services.AddHttpContextAccessor();
            services.RegisterKachuwaWeb();
            services.AddWebSocketManager();
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Insert(0, new ViewOverrideLocationExpander());

            });
            return services;
            // Add application services.
            //services.AddTransient<IEmailSender, EmailSender>();
            //services.AddTransient<ISmsSender, SmsSender>();

        }


        public static IApplicationBuilder UseKachuwaCore(this IApplicationBuilder app, IWebHostEnvironment hostingEnvironment, IServiceProvider serviceProvider)
        {

            new KachuwaAppBuilder(app, serviceProvider, hostingEnvironment);
            app.UseKSockets(serviceProvider);
            app.UseKachuwaLocalization();
            app.UseCors("corsGlobalPolicy");
            app.UseSession();
            app.UseResponseCompression();
            return app;

        }
        public static void UseKachuwaInsight(this ILoggerFactory loggerFactory, IOptions<ApplicationInsightsSettings> applicationInsightsSettings, IServiceProvider serviceProvider)
        {

            loggerFactory.AddApplicationInsights(applicationInsightsSettings.Value, serviceProvider);


        }

    }
}
