using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kachuwa.Web.TagHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebApp.TagHelpers
{
    public class Class
    {
    }



    public class PluginTagHelper : TagHelper
    {

    }

    public class CacheTagHelper : TagHelper
    {
        
    }

    [HtmlTargetElement("alert")]
    public class AlertTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "h2";
            // Component.InvokeAsync("Simple", new { number = 5 })
            output.Content.SetHtmlContent("Alert html");
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
    
    //usage
    //<component name="RecentComments" params="new { take: 5, random: true }"></component>
}
