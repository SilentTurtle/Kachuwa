using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace Kachuwa.Web.Optimizer
{
    public interface IKachuwaBundler
    {
        Task<HtmlString> BundleCss(string[] files);
        Task<HtmlString> BundleCss(string name, string[] files);
        Task<HtmlString> BundleJs(string[] files);
        Task<HtmlString> BundleJs(string name, string[] files);
    }
}
