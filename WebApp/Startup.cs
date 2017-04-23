using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Kachuwa.Core.DI;
using Kachuwa.Data;
using Kachuwa.Data.Crud;
using Kachuwa.Identity.Extensions;
using Kachuwa.Identity.Models;
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


            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.TryAddSingleton<IViewComponentSelector, Default2ViewComponentSelector>();
            services.TryAddTransient<IViewComponentHelper, Default2ViewComponentHelper>();
            string conn = Configuration.GetConnectionString("DefaultConnection");
            IDatabaseFactory dbFactory = DatabaseFactories.GetFactory(Dialect.SQLServer, conn);
            // services.AddScoped(typeof(IDatabaseFactory)).
            var asdf = new Bootstrapper(services);


            services.AddSingleton(Configuration);
            services.ConfigureIdentityCryptography(Configuration.GetSection("DapperIdentityCryptography"));
            //      .ConfigureDapperIdentityCryptography(Configuration.GetSection("DapperIdentityCryptography"));

            services.AddIdentity<IdentityUser, IdentityRole>(x =>
                {
                    x.Password.RequireDigit = false;
                    x.Password.RequiredLength = 1;
                    x.Password.RequireLowercase = false;
                    x.Password.RequireNonAlphanumeric = false;
                    x.Password.RequireUppercase = false;
                }).UseDapperWithSqlServer()
                .AddClaimsPrincipalFactory
                <Kachuwa.Identity.ClaimFactory.KachuwaClaimsPrincipalFactory<IdentityUser, IdentityRole>>()
                .AddDefaultTokenProviders();
            // Add framework services.
            services.AddMvc();
            services.AddScoped<IView, Razor2View>();
            //var moduleAssembly = typeof(Kachuwa.PluginOne.PluginOneViewComponent).GetTypeInfo().Assembly;

            //var embeddedFileProvider = new EmbeddedFileProvider(
            //    moduleAssembly
            //);
            var plugs = new PluginBootStrapper(hostingEnvironment, new FileBaseLogger(), services);
          
            //services.Configure<RazorViewEngineOptions>(options =>
            //{
            //    //options.FileProviders.Add(embeddedFileProvider);
            //    options.FileProviders.Add(pluginFileProvider);
            //});
            services.Configure<RazorViewEngineOptions>(opts =>
            {
                opts.FileProviders.Add(
                    new DatabaseFileProvider(Configuration.GetConnectionString("DefaultConnection")));
            });
            services.RegisterThemeService(config =>
            {
                config.Directory = "~/Themes";
                config.FrontendThemeName = "Default";
                config.BackendThemeName = "Default";
                config.ThemeResolver = new DefaultThemeResolver();
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            // loggerFactory.AddDebug();

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
            app.UseWebSockets();
            app.UseMiddleware<ChatWebSocketMiddleware>();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }

    public class PluginViewProvider : EmbeddedFileProvider
    {
        public PluginViewProvider(Assembly assembly) : base(assembly)
        {
        }

        public PluginViewProvider(Assembly assembly, string baseNamespace) : base(assembly, baseNamespace)
        {
        }

    }
}
