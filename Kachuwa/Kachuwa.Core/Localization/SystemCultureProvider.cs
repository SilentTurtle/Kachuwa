using System;
using System.Threading.Tasks;
using Kachuwa.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Kachuwa.Localization
{


    public class SystemCultureProvider : RequestCultureProvider
    {
        public IConfigurationRoot Configuration { get; set; }

        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException();
            }

            var kachuwaConfigSnap=httpContext.RequestServices.GetService<IOptionsSnapshot<KachuwaAppConfig>>();
           // var kachuwaConfig = kachuwaConfigSnap.Value;
            var providerResultCulture = new ProviderCultureResult("");//kachuwaConfig.BaseCulture);

            return Task.FromResult(providerResultCulture);
        }
        //use
        // options.RequestCultureProviders.Insert(0, new SystemCultureProvider());
        // app.UseRequestLocalization(options);
    }
}