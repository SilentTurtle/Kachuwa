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
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using Kachuwa.Web.Service;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kachuwa.Web
{
    public class WebServiceRegistrar : IServiceRegistrar
    {
        public void Register(IServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            serviceCollection.RegisterKachuwaWebServices();
            var embeddedAssembly = new EmbeddedFileProvider(typeof(WebServiceRegistrar).GetTypeInfo().Assembly);
            serviceCollection.Configure<RazorViewEngineOptions>(opts =>
            {
                opts.FileProviders.Add(embeddedAssembly);
            });
        }

        public void Update(IServiceCollection serviceCollection)
        {
            var builder=serviceCollection.BuildServiceProvider();
            var settingService=builder.GetService<ISettingService>();
            serviceCollection.AddSingleton(settingService.CrudService.Get(1));
        }
    }
}
