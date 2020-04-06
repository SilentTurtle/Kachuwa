using System.Reflection;
using Kachuwa.Core.DI;
using Kachuwa.HtmlContent.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;

namespace Kachuwa.HtmlContent
{
    public class HtmlModuleServiceRegistrar : IServiceRegistrar
    {
        public void Update(IServiceCollection serviceCollection)
        {
           
        }

        public void Register(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddTransient<IHtmlContentService, HtmlContentService>();
            var assp = new EmbeddedFileProvider(typeof(HtmlModule).GetTypeInfo().Assembly);
            serviceCollection.Configure<MvcRazorRuntimeCompilationOptions>(opts =>
            {
                opts.FileProviders.Add(assp);
            });
        }
    }
}