using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Kachuwa.Web.TagHelpers
{
    [HtmlTargetElement("module")]
    public class ModuleTagHelper : TagHelper
    {
        [HtmlAttributeName("name")]
        public string Name { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // var childContent = await output.GetChildContentAsync();
            //var modalContext = (ModalContext)context.Items[typeof(ModalTagHelper)];
            //modalContext.Body = childContent;
            //output.SuppressOutput();
            //LOAD view components here
            output.TagName = "h3";
            // Component.InvokeAsync("Simple", new { number = 5 })
            output.Content.SetHtmlContent("Module" + Name);
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}