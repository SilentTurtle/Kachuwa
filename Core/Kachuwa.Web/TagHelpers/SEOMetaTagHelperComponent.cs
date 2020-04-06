using System;
using System.Threading.Tasks;
using Kachuwa.Caching;
using Kachuwa.Configuration;
using Kachuwa.Log;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Kachuwa.Web.TagHelpers
{
    public class SEOMetaTagHelperComponent : TagHelperComponent
    {
        private readonly ISeoService _seoService;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ILogger _logger;
        private readonly ICacheService _cacheService;
        private readonly KachuwaAppConfig _kachuwaAppConfig;
        public override int Order => 1;
        public SEOMetaTagHelperComponent( ISeoService seoService, IActionContextAccessor actionContextAccessor,
            IOptionsSnapshot<KachuwaAppConfig> configSnapShot
            , ILogger logger, ICacheService cacheService)
        {
            _seoService = seoService;
            _actionContextAccessor = actionContextAccessor;
            _logger = logger;
            _cacheService = cacheService;
            _kachuwaAppConfig = configSnapShot.Value;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (_kachuwaAppConfig.IsInstalled)
            { var area = _actionContextAccessor.ActionContext.RouteData.Values["area"];

                if (area == null)
                {
                   
                    try
                    {
                        if (string.Equals(context.TagName, "head", StringComparison.Ordinal))
                        {
                            string metatags = await _cacheService.GetAsync<string>("SEOMetaTagHelper", async () => await _seoService.GenerateMetaContents(), TimeSpan.FromHours(1));
                            output.PreContent.AppendHtmlLine($"{metatags}");
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Log(LogType.Error, () => e.Message, e);
                    }
                }
            }
        }
    }
}