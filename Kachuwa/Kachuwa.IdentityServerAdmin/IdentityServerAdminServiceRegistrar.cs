using System;
using Kachuwa.Core.DI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kachuwa.IdentityServerAdmin
{
    public class IdentityServerAdminServiceRegistrar:IServiceRegistrar
    {
        public void Update(IServiceCollection serviceCollection)
        {
           
        }

        public void Register(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            
        }
    }
}