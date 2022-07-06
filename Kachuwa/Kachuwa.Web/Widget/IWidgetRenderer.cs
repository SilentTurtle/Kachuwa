using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kachuwa.Web
{
    public interface IWidgetRenderer
    {
        Task<IHtmlContent> Render(IWidget widget,ViewContext viewContext);
    }
}