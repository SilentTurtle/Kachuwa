using Kachuwa.Core.Extensions;
using Kachuwa.Data;
using Kachuwa.Data.Crud;
using Kachuwa.Identity.Extensions;
using Kachuwa.Identity.IdSrv;
using Kachuwa.Identity.Service;
using Kachuwa.Job;
using Kachuwa.Log.Insight;
using Kachuwa.Web;
using Kachuwa.Web.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Smidge;
using Smidge.Nuglify;
using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace KachuwaApp
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Configuration["KachuwaAppConfig:DataProtectionKeyPath"]))
            //    .SetApplicationName(Configuration["KachuwaAppConfig:AppName"]);

            services.AddSingleton(Configuration);
            var serviceProvider = services.BuildServiceProvider();
            //TODO can be used in Action Config in KachuwaSetup
            //registering default database factory service
            IDatabaseFactory dbFactory = DatabaseFactories.SetFactory(Dialect.SQLServer, serviceProvider);
            services.AddSingleton(dbFactory);
            //registering theme
            //services.RegisterThemeService(config =>
            //{
            //    config.FrontendThemeName = "Default";
            //    config.BackendThemeName = "Admin3";
            //    config.LayoutName = "_layout";
            //});
            services.ConfigureKachuwa(setup =>
            {
                //setup.UseDefaultMemory(services)
                //    .UseKachuwaLocalization(services, config =>
                //    {
                //        config.UseDbResources = true;
                //        config.UseJsonResources = true;
                //    });

            });


            services.AddAuthorization(options =>
            {
                //options.AddPolicy("Test", policy => policy.Requirements.Add(new HasScopeRequirement("read:messages", "xyz.com")));
                options.AddPolicy(PolicyConstants.PagePermission,
                    policy => policy.Requirements.Add(new PagePermissionRequirement()));

            });

            services.RegisterKachuwaCoreServices(serviceProvider);
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAppUserService, AppUserService>();
            services.TryAddTransient<ILoginHistoryService, LoginHistoryService>();

            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, PagePermissionHandler>();
            // Register the Swagger generator, defining one or more Swagger documents

            services.AddAntiforgery(options =>
            {

                //options.FormFieldName = "AF_Tix";
                //options.HeaderName = "X-CSRF-TOKEN-Tixalaya";
                //options.SuppressXFrameOptionsHeader = false;
                //options.Cookie.Domain = "tixalaya.com";
                //options.Cookie.Name = "X-CSRF-TOKEN-Tixalaya";
                //options.Cookie.Path = "Path";
                //options.Cookie.Expiration = TimeSpan.FromMinutes(10);
                //options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            });

            services.Configure<GzipCompressionProviderOptions>
                (options => options.Level = CompressionLevel.Fastest);
            services.AddResponseCompression(options => { options.Providers.Add<GzipCompressionProvider>(); });
            // Add framework services.
            IMvcBuilder mvcBuilder = services.AddMvc(options =>
            {
                options.Filters.Add(new AuditAttribute());
            })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;//camel casing
                    options.JsonSerializerOptions.WriteIndented = true;

                    // options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    // options.SerializerSettings.Formatting = Formatting.Indented;
                })
                .AddRazorRuntimeCompilation().AddViewComponentsAsServices(); //.SetCompatibilityVersion(CompatibilityVersion.Version_2_2); 
                                                                             //dual authorization support
            services.AddControllersWithViews();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            //Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //foreach (Assembly assembly in assemblies)
            //{
            //    try
            //    {
            //        if (assembly.GetTypes().Any(t => t.IsSubclassOf(typeof(Controller))))
            //        {
            //            mvcBuilder.AddApplicationPart(assembly);
            //        }
            //    }
            //    catch
            //    {
            //        // bugs in unit test framework throw exceptions for weird assemblies like intellitrace
            //    }
            //}
            services.AddSingleton<RouteValueTransformer>();


            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.ConsentCookie.Domain = Configuration["KachuwaAppConfig:CookieDomain"];
                // options.MinimumSameSitePolicy = SameSiteMode.None;
                options.ConsentCookie.Name = "_kachuwa_consent";

            });
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
            })
               .AddCookie("Cookies");
            //services.AddAuthentication(options =>
            // {
            //  options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //  options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //   options.DefaultChallengeScheme = "oidc";
            //  options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            // })
            //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            // {
            //     options.AccessDeniedPath = "/access-denied";
            //     options.LoginPath = "/account/login";
            //    // Configure the client application to use sliding sessions
            //    options.SlidingExpiration = true;
            //    // Expire the session of 15 minutes of inactivity
            //    options.SessionStore = new MemoryCacheTicketStore();
            //     options.Events = new CookieAuthenticationEvents()
            //     {
            //        // OnValidatePrincipal = new Func<CookieValidatePrincipalContext, Task>(SecurityStampValidator.ValidatePrincipalAsync)
            //    };
            //     options.SlidingExpiration = true;
            //    // Expire the session of 15 minutes of inactivity
            //    //options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            //    options.Cookie.SameSite = SameSiteMode.Lax;
            //     options.Cookie.HttpOnly = true;
            //     options.ExpireTimeSpan = TimeSpan.FromDays(30);
            //     options.Events = new CookieAuthenticationEvents()
            //     {
            //         OnValidatePrincipal = async context =>
            //         {
            //             const string claimType = "validatedat";
            //             const int reValidateAfterMinutes = 5;

            //             if (!(context.Principal?.Identity is ClaimsIdentity claimIdentity)) return;

            //            //if (!context.Principal.HasClaim(c => c.Type == claimType) ||
            //            //    DateTimeOffset.Now.UtcDateTime.Subtract(new DateTime(long.Parse(context.Principal.Claims.First(c => c.Type == claimType).Value))) > TimeSpan.FromMinutes(reValidateAfterMinutes))
            //            //{
            //            var mgr = context.HttpContext.RequestServices
            //             .GetRequiredService<SignInManager<Kachuwa.Identity.Models.IdentityUser>>();
            //             if (string.IsNullOrEmpty(claimIdentity.Name)) return;
            //             var user = await mgr.UserManager.FindByNameAsync(claimIdentity.Name);
            //             if (user != null &&
            //                 claimIdentity.Claims.FirstOrDefault(c => c.Type == "AspNet.Identity.SecurityStamp")
            //                     ?.Value == await mgr.UserManager.GetSecurityStampAsync(user))
            //             {
            //                 claimIdentity.FindAll(claimType).ToList()
            //                     .ForEach(c => claimIdentity.TryRemoveClaim(c));
            //                 claimIdentity.AddClaim(new Claim(claimType,
            //                     DateTimeOffset.Now.UtcDateTime.Ticks.ToString(), typeof(long).ToString()));
            //                 context.ShouldRenew = true;
            //             }
            //             else
            //             {
            //                 context.RejectPrincipal();
            //                 await mgr.SignOutAsync();
            //             }

            //            //}
            //        }
            //     };
            // });
            //.AddJwtBearer(opts =>
            //{
            //    opts.Authority = "https://localhost:44368";
            //    opts.Audience = "KachuwaAPI";
            //    opts.RequireHttpsMetadata = false;
            //    opts.IncludeErrorDetails = true;
            //    opts.Events = new JwtBearerEvents
            //    {
            //        OnMessageReceived = ctx =>
            //        {
            //                // replace "token" with whatever your param name is
            //                if (ctx.Request.Method.Equals("GET") && ctx.Request.Query.ContainsKey("token"))
            //                ctx.Token = ctx.Request.Query["token"];
            //            return Task.CompletedTask;
            //        }
            //    };
            //})
            //.AddOpenIdConnect("oidc", options =>
            //{
            //        //  options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //        //  options.SignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            //        options.SaveTokens = true;
            //    options.Authority = "https://localhost:44368";
            //    options.ClientId = "okweb";
            //    options.ClientSecret = "passwordchaina";
            //    options.RequireHttpsMetadata = true;
            //    options.ResponseType = "id_token";
            //    options.GetClaimsFromUserInfoEndpoint = true;
            //    options.SaveTokens = true;
            //    options.NonceCookie.Name = "_kachuwa_id_nonce";
            //    options.CorrelationCookie.Name = "_kachawa_corr";
            //    options.Scope.Clear();
            //    options.Scope.Add("openid");
            //    options.Scope.Add("profile");
            //    options.Scope.Add("email");

            //        //options.SaveTokens = true;
            //        //options.ClaimActions.Remove("auth_time");
            //        //options.ClaimActions.MapUniqueJsonKey("sub", "sub");
            //        //options.ClaimActions.MapUniqueJsonKey("name", "name");
            //        //options.ClaimActions.MapUniqueJsonKey("given_name", "given_name");
            //        //options.ClaimActions.MapUniqueJsonKey("family_name", "family_name");
            //        //options.ClaimActions.MapUniqueJsonKey("profile", "profile");
            //        //options.ClaimActions.MapUniqueJsonKey("email", "email");
            //        // options.ClaimActions.MapUniqueJsonKey("auth_time", "iat");
            //        // options.ClaimActions.MapUniqueJsonKey("idp", "iss");
            //        options.Events.OnTicketReceived = async (context) =>
            //    {
            //        context.Properties.ExpiresUtc = DateTime.UtcNow.AddDays(1);
            //    };
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        NameClaimType = "name",
            //        RoleClaimType = "role"
            //    };
            //})

            // services.AddOidcStateDataFormatterCache("oidc");





            services.AddSession(options =>
            {
                options.Cookie.Name = ".Kachuwa.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.IsEssential = true; // make the session cookie Essential
            });
            //services.AddDistributedSqlServerCache(options =>
            //{
            //    options.ConnectionString = Configuration["ConnectionStrings:DefaultConnection"];
            //    options.SchemaName = "dbo";
            //    options.TableName = "Sessions";
            //});

            services.AddLocalization();
            // File Sile
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = Int64.MaxValue;
                options.MultipartBoundaryLengthLimit = Int32.MaxValue;
                options.ValueLengthLimit = Int32.MaxValue;
                options.MultipartHeadersLengthLimit = Int32.MaxValue;
                options.ValueCountLimit = Int32.MaxValue;
            });

            //enable directory browsing
            //services.AddDirectoryBrowser();
            services.AddSignalR().AddJsonProtocol(options =>
            {
                // options.PayloadSerializerSettings.ContractResolver = new DefaultContractResolver();
                // options.PayloadSerializerSettings.Formatting = Formatting.Indented;
            }); ;


            //In production, the React files will be served from this directory

            //services.RegisterSchedulingServer(Configuration);

            //services.AddHsts(options =>
            //{
            //    options.Preload = true;
            //    options.IncludeSubDomains = true;
            //    options.MaxAge = TimeSpan.FromDays(1);

            //});
            //services.AddHttpsRedirection(options =>
            //{
            //    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
            //    options.HttpsPort = hostingEnvironment.IsDevelopment() ? 44330 : 443;
            //});

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider,
            IWebHostEnvironment env, ILoggerFactory loggerFactory,
            IOptions<ApplicationInsightsSettings> applicationInsightsSettings)
        {
            //app.UseHttpsRedirection();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                //
            }

            app.UseSession();
            app.UseResponseCompression();

            var provider = new FileExtensionContentTypeProvider();
            // Add new mappings
            provider.Mappings[".log"] = "text/plain";
            provider.Mappings[".txt"] = "text/plain";

            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(hostingEnvironment.ContentRootPath, @"Logs")),
                RequestPath = new PathString("/dev/logs"),
                EnableDirectoryBrowsing = true,
                StaticFileOptions =
                {
                    DefaultContentType = "text/plain",
                    ContentTypeProvider = provider

                }

            });
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(hostingEnvironment.ContentRootPath, @"wwwroot", "lib")),
                RequestPath = new PathString("/lib")
            });
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                   Path.Combine(hostingEnvironment.ContentRootPath, @"wwwroot", "assets")),
                RequestPath = new PathString("/assets")
            });                  
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(hostingEnvironment.ContentRootPath, @"Themes")),
                RequestPath = new PathString("/themes")
            });

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(hostingEnvironment.ContentRootPath, @"Locale")),
                RequestPath = new PathString("/locale")
            });


            app.UseAuthentication(); // not needed, since UseIdentityServer adds the authentication middleware
            // app.UseIdentityServer();
            app.UseStaticHttpContext();

            app.UseWebSockets();

            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("hi-IN"),
                new CultureInfo("en-GB"),
                //new CultureInfo("en"),
                new CultureInfo("es-ES"),
                new CultureInfo("zh-CN"),
                new CultureInfo("es"),
                new CultureInfo("fr-FR"),
                new CultureInfo("fr"),
                new CultureInfo("ne-NP"),
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures

            });
            app.UseKachuwaApps(serviceProvider, hostingEnvironment);
            //core
            app.UseKachuwaCore(serviceProvider);

            // config do openID
            //web
            app.UseKachuwaWeb(false);

            //app.UseSignalR(routes =>  // <-- SignalR
            //{

            //   // routes.MapHub<KachuwaUserHub>("/hubs/user");


            //    //routes.MapHub<DashboardHub>("/hubs/dashboard");

            //});

            app.UseCookiePolicy();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{pageUrl?}",
                    defaults: new { controller = "KachuwaPage", action = "Index" }
                    //constraints: new { pageUrl = @"\w+" }
                    );
                endpoints.MapControllerRoute(
    name: "default2",
    pattern: "{controller}/{action}/{id?}",
    defaults: new { controller = "Home", action = "Index" });
                endpoints.MapControllerRoute(
               name: "MyArea",
               pattern: "{area:exists}/{controller}/{action}/{id?}",
                defaults: new { area = "Admin", controller = "Dashboard", action = "Index" });


                //endpoints.MapControllerRoute(
                //    name: "default2",
                //    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
            //app.UseMvc(routes =>
            //{
            //    //routes.MapRoute(
            //    //    name: "default",
            //    //    template: "{pageUrl?}",
            //    //    defaults: new { controller = "KachuwPage", action = "Index" }
            //    //    // , constraints: new { pageUrl = @"\w+" }
            //    //);
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");


            //    routes.MapRoute(name: "areaRoute",
            //        template: "{area:exists}/{controller}/{action}/{id?}",
            //        defaults: new { area = "Admin", controller = "Dashboard", action = "Index" });


            //});
            app.UseSmidge();
            // app.UseSmidgeNuglify();
            //  app.UseSchedulingServer();

        }

    }
    public class RouteValueTransformer : DynamicRouteValueTransformer
    {
        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            var controller = (string)values["controller"];
            if (controller == null)
                values["controller"] = "KachuwaPage";

            var action = (string)values["action"];
            if (action == null)
                values["action"] = "index";

            return values;
        }
    }
}
