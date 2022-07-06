using System.Reflection;
using Kachuwa.Core.DI;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Kachuwa.FCM
{
    public class ServiceRegistrar : IServiceRegistrar
    {
        public void Register(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddTransient<IFCMService, FCMService>();
            serviceCollection.AddTransient<IFCMDeviceService, FCMDeviceService>();
            var embeddedAssembly = new EmbeddedFileProvider(typeof(ServiceRegistrar).GetTypeInfo().Assembly);
            serviceCollection.Configure<MvcRazorRuntimeCompilationOptions>(opts => { opts.FileProviders.Add(embeddedAssembly); });

        }
        public void Update(IServiceCollection serviceCollection)
        {
        }
    }
}