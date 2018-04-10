using System;
using System.Threading.Tasks;
using Kachuwa.Configuration;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Kachuwa.Web.TagHelpers
{
    public class JsonLdTagHelperComponent : TagHelperComponent
    {
        private readonly ISeoService _seoService;
        private readonly KachuwaAppConfig _kachuwaAppConfig;

        //order to inject first or last
        public override int Order => 1;
        public JsonLdTagHelperComponent(ISeoService seoService, IOptionsSnapshot<KachuwaAppConfig> configSnapShot)
        {
            _seoService = seoService;
            _kachuwaAppConfig = configSnapShot.Value;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (_kachuwaAppConfig.IsInstalled)
            {
                if (string.Equals(context.TagName, "head", StringComparison.Ordinal))
                {
                    output.PostContent.AppendHtmlLine($"{await _seoService.GenerateJsonLdForPage()}");
                }
            }

        }
    }
}