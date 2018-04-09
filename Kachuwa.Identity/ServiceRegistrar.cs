using Kachuwa.Core.DI;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Kachuwa.Identity.Models;
using Kachuwa.Identity.Extensions;
using Microsoft.Extensions.Configuration;
using Kachuwa.Identity.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kachuwa.Identity
{
    public class ServiceRegistrar : IServiceRegistrar
    {
        public void Register(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.ConfigureIdentityCryptography(configuration.GetSection("DapperIdentityCryptography"));
            //      .ConfigureDapperIdentityCryptography(Configuration.GetSection("DapperIdentityCryptography"));

            serviceCollection.AddScoped<IAppUserService, AppUserService>();

            serviceCollection.AddIdentity<IdentityUser, IdentityRole>(x =>
            {
                x.SignIn.RequireConfirmedEmail = bool.Parse(configuration["KachuwaAppConfig:RequireConfirmedEmail"]);
                x.Password.RequireDigit = false;
                x.Password.RequiredLength =int.Parse(configuration["KachuwaAppConfig:PasswordLength"]);
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = bool.Parse(configuration["KachuwaAppConfig:RequireNonAlphanumeric"]);
                x.Password.RequireUppercase = bool.Parse(configuration["KachuwaAppConfig:RequireUppercase"]); ;
            }).UseDapperWithSqlServer()
                .AddClaimsPrincipalFactory
                <Kachuwa.Identity.ClaimFactory.KachuwaClaimsPrincipalFactory<IdentityUser, IdentityRole>>()
                .AddDefaultTokenProviders();


           // serviceCollection.AddKachuwaIdentitySever(serviceCollection.BuildServiceProvider().GetService<IHostingEnvironment>());



        }

        public void Update(IServiceCollection serviceCollection)
        {
            
        }
    }
}
