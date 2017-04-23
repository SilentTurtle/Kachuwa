using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Kachuwa.Web.TagHelpers
{
    class Class1
    {
    }
    public class WebInfo
    {
        public object Version { get; set; }
        public object CopyrightYear { get; set; }
        public object Approved { get; set; }
        public object TagsToShow { get; set; }
    }
    public class WebsiteInformationTagHelper : TagHelper
    {
        public WebInfo Info { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "section";
            // Component.InvokeAsync("Simple", new { number = 5 })
            output.Content.SetHtmlContent(
    $@"<ul><li><strong>Version:</strong> {Info.Version}</li>
<li><strong>Copyright Year:</strong> {Info.CopyrightYear}</li>
<li><strong>Approved:</strong> {Info.Approved}</li>
<li><strong>Number of tags to show:</strong> {Info.TagsToShow}</li></ul>");
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}
