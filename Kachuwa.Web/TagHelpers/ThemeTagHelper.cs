using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Kachuwa.Web.TagHelpers
{
    [HtmlTargetElement("theme")]
    public class ThemeTagHelper : TagHelper
    {
        // public ThemeContext Info { get; set; }

        [HtmlAttributeName("view")]
        public string ViewName { get; set; }

        [HtmlAttributeName("controller")]
        public string Controller { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "section";
            //// Component.InvokeAsync("Simple", new { number = 5 })
            //output.Content.SetHtmlContent("<alert/>");
            //output.TagMode = TagMode.StartTagAndEndTag;
            //this will render all its child taghelper or html markups
            var childContent = await output.GetChildContentAsync();
            var xx = context.Items;

            // var modalContext = context.Items[typeof(AlertTagHelper)];
            output.Content.SetHtmlContent(childContent);

            // output.SuppressOutput();
        }
    }
    [HtmlTargetElement("input", Attributes = "asp-for")]
    public class MaxLengthTagHelper : TagHelper
    {
        public override int Order { get; } = int.MaxValue;

        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }
        public MaxLengthTagHelper(IHtmlGenerator generator)
        {
            Generator = generator;
        }
      

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        protected IHtmlGenerator Generator { get; }

        public override async void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);
            //For=new ModelExpression();
            var metadataProvider = new EmptyModelMetadataProvider();
            var metadata = metadataProvider.GetMetadataForType(typeof(Page));
            var explorer = new ModelExplorer(metadataProvider, metadata,new Page());
            For=new ModelExpression("Name",explorer);
            // Process only if 'maxlength' attribute is not present already
            if (For != null)
            {
                var tagBuilder = Generator.GenerateValidationMessage(
                    ViewContext,
                    For.ModelExplorer,
                    For.Name,
                    message: null,
                    tag: null,
                    htmlAttributes: null);

                if (tagBuilder != null)
                {
                    output.MergeAttributes(tagBuilder);

                    // Do not update the content if another tag helper targeting this element has already done so.
                    if (!output.IsContentModified)
                    {
                        // We check for whitespace to detect scenarios such as:
                        // <span validation-for="Name">
                        // </span>
                        var childContent = await output.GetChildContentAsync();
                        if (childContent.IsEmptyOrWhiteSpace)
                        {
                            // Provide default message text (if any) since there was nothing useful in the Razor source.
                            if (tagBuilder.HasInnerHtml)
                            {
                                output.Content.SetHtmlContent(tagBuilder.InnerHtml);
                            }
                        }
                        else
                        {
                            output.Content.SetHtmlContent(childContent);
                        }
                    }
                }
            }
        }
    }
}