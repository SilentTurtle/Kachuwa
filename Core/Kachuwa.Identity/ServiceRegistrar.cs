using System;
using Kachuwa.Core.DI;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Kachuwa.Identity.Models;
using Kachuwa.Identity.Extensions;
using Kachuwa.Identity.IdSrv;
using Microsoft.Extensions.Configuration;
using Kachuwa.Identity.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using IdentityUser = Kachuwa.Identity.Models.IdentityUser;
using IdentityRole = Kachuwa.Identity.Models.IdentityRole;
using Microsoft.IdentityModel.Logging;

namespace Kachuwa.Identity
{
    public class ServiceRegistrar : IServiceRegistrar
    {
        public void Register(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            IdentityModelEventSource.ShowPII = true;
            serviceCollection.TryAddTransient<IUserDeviceService, UserDeviceService>();
            serviceCollection.ConfigureIdentityCryptography(configuration.GetSection("DapperIdentityCryptography"));

            serviceCollection.AddScoped<IAppUserService, AppUserService>();
            serviceCollection.TryAddTransient<ILoginHistoryService, LoginHistoryService>();

            var builder = serviceCollection.AddIdentity<IdentityUser, IdentityRole>(x =>
                {
                    x.SignIn.RequireConfirmedEmail =
                        bool.Parse(configuration["KachuwaAppConfig:RequireConfirmedEmail"]);
                    x.Password.RequireDigit = false;
                    x.Password.RequiredLength = int.Parse(configuration["KachuwaAppConfig:PasswordLength"]);
                    x.Password.RequireLowercase = false;
                    x.Password.RequireNonAlphanumeric =
                        bool.Parse(configuration["KachuwaAppConfig:RequireNonAlphanumeric"]);
                    x.Password.RequireUppercase = bool.Parse(configuration["KachuwaAppConfig:RequireUppercase"]);


                }).UseDapperWithSqlServer()
                .AddClaimsPrincipalFactory
                    <Kachuwa.Identity.ClaimFactory.KachuwaClaimsPrincipalFactory<IdentityUser, IdentityRole>>()
                .AddDefaultTokenProviders();

            builder.AddIdentityServerUserClaimsPrincipalFactory();
            //setting up for multiple sub domain identity exchange cookie
            //identity cookie
            serviceCollection.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "_kachuwa_id";
                options.LoginPath = new PathString("/account/login");
                options.AccessDeniedPath = new PathString("/access-denied");
                options.LogoutPath = new PathString("/account/logout");
                options.CookieManager = new CookieManager();
                options.Cookie.Domain = configuration["KachuwaAppConfig:CookieDomain"];
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.HttpOnly = false;
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
                options.DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo("app_data\\keys"));

                options.Events.OnValidatePrincipal = async context =>
                {
                    const string claimType = "validatedat";
                    const int reValidateAfterMinutes = 5;

                    if (!(context.Principal?.Identity is ClaimsIdentity claimIdentity)) return;

                    //if (!context.Principal.HasClaim(c => c.Type == claimType) ||
                    //    DateTimeOffset.Now.UtcDateTime.Subtract(new DateTime(long.Parse(context.Principal.Claims.First(c => c.Type == claimType).Value))) > TimeSpan.FromMinutes(reValidateAfterMinutes))
                    //{
                    var mgr = context.HttpContext.RequestServices.GetRequiredService<SignInManager<IdentityUser>>();
                    var user = await mgr.UserManager.FindByNameAsync(claimIdentity.Name);
                    var sstamp = await mgr.UserManager.GetSecurityStampAsync(user);
                    if (user != null && claimIdentity.Claims.FirstOrDefault(c => c.Type == "AspNet.Identity.SecurityStamp")?.Value == sstamp)
                    {
                        claimIdentity.FindAll(claimType).ToList().ForEach(c => claimIdentity.TryRemoveClaim(c));
                        claimIdentity.AddClaim(new Claim(claimType, DateTimeOffset.Now.UtcDateTime.Ticks.ToString(), typeof(long).ToString()));
                        context.ShouldRenew = true;
                    }
                    else
                    {
                        context.RejectPrincipal();
                        await mgr.SignOutAsync();
                    }
                    //}
                };

            });
            serviceCollection.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.FromSeconds(30));

            serviceCollection.AddKachuwaIdentitySever(serviceCollection.BuildServiceProvider().GetService<IWebHostEnvironment>(),configuration);



        }

        public void Update(IServiceCollection serviceCollection)
        {

        }
    }
}
