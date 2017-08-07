using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.Configuration;
using Kachuwa.Identity.IdentityConfig;
using Kachuwa.Identity.IdSrv;
using Kachuwa.Identity.Models;
using Kachuwa.Identity.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.Identity
{
    public static class IdentityBuilderExtensions
    {

        public static void AddKachuwaIdentitySever(this IServiceCollection serviceCollection, IHostingEnvironment environment)
        {
          //  var cert = new X509Certificate2(Path.Combine(environment.ContentRootPath, "kachuwaframework.pfx"), "");

            serviceCollection.AddIdentityServer(config =>
                {
                    config.Events.RaiseSuccessEvents = true;
                    config.Events.RaiseFailureEvents = true;
                    config.Events.RaiseErrorEvents = true;
                    config.Authentication = new AuthenticationOptions()
                    {
                        CookieLifetime = TimeSpan.FromDays(30),
                        RequireAuthenticatedUserForSignOutMessage = false,
                    };
                    config.IssuerUri = "http://localhost:11258/";// "http://kachuwaframework.com";

                })
                //.AddSigningCredential(cert)
                .AddTemporarySigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(Resources.GetIdentityResources())
                .AddInMemoryApiResources(Resources.GetApiResources())
                .AddInMemoryClients(IdentityConfig.Clients.Get())
                .AddAspNetIdentity<IdentityUser>()
                .AddProfileService<CustomProfileService>()
                .AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>();
            //.AddClientStore<ClientStoreService>();
        }
    }
}