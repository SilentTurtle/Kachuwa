
using System;
using Kachuwa.Log;
using Kachuwa.Plugin;
using Kachuwa.Web.Razor;
using Kachuwa.Web.Theme;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using WebApp.TagHelpers;
using WebApp.Test;
using Kachuwa.Web.IdentityConfig;
using Kachuwa.Web.Services;
using Kachuwa.Core.Extensions;
using ApplicationInsightsLogging;
using Kachuwa.Web;
using Microsoft.Extensions.Options;

namespace WebApp
{
    public class Startup
    {
        private IHostingEnvironment hostingEnvironment;
        public Startup(IHostingEnvironment env)
        {
            hostingEnvironment = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMemoryCache();
            services.RegisterKachuwaCoreServices(hostingEnvironment,Configuration);
          
            // Add framework services.
            services.AddMvc();


            services.AddScoped<IView, Razor2View>();
            //var moduleAssembly = typeof(Kachuwa.PluginOne.PluginOneViewComponent).GetTypeInfo().Assembly;

            //var embeddedFileProvider = new EmbeddedFileProvider(
            //    moduleAssembly
            //);
           
            //services.Configure<RazorViewEngineOptions>(options =>
            //{
            //    //options.FileProviders.Add(embeddedFileProvider);
            //    options.FileProviders.Add(pluginFileProvider);
            //});
            //services.Configure<RazorViewEngineOptions>(opts =>
            //{
            //    opts.FileProviders.Add(
            //        new DatabaseFileProvider(Configuration.GetConnectionString("DefaultConnection")));
            //});
            //services.RegisterThemeService(config =>
            //{
            //    config.Directory = "~/Themes";
            //    config.FrontendThemeName = "Default";
            //    config.BackendThemeName = "Default";
            //    config.ThemeResolver = new DefaultThemeResolver();
            //});

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider, IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<ApplicationInsightsSettings> applicationInsightsSettings)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            // loggerFactory.AddDebug();
           // loggerFactory.MinimumLevel = LogLevel.Debug;
           // loggerFactory.AddDebug(LogLevel.Debug);
            var logger = loggerFactory.CreateLogger("Startup");
            logger.LogWarning("Logger configured!");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
           
            app.UseStaticFiles();
            app.UseIdentity();
            app.UseIdentityServer();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
            //var externalCookieScheme = app.ApplicationServices.GetRequiredService<IOptions<IdentityOptions>>().Value.Cookies.ExternalCookieAuthenticationScheme;
            //app.UseGoogleAuthentication(new GoogleOptions
            //{
            //    AuthenticationScheme = "Google",
            //    SignInScheme = externalCookieScheme,
            //    ClientId = "998042782978-s07498t8i8jas7npj4crve1skpromf37.apps.googleusercontent.com",
            //    ClientSecret = "HsnwJri_53zn7VcO1Fm7THBb",
            //});
            app.UseWebSockets();
            // app.UseMiddleware<ChatWebSocketMiddleware>();

            //app.MapWebSocketManager("/chat", serviceProvider.GetService<ChatHandler>());
            app.UseKSockets(serviceProvider);

            app.UseKachuwaApps(loggerFactory,applicationInsightsSettings, serviceProvider);
        }
    }
}
