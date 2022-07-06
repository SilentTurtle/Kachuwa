using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using Kachuwa.Caching;
using Kachuwa.Log;
using Kachuwa.Web.Layout;
using Kachuwa.Web.Module;
using Kachuwa.Web.Notification;
using Kachuwa.Web.Optimizer;
using Kachuwa.Web.Security;
using Kachuwa.Web.Service;
using Kachuwa.Web.Service.Installer;
using Kachuwa.Web.Services;
using Kachuwa.Web.TagHelpers;
using Kachuwa.Web.Templating;
using Kachuwa.Web.Theme;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Smidge;
using Smidge.Cache;
using Smidge.CompositeFiles;
using Smidge.FileProcessors;
using Smidge.Nuglify;
using Smidge.Options;

namespace Kachuwa.Web
{
    public static class KachuwaWebExtensions
    {
        public static IServiceCollection RegisterKachuwaWebServices(this IServiceCollection services,
            bool isInstalled, IConfiguration configuration)
        {
            services.AddAuthorization(options =>
            {
                //options.AddPolicy("Test", policy => policy.Requirements.Add(new HasScopeRequirement("read:messages", "xyz.com")));
                options.AddPolicy(PolicyConstants.PagePermission,
                    policy => policy.Requirements.Add(new PagePermissionRequirement()));

            });
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, PagePermissionHandler>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<ISettingService, SettingService>();
            services.TryAddSingleton<ILayoutRenderer, LayoutContentRenderer>();
            services.TryAddSingleton<ISeoService, SeoService>();
            services.TryAddSingleton<IPageService, PageService>();
            services.AddSingleton<ICountryService, CountryService>();
            services.AddSingleton<IAuditService, AuditService>();
            services.AddScoped<IKachuwaConfigurationManager, KachuwaConfigurationManager>();
            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger>();
            var pageService = serviceProvider.GetService<IPageService>();
            var cacheService = serviceProvider.GetService<ICacheService>();
            var contextAccessor = serviceProvider.GetService<IHttpContextAccessor>();

            services.Configure<MvcRazorRuntimeCompilationOptions>(opts =>
            {
                opts.FileProviders.Add(
                    new PageFileProvider(pageService, logger, cacheService, contextAccessor));
            });
            services.AddSingleton<IScriptRunner, SQLScriptRunner>();
            services.AddSingleton<IModuleService, ModuleService>();
            services.AddSingleton<IModuleManager, ModuleManager>();
            services.AddScoped<IModuleComponentProvider, ModuleComponentProvider>();

            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<ITemplateEngine, MustacheTemplateEngine>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            //services.AddScoped<ITagHelperComponent, SEOMetaTagHelperComponent>();
           // services.AddScoped<ITagHelperComponent, JsonLdTagHelperComponent>();
            //no use of time zone
            services.AddScoped<ITagHelperComponent, SystemVariablesTagHelperComponent>();
            //services.AddScoped<ITagHelperComponent, TokenTagHelperComponent>();
            services.RegisterNotificationService();

            //use IEmailServiceProviderService for sender
             services.AddScoped<IEmailSender, SmtpEmailSender>();
            //services.AddScoped<IEmailSender, SendGridEmailSender>();
            // services.AddScoped<IEmailSender, MailJetEmailSender>();

            // services.AddSingleton<ISmsSender, SmsSender>();
            services.AddSingleton<IEmailTemplateService, EmailTemplateService>();
            
            services.TryAddTransient<IEmailLogService, EmailLogService>();
            services.TryAddTransient<IEmailServiceProviderService, EmailServiceProviderService>();
          //  services.AddSingleton<ISMSService, SMSService>();
            services.AddSingleton<ITemplateDataSourceManager, TemplateDataSourceManager>();
            services.AddScoped<IWidgetRenderer, WidgetRenderer>();
            services.AddSingleton<IWidgetService, WidgetService>();
            services.AddSingleton<IDashboardWidgetManager, DashboardWidgetManager>();
            services.AddSingleton<IImportService, ImportService>();
            services.AddSingleton<IExportService, ExportService>();
            services.AddSingleton<IPermissionService, PermissionService>();
            services.AddSingleton<ISMSLogService, SMSLogService>();
            services.AddSingleton<ISMSService, SMSService>();
            services.AddSingleton<IRestrictionService, RestrictionService>();
            services.AddSingleton<ISMSTemplateService, SMSTemplateService>();
            services.AddSingleton<IDbBakRestoreService, DbBakRestoreService>();
            services.AddSingleton<ISmsSender, AAKASHSMSSender>();
            //****for testing******//
            //services.TryAddSingleton<IRazorViewEngine, RazorViewEngine2>();
            //services.TryAddSingleton<IView,Razor2View>();

            //var ctxaccessor = services.BuildServiceProvider().GetService<IHttpContextAccessor>();
            //var ctx = new ContextResolver(ctxaccessor);
            //services.AddSingleton(ctx);
            services.AddSingleton<IPaymentService, PaymentService>();
            if (isInstalled)
            {
                try
                {
                    var modules = new ModuleRegistrar(services, logger);
                }
                catch (Exception e)
                {//if any db error occurs then restarting appliation.
                  var applicationLife=  serviceProvider.GetService<IHostApplicationLifetime>();
                  applicationLife.StopApplication();

                }
                
            }

            var hostingProvider = serviceProvider.GetService<IWebHostEnvironment>();
            services.AddSmidge(configuration.GetSection("smidge") );
            services.AddSmidgeNuglify();
            services.Configure<SmidgeOptions>(opt =>
            {
                opt.DefaultBundleOptions.DebugOptions.SetCacheBusterType<AppDomainLifetimeCacheBuster>();
                opt.DefaultBundleOptions.ProductionOptions.SetCacheBusterType<AppDomainLifetimeCacheBuster>();
                opt.DefaultBundleOptions.ProductionOptions.CompressResult = false;
                opt.DefaultBundleOptions.ProductionOptions.ProcessAsCompositeFile = true;
                opt.UrlOptions = new UrlManagerOptions
                {
                    BundleFilePath = "optimizer",
                    MaxUrlLength = 2048,
                    CompositeFilePath = "optimizerx"
                };

                opt.PipelineFactory.OnCreateDefault = (type, pipeline) => pipeline.Replace<JsMinifier, NuglifyJs>(opt.PipelineFactory);
            });
            services.AddScoped<IKachuwaBundler, SmidgeBundler>();
          
            services.RegisterThemeService(config =>
            {
                config.Directory = "Themes";
                config.LayoutName = "_layout";
            });

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
                }).AddNewtonsoftJson(options => {
                    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.Formatting = Formatting.Indented;
                })
                //.AddJsonOptions(options =>
                //{
                //    options.JsonSerializerOptions.PropertyNamingPolicy = null;//camel casing
                //    options.JsonSerializerOptions.WriteIndented = true;
                //    options.JsonSerializerOptions.DictionaryKeyPolicy = null;

                //    // options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                //    // options.SerializerSettings.Formatting = Formatting.Indented;
                //})
                .AddViewLocalization()
                .AddRazorRuntimeCompilation()
                .AddViewComponentsAsServices(); //.SetCompatibilityVersion(CompatibilityVersion.Version_2_2); 
            //dual authorization support
            services.AddControllersWithViews().AddNewtonsoftJson();
            services.AddControllers().AddNewtonsoftJson();
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.ConsentCookie.Domain = configuration["KachuwaAppConfig:CookieDomain"];
                // options.MinimumSameSitePolicy = SameSiteMode.None;
                options.ConsentCookie.Name = "_kachuwa_consent";

            });
            services.AddSession(options =>
            {
                options.Cookie.Name = ".Kachuwa.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.IsEssential = true; // make the session cookie Essential
            });
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = Int64.MaxValue;
                options.MultipartBoundaryLengthLimit = Int32.MaxValue;
                options.ValueLengthLimit = Int32.MaxValue;
                options.MultipartHeadersLengthLimit = Int32.MaxValue;
                options.ValueCountLimit = Int32.MaxValue;
            });
            return services;
        }
        public static IApplicationBuilder UseKachuwaWeb(this IApplicationBuilder app,IWebHostEnvironment hostingEnvironment, bool useDefaultRoute=true)
        {

            app.UseMiddleware<ModuleResourceMiddleware>();
            var provider = new FileExtensionContentTypeProvider();
            // Add new mappings
            provider.Mappings[".log"] = "text/plain";
            provider.Mappings[".txt"] = "text/plain";
            // Add new mappings


            provider.Mappings[".mpd"] = "application/dash+xml";
            provider.Mappings[".m4a"] = "audio/mp4";
            provider.Mappings[".aac"] = "audio/aac";
            provider.Mappings[".m4a"] = "audio/mp4";
            provider.Mappings[".m3u"] = "audio/x-mpegurl";
            provider.Mappings[".m4s"] = "video/iso.segment";
            provider.Mappings[".m3u8"] = "application/vnd.apple.mpegURL";
            provider.Mappings[".ts"] = "video/MP2T";
            provider.Mappings[".vtt"] = "text/vtt";
            provider.Mappings[".srt"] = "text/srt";
            if (!Directory.Exists(Path.Combine(hostingEnvironment.ContentRootPath, "Logs")))
                Directory.CreateDirectory(Path.Combine(hostingEnvironment.ContentRootPath, "Logs"));
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
            if (!Directory.Exists(Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot", @"lib")))
                Directory.CreateDirectory(Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot", @"lib"));
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(hostingEnvironment.ContentRootPath, @"wwwroot", "lib")),
                RequestPath = new PathString("/lib")
            });
            if (!Directory.Exists(Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot", @"assets")))
                Directory.CreateDirectory(Path.Combine(hostingEnvironment.ContentRootPath, "wwwroot", @"assets"));
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(hostingEnvironment.ContentRootPath, @"wwwroot", "assets")),
                RequestPath = new PathString("/assets")
            });
            if (!Directory.Exists(Path.Combine(hostingEnvironment.ContentRootPath, "Themes")))
                Directory.CreateDirectory(Path.Combine(hostingEnvironment.ContentRootPath, "Themes"));
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(hostingEnvironment.ContentRootPath, @"Themes")),
                RequestPath = new PathString("/themes")
            });
            if (!Directory.Exists(Path.Combine(hostingEnvironment.ContentRootPath, "Locale")))
                Directory.CreateDirectory(Path.Combine(hostingEnvironment.ContentRootPath, "Locale"));
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
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseAuthorization();
            app.UseIdentityServer();
           
            if (useDefaultRoute)
            {
                app.UseRoutes();
            }
            app.UseSmidge();
            //app.UseSmidgeNuglify();
            return app;
        }
        public static IApplicationBuilder UseRoutes(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{pageUrl?}",
                    defaults: new { controller = "KachuwaPage", action = "Index" }
                   //, constraints: new { pageUrl = @"\w+" }
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
         
            return app;
        }

    }
}