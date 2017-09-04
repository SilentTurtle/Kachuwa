using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Kachuwa.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Kachuwa.Core.Extensions;
using Kachuwa.Log;
using Kachuwa.Log.Insight;
using Kachuwa.Web;
using Kachuwa.Web.Middleware;
using Kachuwa.Web.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
                .AddJsonFile("config/kachuwaconfig.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

           
            services.AddSingleton(Configuration);
            var serviceProvider = services.BuildServiceProvider();


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
            services.RegisterKachuwaCoreServices(serviceProvider);
            services.AddLocalization();
            // Add framework services.
            services.AddMvc(options =>
            {
                //options.Filters.Add(new AuthorizeFilter(guestPolicy));

            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.Formatting = Formatting.Indented;
            }).AddViewComponentsAsServices();
            //enable directory browsing
            //services.AddDirectoryBrowser();




        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider,
            IHostingEnvironment env, ILoggerFactory loggerFactory,
            IOptions<ApplicationInsightsSettings> applicationInsightsSettings)
        {


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

            app.UseIdentity();
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
                new CultureInfo("en-US")
                //new CultureInfo("en-AU"),
                //new CultureInfo("en-GB"),
                //new CultureInfo("en"),
                //new CultureInfo("es-ES"),
                //new CultureInfo("es-MX"),
                //new CultureInfo("es"),
                //new CultureInfo("fr-FR"),
                //new CultureInfo("fr"),
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

        }
    }

}
