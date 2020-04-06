using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

namespace Kachuwa.Localization
{
  

    public static class LocaleResourceExtension
    {
        public static IServiceCollection EnableKachuwaLocalization(this IServiceCollection services,Action<LocaleSetting> config  )
        {
            var setting=new LocaleSetting();
            config(setting);
            services.TryAddSingleton<LocaleSetting>(setting);
            services.TryAddSingleton<ILocaleService, LocaleService>();
            services.TryAddSingleton<ResourceBuilder>();
            services.TryAddSingleton<ILocaleResourceProvider,LocaleResourceProvider>();
            //services.Configure<MvcDataAnnotationsLocalizationOptions>(options =>
            //{
            //    options.DataAnnotationLocalizerProvider = (type, factory) => new DataAnnotationLocalizer();
            //});
           
            return services;
        }
        public static IApplicationBuilder UseKachuwaLocalization(this IApplicationBuilder app)
        {
           var builder= app.ApplicationServices.GetService<ResourceBuilder>();
            Task.Run(async () => { await builder.Build(); });
            return app;
        }
    }
}