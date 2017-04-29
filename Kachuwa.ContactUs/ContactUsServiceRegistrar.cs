using System.Collections.Generic;
using Kachuwa.Core.DI;
using Kachuwa.Web.Razor;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using Kachuwa.Web.Module;

namespace Kachuwa.ContactUs
{
    public class ContactUsServiceRegistrar : IServiceRegistrar
    {
        public void Update(IServiceCollection serviceCollection)
        {

        }

        public void Register(IServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            serviceCollection.AddTransient<IContactUsService, ContactUsService>();
            var assp = new EmbeddedFileProvider(typeof(ContactUsInfo).GetTypeInfo().Assembly);
            serviceCollection.Configure<RazorViewEngineOptions>(opts =>
            {
                opts.FileProviders.Add(assp);
            });
        }
    }

    public class ContactUsModule : IModule
    {
        public string Name { get; set; }="ContactUs";
        public string Version { get; set; } = "1.0.0.0";
        public List<string> SupportedVersions { get; set; }=new List<string>(){"1.0.0"};
        public string Author { get; set; } = "Binod Tamang";
        public Assembly Assembly { get; set; } = typeof(ContactUsModule).GetTypeInfo().Assembly;
    }
}