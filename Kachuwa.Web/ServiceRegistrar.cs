using Kachuwa.Core.DI;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Kachuwa.Identity.Models;
using Kachuwa.Identity.Extensions;
using Microsoft.Extensions.Configuration;
using Kachuwa.Identity.IdentityConfig;
using Kachuwa.Identity.IdSrv;
using Kachuwa.Log;
using Kachuwa.Web.Module;
using Kachuwa.Web.Services;

namespace Kachuwa.Web
{
    public class ServiceRegistrar : IServiceRegistrar
    {
        public void Register(IServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            serviceCollection.AddTransient<IEmailSender, EmailSender>();
            serviceCollection.AddTransient<ISmsSender, SmsSender>();
            var logger = serviceCollection.BuildServiceProvider().GetService<ILogger>();
            var modules = new ModuleRegistrar(serviceCollection, logger);
        }

        public void Update(IServiceCollection serviceCollection)
        {
            
        }
    }
}
