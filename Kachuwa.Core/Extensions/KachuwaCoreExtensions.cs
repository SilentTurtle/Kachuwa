using Kachuwa.Core.DI;
using Kachuwa.Data;
using Kachuwa.Data.Crud;
using Kachuwa.Web.Razor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kachuwa.Core.Extensions
{
    public static class KachuwaCoreExtensions
    {


        public static IServiceCollection RegisterKachuwaCoreServices(this IServiceCollection services, IConfigurationRoot configuration)
        {

            services.AddScoped<IViewRenderService, ViewRenderService>();
            //services.TryAddSingleton<IViewComponentSelector, Default2ViewComponentSelector>();
           // services.TryAddTransient<IViewComponentHelper, Default2ViewComponentHelper>();
            string conn = configuration.GetConnectionString("DefaultConnection");
            IDatabaseFactory dbFactory = DatabaseFactories.GetFactory(Dialect.SQLServer, conn);
            // services.AddScoped(typeof(IDatabaseFactory)).
            var asdf = new Bootstrapper(services, configuration);
            services.AddSingleton(configuration);

            return services;
            // Add application services.
            //services.AddTransient<IEmailSender, EmailSender>();
            //services.AddTransient<ISmsSender, SmsSender>();
        }
    }
}
