using System.Globalization;
using Kachuwa.Web.Extensions;

namespace Kachuwa.Web
{
   
    public static class StringHelper
    {
        public static string Ellipsis(this string content, int length)
        {
            if (content.Length <= length) return content;
            int pos = content.IndexOf(" ", length);
            if (pos >= 0)
                return content.Substring(0, pos) + "...";
            return content;
        }
    }
}