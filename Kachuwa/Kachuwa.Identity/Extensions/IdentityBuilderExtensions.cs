using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.Configuration;
using IdentityServer4.Test;
using Kachuwa.Identity.IdentityConfig;
using Kachuwa.Identity.IdSrv;
using Kachuwa.Identity.Models;
using Kachuwa.Identity.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Kachuwa.Identity
{
    public static class IdentityBuilderExtensions
    {

        public static void AddKachuwaIdentitySever(this IServiceCollection serviceCollection, IWebHostEnvironment environment,IConfiguration configuration)
        {
            //  var cert = new X509Certificate2(Path.Combine(environment.ContentRootPath, "kachuwaframework.pfx"), "");
            //serviceCollection.AddIdentityServer()
            //    .AddDeveloperSigningCredential()
            //    .AddInMemoryPersistedGrants()
            //    .AddInMemoryIdentityResources(Resources.GetIdentityResources())
            //    .AddInMemoryApiResources(Resources.GetApiResources())
            //    .AddInMemoryClients(IdentityConfig.Clients.Get())
            //    .AddAspNetIdentity<IdentityUser>();


            serviceCollection.AddIdentityServer(config =>
                {
                    config.Events.RaiseSuccessEvents = true;
                    config.Events.RaiseFailureEvents = true;
                    config.Events.RaiseErrorEvents = true;
                    //config.Authentication = new AuthenticationOptions()
                    //{
                    //    CookieLifetime = TimeSpan.FromDays(30),
                    //    RequireAuthenticatedUserForSignOutMessage = false,
                    //};
                    config.IssuerUri = configuration["KachuwaAppConfig:SiteUrl"].ToString(); ; // "http://kachuwaframework.com";

                })
                //.AddSigningCredential(cert)
                //.AddTemporarySigningCredential()
                //.AddDeveloperSigningCredential()
                //.AddInMemoryPersistedGrants()
                //.AddInMemoryIdentityResources(Resources.GetIdentityResources())
                //.AddInMemoryApiResources(Resources.GetApiResources())
                //.AddInMemoryClients(IdentityConfig.Clients.Get())
                //.AddAspNetIdentity<IdentityUser>()
                //.AddProfileService<CustomProfileService>()
                //.AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>();
                .AddDeveloperSigningCredential()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(Resources.GetIdentityResources())
                .AddInMemoryApiScopes(Resources.Apis)
                .AddInMemoryApiResources(Resources.GetApiResources())
                .AddInMemoryClients(IdentityConfig.Clients.Get())

                .AddAspNetIdentity<IdentityUser>()
                .AddProfileService<CustomProfileService>()
                .AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>();
            //.AddClientStore<ClientStoreService>();
            //.AddTestUsers(new List<TestUser>(){new TestUser()
            //{
            //    Username = "Binod",
            //    Password = "kachuwa",
            //    SubjectId = "B01",

            //}});
        }
    }
}