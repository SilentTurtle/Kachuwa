//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc.ModelBinding;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.AspNetCore.Mvc.TagHelpers;
//using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using Microsoft.AspNetCore.Razor.TagHelpers;

//namespace Kachuwa.Web.TagHelpers
//{
//    [HtmlTargetElement("theme")]
//    public class ThemeTagHelper : TagHelper
//    {
//        // public ThemeContext Info { get; set; }

//        [HtmlAttributeName("view")]
//        public string ViewName { get; set; }

//        [HtmlAttributeName("controller")]
//        public string Controller { get; set; }

//        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
//        {
//            output.TagName = "section";
//            //// Component.InvokeAsync("Simple", new { number = 5 })
//            //output.Content.SetHtmlContent("<alert/>");
//            //output.TagMode = TagMode.StartTagAndEndTag;
//            //this will render all its child taghelper or html markups
//            var childContent = await output.GetChildContentAsync();
//            var xx = context.Items;

//            // var modalContext = context.Items[typeof(AlertTagHelper)];
//            output.Content.SetHtmlContent(childContent);

//            // output.SuppressOutput();
//        }
//    }
//}