using System;
using System.Threading.Tasks;
using Kachuwa.Configuration;
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
        private readonly KachuwaAppConfig _kachuwaAppConfig;
        public override int Order => 1;
        public SEOMetaTagHelperComponent( ISeoService seoService, IActionContextAccessor actionContextAccessor, IOptionsSnapshot<KachuwaAppConfig> configSnapShot)
        {
            _seoService = seoService;
            _actionContextAccessor = actionContextAccessor;
            _kachuwaAppConfig = configSnapShot.Value;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (_kachuwaAppConfig.IsInstalled)
            { var area = _actionContextAccessor.ActionContext.RouteData.Values["area"];

                if (area == null)
                {
                    if (string.Equals(context.TagName, "head", StringComparison.Ordinal))
                    {
                        output.PreContent.AppendHtmlLine($"{await _seoService.GenerateMetaContents()}");
                    }
                }
            }
        }
    }
}