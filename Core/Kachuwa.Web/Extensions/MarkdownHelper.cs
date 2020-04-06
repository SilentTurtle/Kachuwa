using System.Globalization;
using Kachuwa.Web.Extensions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Threading.Tasks;
using CommonMark;

namespace Kachuwa.Web.Extensions
{
   
    public static class MarkdownHelper
    {
        public static string ToHtml(this string content)
        {
            var markdown = content;
            var html = CommonMarkConverter.Convert(markdown);
            return html ?? "";
        }
    }
}