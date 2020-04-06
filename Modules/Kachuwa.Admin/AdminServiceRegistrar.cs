using Kachuwa.Core.DI;
using System.Reflection;
using Kachuwa.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;

namespace Kachuwa.Admin
{
    public class AdminServiceRegistrar : IServiceRegistrar
    {
        public void Update(IServiceCollection serviceCollection)
        {

        }

        public void Register(IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton<IPageService, PageService>();
            var assp = new EmbeddedFileProvider(typeof(AdminServiceRegistrar).GetTypeInfo().Assembly);
            //serviceCollection.Configure<RazorViewEngineOptions>(opts =>
            //{
            //    opts.FileProviders.Add(assp);
            //});
            serviceCollection.Configure<MvcRazorRuntimeCompilationOptions>(opts =>
opts.FileProviders.Add(assp)
);
        }
    }
}