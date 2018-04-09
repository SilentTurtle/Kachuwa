using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Kachuwa.Core.Extensions;
using Kachuwa.Data;
using Kachuwa.Data.Crud;
using Kachuwa.Log.Insight;
using Kachuwa.Web;
using Kachuwa.Web.Theme;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Kachuwa.Identity;
using Kachuwa.RTC;
using Microsoft.AspNetCore.Authorization;
using Kachuwa.Web.Security;

namespace KachuwaApp
{
       

    public class Startup
    {
        private IHostingEnvironment hostingEnvironment;
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            hostingEnvironment = env;
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddSingleton(Configuration);
            var serviceProvider = services.BuildServiceProvider();
            //TODO can be used in Action Config in KachuwaSetup
            //registering default database factory service
            IDatabaseFactory dbFactory = DatabaseFactories.SetFactory(Dialect.SQLServer, serviceProvider);
            services.AddSingleton(dbFactory);
            //registering theme
            services.RegisterThemeService(config =>
            {
                config.FrontendThemeName = "Default";
                config.BackendThemeName = "Admin";
                config.LayoutName = "_layout";
            });
            services.ConfigureKachuwa(setup =>
            {
                //setup.UseDefaultMemory(services)
                //    .UseKachuwaLocalization(services, config =>
                //    {
                //        config.UseDbResources = true;
                //        config.UseJsonResources = true;
                //    });

            });
            //var guestPolicy = new AuthorizationPolicyBuilder()
            //// .RequireAuthenticatedUser()
            // .RequireClaim("scope", "dataEventRecords")
            // .Build();

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("dataEventRecordsAdmin", policyAdmin =>
            //    {
            //        policyAdmin.RequireClaim("role", "dataEventRecords.admin");
            //    });
            //    options.AddPolicy("dataEventRecordsUser", policyUser =>
            //    {
            //        policyUser.RequireClaim("role", "dataEventRecords.user");
            //    });

            //});

            services.AddAuthorization(options =>
            {
                //options.AddPolicy("Test", policy => policy.Requirements.Add(new HasScopeRequirement("read:messages", "xyz.com")));
                options.AddPolicy(PolicyConstants.PagePermission, policy => policy.Requirements.Add(new PagePermissionRequirement()));
            });

            services.RegisterKachuwaCoreServices(serviceProvider);
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
            //-----TESTED OK-----------
            //serviceProvider = services.BuildServiceProvider();
            //var configtoJson = serviceProvider.GetService<ConfigToJson>();
            //var kconfig = serviceProvider.GetService<IOptionsSnapshot<KachuwaAppConfig>>().Value;
            //for (int i = 0; i < 10; i++)
            //{
            //    kconfig.AppName = "test"+i;
            //    configtoJson.SaveKachuwaConfig(kconfig);
            //}

            // Add framework services.
            services.AddMvc(options =>
            {
                //options.Filters.Add(new AuthorizeFilter(guestPolicy));

            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.Formatting = Formatting.Indented;
            }).AddViewComponentsAsServices();
            services.AddKachuwaIdentitySever(hostingEnvironment, Configuration);
            //dual authorization support
            services.AddAuthentication(o =>
                    {// setting default authorization scheme ,authorize attribute apply default schem
                     //o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;


                    })
                .AddCookie(cfg => cfg.SlidingExpiration = true)//default scheme
                .AddJwtBearer(opts =>
                {
                    opts.Authority = Configuration["KachuwaAppConfig:SiteUrl"].ToString();
                    opts.Audience = "TixalayaApi";
                    opts.RequireHttpsMetadata = false;
                    opts.IncludeErrorDetails = true;
                });

            ////working example
            //services.AddIdentityServer()
            //    .AddDeveloperSigningCredential()
            //    .AddInMemoryApiResources(new[]
            //    {
            //        new ApiResource
            //        {
            //            Name = "MyApi",
            //            ApiSecrets = { new Secret("supersecret".Sha256()) },
            //            Scopes = { new Scope("myapi") },
            //        }
            //    })
            //    .AddInMemoryClients(new[]
            //    {
            //        new Client
            //        {
            //            ClientId = "ClientId",
            //            ClientSecrets = { new Secret("ClientId:ClientSecret".Sha256()) },
            //            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            //            AllowedScopes = { "myapi" },
            //        }
            //    })
            //    .AddTestUsers(new List<TestUser>
            //    {
            //        new TestUser
            //        {
            //            SubjectId = "some-unique-id-12345678980",
            //            Username = "john",
            //            Password = "123456"
            //        }
            //    });

            //services.AddAuthentication(o =>
            //    {
            //        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

            //    })
            //    .AddJwtBearer(opts =>
            //    {
            //        opts.Authority = "http://localhost:33641";
            //        opts.Audience = "MyApi";
            //        opts.RequireHttpsMetadata = false;

            //        opts.IncludeErrorDetails = true;
            //    });

            services.AddSession(options =>
            {
                options.Cookie.Name = ".Kachuwa.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(10);
            });
            services.AddLocalization();
            // File Sile
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = Int64.MaxValue;
                options.ValueLengthLimit = Int32.MaxValue;
                options.MultipartHeadersLengthLimit = Int32.MaxValue;
                options.ValueCountLimit = Int32.MaxValue;
            });

            //enable directory browsing
            //services.AddDirectoryBrowser();
            services.AddSignalR(o =>
            {


            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider,
            IHostingEnvironment env, ILoggerFactory loggerFactory,
            IOptions<ApplicationInsightsSettings> applicationInsightsSettings)
        {
            // loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();
            // loggerFactory.AddFile("Logs/ts-{Date}.txt");
        
            loggerFactory.UseKachuwaInsight(applicationInsightsSettings, serviceProvider);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Home/Error");
            }
            app.UseSession();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"Themes")),
                RequestPath = new PathString("/themes")
            });
            // app.UseDirectoryBrowser(new DirectoryBrowserOptions()
            // {
            //     FileProvider = new PhysicalFileProvider(
            //Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "images")),
            //     RequestPath = new PathString("/MyImages")
            // });
            //app.UseStaticFiles(); // For the wwwroot folder

            var provider = new FileExtensionContentTypeProvider();
            // Add new mappings
            provider.Mappings[".log"] = "text/plain";
            provider.Mappings[".txt"] = "text/plain";
            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), @"Logs")),
                RequestPath = new PathString("/dev/logs"),
                EnableDirectoryBrowsing = true,
                StaticFileOptions =
                {
                    DefaultContentType = "text/plain",
                    ContentTypeProvider = provider

                }

            });
            app.UseAuthentication();

            app.UseIdentityServer();

            // app.UseMiddleware<WebTokenMiddleware>();
            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //IdentityServerAuthenticationOptions identityServerValidationOptions = new IdentityServerAuthenticationOptions
            //{
            //    Authority = "http://localhost:11258/",
            //    // "http://kachuwaframework.com",//
            //    AllowedScopes = new List<string> { "dataEventRecords" },
            //    ApiSecret = "dataEventRecordsSecret",
            //    ApiName = "dataEventRecords",
            //    AutomaticAuthenticate = false,

            //    SupportedTokens = SupportedTokens.Both,
            //    // TokenRetriever = _tokenRetriever,
            //    // required if you want to return a 403 and not a 401 for forbidden responses
            //    AutomaticChallenge = true,
            //    RequireHttpsMetadata = false

            //};
            //app.UseIdentityServerAuthentication(identityServerValidationOptions);

            //app.Use(async (context, next) =>
            //{
            //    context.Response.Headers.Add("Access-Control-Allow-Origin", "SAMEORIGIN");
            //    await next();
            //});
            app.UseStaticHttpContext();
            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715
            //var externalCookieScheme = app.ApplicationServices.GetRequiredService<IOptions<IdentityOptions>>().Value.Cookies.ExternalCookieAuthenticationScheme;
            //app.UseGoogleAuthentication(new GoogleOptions
            //{
            //    AuthenticationScheme = "Google",
            //    SignInScheme = externalCookieScheme,
            //    ClientId = "998042782978-s07498t8i8jas7npj4crve1skpromf37.apps.googleusercontent.com",
            //    ClientSecret = "HsnwJri_53zn7VcO1Fm7THBb",
            //});

            // app.UseMiddleware<ChatWebSocketMiddleware>();
            //app.UseMiddleware<HeaderLogMiddleware>();
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
            //core
            app.UseKachuwaCore(serviceProvider);
            //web
            app.UseKachuwaWeb(true);

            app.UseSignalR(routes =>  // <-- SignalR
            {
                routes.MapHub<KachuwaNotificationHub>("/hubs/kachuwanotification");              

            });
            //on client side
            //let logger: ILogger;
            //let transportType: TransportType;
            //const hubConnetion = new HubConnection(
            //    new HttpConnection(
            //            url,

            //        {
            //            transport: transportType,
            //            logging: logger
            //        }));
        }
    }


}
