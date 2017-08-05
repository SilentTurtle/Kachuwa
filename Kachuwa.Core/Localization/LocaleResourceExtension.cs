using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kachuwa.Localization
{
    public static class LocaleResourceExtension
    {
        public static IServiceCollection EnableLocalization(this IServiceCollection sevices,Action<LocaleSetting> config  )
        {
            var setting=new LocaleSetting();
            config(setting);
            sevices.TryAddSingleton<LocaleSetting>(setting);

            sevices.TryAddSingleton<ILocaleService, LocaleService>();
            sevices.TryAddSingleton<ResourceBuilder>();
            sevices.TryAddSingleton<ILocaleResourceProvider,LocaleResourceProvider>();

            return sevices;
        }
        public static IApplicationBuilder UseLocalization(this IApplicationBuilder app)
        {
           var builder= app.ApplicationServices.GetService<ResourceBuilder>();
            Task.Run(async () => { await builder.Build(); });
            return app;
        }
    }
}