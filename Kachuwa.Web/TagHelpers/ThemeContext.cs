using Microsoft.AspNetCore.Html;

namespace Kachuwa.Web.TagHelpers
{
    public class ThemeContext
    {
        public IHtmlContent Header { get; set; }
        public IHtmlContent Body { get; set; }
        public IHtmlContent Footer { get; set; }
    }
}