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
using Microsoft.AspNetCore.Http;

namespace Kachuwa.Web
{
    public class ServiceRegistrar : IServiceRegistrar
    {
        public void Register(IServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            serviceCollection.AddTransient<IEmailSender, EmailSender>();
            serviceCollection.AddTransient<ISmsSender, SmsSender>();
			 var ctxaccessor=  serviceCollection.BuildServiceProvider().GetService<IHttpContextAccessor>();
            var ctx = new ContextResolver(ctxaccessor);
            serviceCollection.AddSingleton(ctx);
            var logger = serviceCollection.BuildServiceProvider().GetService<ILogger>();
            var modules = new ModuleRegistrar(serviceCollection, logger);
        }

        public void Update(IServiceCollection serviceCollection)
        {
            
        }
    }
}
