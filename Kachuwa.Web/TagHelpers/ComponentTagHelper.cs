using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Kachuwa.Web.TagHelpers
{
    [HtmlTargetElement("component")]
    public class ComponentTagHelper : TagHelper
    {
        private readonly DefaultViewComponentHelper _componentHelper;

        [HtmlAttributeName("name")]
        public string Name { get; set; }

        [HtmlAttributeName("params")]
        public object Params { get; set; }

        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }

        public ComponentTagHelper(IViewComponentHelper componentHelper)
        {
            _componentHelper = componentHelper as DefaultViewComponentHelper;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            _componentHelper.Contextualize(ViewContext);

            output.Content.AppendHtml(
                await _componentHelper.InvokeAsync(Name, Params)
            );
        }
    }
}