using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Kachuwa.Web.TagHelpers
{

    [HtmlTargetElement("markdown")]
    public class MarkdownTagHelper : TagHelper
    {
        [HtmlAttributeName("text")]
        public string Text { get; set; }

        [HtmlAttributeName("source")]
        public ModelExpression Source { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
           
            output.TagName = "div";
            output.Content.SetHtmlContent("");
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}