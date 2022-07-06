
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Threading.Tasks;
using Kachuwa.Data;
using Kachuwa.Core.Extensions;
using Kachuwa.Log.Insight;
using Kachuwa.Web;
using Kachuwa.Data.Crud;
using Kachuwa.Job;
using Microsoft.Extensions.Hosting;

namespace KachuwaWebApp
{
    public class Startup
    {
        private IWebHostEnvironment hostingEnvironment;
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            hostingEnvironment = env;
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);
            var serviceProvider = services.BuildServiceProvider();
            //TODO can be used in Action Config in KachuwaSetup
            //registering default database factory service
            IDatabaseFactory dbFactory = DatabaseFactories.SetFactory(Dialect.SQLServer, serviceProvider);
            services.AddSingleton(dbFactory);
            services.RegisterKachuwaCoreServices(serviceProvider);

            services.AddSignalR(o =>
            {

                o.EnableDetailedErrors = true;
                o.ClientTimeoutInterval = TimeSpan.FromSeconds(60);

            }).AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.PropertyNamingPolicy = null;//camel casing

            });
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "egtkt";
                options.LoginPath = new PathString("/account/login");
                options.AccessDeniedPath = new PathString("/access-denied");
                options.LogoutPath = new PathString("/account/logout");
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.HttpOnly = false;
                options.ExpireTimeSpan = TimeSpan.FromDays(30);


            });
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";

            })
                .AddCookie("Cookies").AddJwtBearer(opts =>
                {
                    opts.Authority = hostingEnvironment.IsDevelopment() ? "https://localhost:44360" : Configuration["KachuwaAppConfig:TokenAuthority"];
                    opts.Audience = "KachuwaApi";
                    opts.RequireHttpsMetadata = true;
                    opts.IncludeErrorDetails = true;
                    opts.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            // replace "token" with whatever your param name is
                            if (ctx.Request.Method.Equals("GET") && ctx.Request.Query.ContainsKey("token"))
                                ctx.Token = ctx.Request.Query["token"];
                            return Task.CompletedTask;
                        }
                    };
                });
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(1);

            });
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
                options.HttpsPort = hostingEnvironment.IsDevelopment() ? 44360 : 443;
            });
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            //services.AddAuthentication().AddGoogle(googleOptions =>
            //{
            //    googleOptions.ClientId = Configuration["Authentication:Google:ClientId"];
            //    googleOptions.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            //});
            //services.AddAuthentication().AddFacebook(facebookOptions =>
            //{
            //    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
            //    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            //});
           // services.RegisterSchedulingServer(Configuration);

           services.AddSignalR();

        }
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider,
            IWebHostEnvironment env, ILoggerFactory loggerFactory,
            IOptions<ApplicationInsightsSettings> applicationInsightsSettings)
        {
            app.UseHttpsRedirection();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseCookiePolicy();
            app.UseSession();
            app.UseRouting();
            //core
            app.UseWebSockets();
            app.UseKachuwaCore(env, serviceProvider);
            app.UseKachuwaWeb(env,true);
            //app.UseEndpoints(endpoints =>
            //{

            //    //endpoints.MapControllerRoute(
            //    //    name: "default",
            //    //    pattern: "{pageUrl?}",
            //    //    defaults: new { controller = "KachuwaPage", action = "Index" }
            //    //    //, constraints: new { pageUrl = @"\w+" }
            //    //);
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller}/{action}/{id?}",
            //        defaults: new { controller = "Home", action = "Index" });
            //    endpoints.MapControllerRoute(
            //        name: "MyArea",
            //        pattern: "{area:exists}/{controller}/{action}/{id?}",
            //        defaults: new { area = "Admin", controller = "Dashboard", action = "Index" });


                
            //   // endpoints.MapHub<KachuwaUserHub>("/hubs/user");
            //    //endpoints.MapHub<CartHub>("/hubs/cart");
            //});
           
            // app.UseSchedulingServer();
            //  app.UseWebSockets();

        }
    }
}

