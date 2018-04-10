using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Kachuwa.Web.TagHelpers
{
    public class JsonLdTagHelperComponent : TagHelperComponent
    {
        private readonly ISeoService _seoService;

        //order to inject first or last
        public override int Order => 1;
        public JsonLdTagHelperComponent(ISeoService seoService)
        {
            _seoService = seoService;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            if (string.Equals(context.TagName, "head", StringComparison.Ordinal))
            {
                output.PostContent.AppendHtmlLine($"{await _seoService.GenerateJsonLdForPage()}");
            }
            
        }
    }
}