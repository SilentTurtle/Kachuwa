using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Kachuwa.Web
{
    public class WidgetRenderer : IWidgetRenderer, IHtmlContent
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly DefaultViewComponentHelper _componentHelper;

        private ViewContext ViewContext { get; set; }
        public WidgetRenderer(IWebHostEnvironment hostingEnvironment, IViewComponentHelper componentHelper )
        {
            _hostingEnvironment = hostingEnvironment;
            _componentHelper = componentHelper as DefaultViewComponentHelper;
        }
        public async Task<IHtmlContent> Render(IWidget widget,ViewContext viewContext)
        {
            var widgetHtml = new HtmlContentBuilder();
           _componentHelper.Contextualize(viewContext);
            widgetHtml.AppendHtml(await _componentHelper.InvokeAsync(widget.WidgetViewComponent, widget.Settings));
            //throw new System.NotImplementedException();
            return (IHtmlContent)widgetHtml;
        }

        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            throw new System.NotImplementedException();
        }
    }
}