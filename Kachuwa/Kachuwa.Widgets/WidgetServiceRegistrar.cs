using System.Reflection;
using Kachuwa.Core.DI;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;

namespace Kachuwa.Widgets
{
    public class WidgetServiceRegistrar : IServiceRegistrar
    {
        public void Update(IServiceCollection serviceCollection)
        {

        }

        public void Register(IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddSingleton<IDashboardWidgetService,DashboardWidgetService>();
            var assp = new EmbeddedFileProvider(typeof(WidgetServiceRegistrar).GetTypeInfo().Assembly);
            services.Configure<RazorViewEngineOptions>(opts =>
            {
                opts.FileProviders.Add(assp);
            });
        }
    }
}