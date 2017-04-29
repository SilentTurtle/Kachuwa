using System.IO;
using System.Text;
using System.Threading.Tasks;
using Kachuwa.Plugin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Kachuwa.Web.TagHelpers
{
    [HtmlTargetElement("plugin")]
    public class PluginTagHelper : TagHelper
    {
        private readonly PluginViewProvider _pluginViewProvider;
        private readonly DefaultViewComponentHelper _componentHelper;

        [HtmlAttributeName("name")]
        public string Name { get; set; }

        [HtmlAttributeName("component")]
        public string Component { get; set; }

        [HtmlAttributeName("params")]
        public object Params { get; set; }

        [ViewContext] // inform razor to inject
        public ViewContext ViewContext { get; set; }

        public PluginTagHelper(IViewComponentHelper componentHelper, PluginViewProvider pluginViewProvider)
        {
            _pluginViewProvider = pluginViewProvider;
            _componentHelper = componentHelper as DefaultViewComponentHelper;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            _componentHelper.Contextualize(ViewContext);

            var file = _pluginViewProvider.GetFileInfo($"Plugin/{Name}/{Component}.cshtml");
            if (file.Exists)
            {

                string viewHtml = "";
                var stream = file.CreateReadStream();
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    viewHtml = reader.ReadToEnd();
                }
                stream.Dispose();

                //TODO :: cache,render


                var tagHelperContent = output.Content.AppendHtml(viewHtml);
            }
            else
            {
                output.SuppressOutput();
            }
    }
    }
}