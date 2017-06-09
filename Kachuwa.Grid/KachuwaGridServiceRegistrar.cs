using Kachuwa.Core.DI;
using Kachuwa.Log;
using Kachuwa.Web.Razor;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace Kachuwa.Grid
{
    public class KachuwaGridServiceRegistrar : IServiceRegistrar
    {
        public void Update(IServiceCollection serviceCollection)
        {
          
        }

        public void Register(IServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            var assp = new EmbeddedFileProvider(typeof(KachuwaGridServiceRegistrar).GetTypeInfo().Assembly);
            serviceCollection.Configure<RazorViewEngineOptions>(opts =>
            {
                opts.FileProviders.Add(assp);
            });
        }
    }
}