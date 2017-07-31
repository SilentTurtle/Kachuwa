using Kachuwa.Core.DI;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Kachuwa.Identity.Models;
using Kachuwa.Identity.Extensions;
using Microsoft.Extensions.Configuration;
using Kachuwa.Identity.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kachuwa.Identity
{
    public class ServiceRegistrar : IServiceRegistrar
    {
        public void Register(IServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            serviceCollection.ConfigureIdentityCryptography(configuration.GetSection("DapperIdentityCryptography"));
            //      .ConfigureDapperIdentityCryptography(Configuration.GetSection("DapperIdentityCryptography"));

            serviceCollection.TryAddSingleton<IAppUserService, AppUserService>();

            serviceCollection.AddIdentity<IdentityUser, IdentityRole>(x =>
            {
                x.Password.RequireDigit = false;
                x.Password.RequiredLength = 8;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;
            }).UseDapperWithSqlServer()
                .AddClaimsPrincipalFactory
                <Kachuwa.Identity.ClaimFactory.KachuwaClaimsPrincipalFactory<IdentityUser, IdentityRole>>()
                .AddDefaultTokenProviders();


            serviceCollection.AddKachuwaIdentitySever(serviceCollection.BuildServiceProvider().GetService<IHostingEnvironment>());



        }

        public void Update(IServiceCollection serviceCollection)
        {
            
        }
    }
}
