using System.Reflection;
using Kachuwa.Core.DI;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Smidge;
using Smidge.Nuglify;

namespace Kachuwa.Web
{
    public class WebServiceRegistrar : IServiceRegistrar
    {
        private bool _isInstalled = false;
        public void Register(IServiceCollection services, IConfiguration configuration)
        {
            var str_isInstalled = configuration["KachuwaAppConfig:IsInstalled"].ToLower();
            _isInstalled = str_isInstalled != "false";

            services.RegisterKachuwaWebServices(_isInstalled, configuration);
            var embeddedAssembly = new EmbeddedFileProvider(typeof(WebServiceRegistrar).GetTypeInfo().Assembly);
            services.Configure<RazorViewEngineOptions>(opts =>
            {
                opts.FileProviders.Add(embeddedAssembly);
            });


        }

        public void Update(IServiceCollection serviceCollection)
        {
            if (_isInstalled)
            {
                // var builder = serviceCollection.BuildServiceProvider();
                //  var settingService = builder.GetService<ISettingService>();
                // serviceCollection.AddSingleton(settingService.CrudService.Get(1));
            }
        }
    }
}