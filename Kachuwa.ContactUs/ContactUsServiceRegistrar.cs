using System.Reflection;
using Kachuwa.Core.DI;
using Kachuwa.Web.Razor;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

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
}